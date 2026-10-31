 using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    private PlayerControls controls;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject seedPrefab; // Prefab de la graine à planter
    [SerializeField] private float groundY = 0.1f; // Hauteur (Y) à laquelle la graine sera plantée

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
    }

    private void MoveDrone()
    {
        moveDirection = controls.Player.Move.ReadValue<Vector2>(); // Lit la direction (clavier OU manette)

        Vector3 horizontalMovement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        transform.Translate(horizontalMovement * moveSpeed * Time.deltaTime);
    }

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    private void OnPlantPressed(InputAction.CallbackContext context)
    {
        // Même x/z que le drone, mai y au sol : la graine est sous le drone
        Vector3 seedPosition = new Vector3(transform.position.x, groundY, transform.position.z);
        Instantiate(seedPrefab, seedPosition, Quaternion.identity);

    }
}
