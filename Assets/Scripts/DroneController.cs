using UnityEngine;
using UnityEngine.InputSystem;

//Pilote le drone : déplacement, plantation et arrosage.
//Le drone reste dans les limites du potager.

public class DroneController : MonoBehaviour
{
    //Classe générée par l'Input System : donne accès aux actions Move, Plant, Water, Pause
    private PlayerControls controls;

    [SerializeField] private float moveSpeed = 5f; //Vitesse de déplacement
    [SerializeField] private GameObject seedPrefab; //Prefab de la graine à planter
    [SerializeField] private float groundY = 0.1f; //Hauteur (Y) à laquelle la graine sera plantée

    //WATER
    [SerializeField] private ParticleSystem waterParticles; //Jet d'eau
    [SerializeField] private float waterRadius = 0.5f; //Rayon de la zone arrosée au sol

    //SCORE
    [SerializeField] private ScoreManager scoreManager; //Donné à chaque plante à sa création

    private Vector2 moveDirection; //Direction (clavier ou manette)

    //Potager
    [SerializeField] private Vector2 mapMin = new Vector2(-11.5f, -7f); //Coin bas gauche du potager
    [SerializeField] private Vector2 mapMax = new Vector2(1.2f, 5.8f); //Coin haut droit du potager

    private void Awake()
    {
        //Création des contrôles avant OnEnable
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Plant.performed += OnPlantPressed; //"Quand on appuie, appelle ma fonction"
    }

    private void OnDisable()
    {
        controls.Player.Plant.performed -= OnPlantPressed; //"Ne m'appelle plus"
        controls.Disable();
    }

    private void Update()
    {
        //Appelées à chaque image
        MoveDrone();
        HandleWatering();
    }

    //Déplace le drone sur le plan horizontal et le garde dans le potager
    private void MoveDrone()
    {
        moveDirection = controls.Player.Move.ReadValue<Vector2>();

        //Le Y du Vector2 devient l'axe Z du monde : la hauteur du drone ne change jamais
        Vector3 horizontalMovement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.Translate(horizontalMovement * moveSpeed * Time.deltaTime, Space.World);

        //Le drone ne sort pas du potager
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, mapMin.x, mapMax.x); //Limite gauche/droite
        pos.z = Mathf.Clamp(pos.z, mapMin.y, mapMax.y); //Limite bas/haut (le Y du Vector2 = Z du monde)
        transform.position = pos;
    }

    //Appelée quand on appuie sur la touche Planter
    private void OnPlantPressed(InputAction.CallbackContext context)
    {
        //Même x/z que le drone, mais y au sol : la graine est sous le drone
        Vector3 seedPosition = new Vector3(transform.position.x, groundY, transform.position.z);
        GameObject seed = Instantiate(seedPrefab, seedPosition, Quaternion.identity);

        //la plante reçoit le score au moment de sa création
        if (seed.TryGetComponent(out PlantGrowth plant))
        {
            plant.Initialize(scoreManager);
        }

    }

    //WATER
    //Tant que la touche est maintenue : particules + bonus de croissance aux plantes sous le drone
    private void HandleWatering()
    {
        bool isWatering = controls.Player.Water.IsPressed(); //Vérifie si le bouton d'arrosage est enfoncé

        //Lance ou arrête les particules selon l'état de la touche
        if (isWatering && !waterParticles.isPlaying)
        {
            waterParticles.Play();
        }
        else if (!isWatering && waterParticles.isPlaying)
        {
            waterParticles.Stop();
        }

        if (!isWatering) return; //Rien à arroser si la touche n'est pas enfoncée

        //Zone d'arrosage : une sphère au sol, sous le drone
        Vector3 center = new Vector3(transform.position.x, groundY, transform.position.z);
        Collider[] hits = Physics.OverlapSphere(center, waterRadius);

        //Chaque plante touchée reçoit le bonus de croissance
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out PlantGrowth plant))
            {
                plant.Water();
            }
        }
    }

    //Aide visuelle (drone sélectionné) : cercle cyan = zone d'arrosage
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, groundY, transform.position.z), waterRadius);
    }

    //Aide visuelle (scene view) : rectangle jaune = limites du potager
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((mapMin.x + mapMax.x) / 2f, groundY, (mapMin.y + mapMax.y) / 2f);
        Vector3 size = new Vector3(mapMax.x - mapMin.x, 0.1f, mapMax.y - mapMin.y);
        Gizmos.DrawWireCube(center, size);
    }
}