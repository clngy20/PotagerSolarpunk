 using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    private PlayerControls controls;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject seedPrefab; // Prefab de la graine à planter
    [SerializeField] private float groundY = 0.1f; // Hauteur (Y) à laquelle la graine sera plantée

    //WATER
    [SerializeField] private ParticleSystem waterParticles;
    [SerializeField] private float waterRadius = 0.5f;

    //SCORE
    [SerializeField] private ScoreManager scoreManager;

    private Vector2 moveDirection;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Plant.performed += OnPlantPressed; // "Quand on appuie, appelle ma fonction"
    }

    private void OnDisable()
    {
        controls.Player.Plant.performed -= OnPlantPressed; // "Ne m'appelle plus" (l'obligatoire inverse)
        controls.Disable();
    }

    private void Update()
    {
        MoveDrone();
        HandleWatering();
    }

    private void MoveDrone()
    {
        moveDirection = controls.Player.Move.ReadValue<Vector2>(); // Lit la direction (clavier OU manette)

        Vector3 horizontalMovement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.Translate(horizontalMovement * moveSpeed * Time.deltaTime);
    }

    private void OnPlantPressed(InputAction.CallbackContext context)
    {
        // Même x/z que le drone, mai y au sol : la graine est sous le drone
        Vector3 seedPosition = new Vector3(transform.position.x, groundY, transform.position.z);
        GameObject seed = Instantiate(seedPrefab, seedPosition, Quaternion.identity);
        if (seed.TryGetComponent(out PlantGrowth plant))
        {
            plant.Initialize(scoreManager);
        }

    }

    //WATER
    private void HandleWatering()
    {
        bool isWatering = controls.Player.Water.IsPressed(); // Vérifie si le bouton d'arrosage est enfoncé
        if (isWatering && !waterParticles.isPlaying)
        {
            waterParticles.Play();
        }
        else if (!isWatering && waterParticles.isPlaying)
        {
            waterParticles.Stop();
        }

        if (!isWatering) return;

        Vector3 center = new Vector3(transform.position.x, groundY, transform.position.z);
        Collider[] hits = Physics.OverlapSphere(center, waterRadius);

        foreach (Collider hit in hits)
        {
            if(hit.TryGetComponent(out PlantGrowth plant))
            {
                plant.Water();
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, groundY, transform.position.z), waterRadius);
    }
}
