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

    [Header("Limites du potage")]
    [SerializeField] private Vector2 mapMin = new Vector2(-11.5f, -7f); // Coin bas gauche du potager
    [SerializeField] private Vector2 mapMax = new Vector2(1.2f, 5.8f); // Coin haut droit du potager

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
        moveDirection = controls.Player.Move.ReadValue<Vector2>();

        Vector3 horizontalMovement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.Translate(horizontalMovement * moveSpeed * Time.deltaTime, Space.World);

        // Le drone ne sort pas du potager
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, mapMin.x, mapMax.x);
        pos.z = Mathf.Clamp(pos.z, mapMin.y, mapMax.y);
        transform.position = pos;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((mapMin.x + mapMax.x) / 2f, groundY, (mapMin.y + mapMax.y) / 2f);
        Vector3 size = new Vector3(mapMax.x - mapMin.x, 0.1f, mapMax.y - mapMin.y);
        Gizmos.DrawWireCube(center, size);
    }
}
