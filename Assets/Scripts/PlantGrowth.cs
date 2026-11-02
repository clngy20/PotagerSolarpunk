using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public enum GrowthState
    {
        Seed,
        Ripe,
        Overripe,
        Burst
    }

    [SerializeField] private float timeToRipe = 5f; //Secondes avant d'être mûr
    [SerializeField] private float timeToOverripe = 5f; //Secondes de plus avant de pourrir
    [SerializeField] private float timeToBurst = 5f; //Secondes de plus avant d'exploser

    [SerializeField] private Color seedColor = Color.white;
    [SerializeField] private Color ripeColor = Color.green;
    [SerializeField] private Color overripeColor = new Color(0.5f, 0.3f, 0f);

    public GrowthState CurrentState { get; private set; } = GrowthState.Seed;

    private float timer;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        ApplyColor(CurrentState); //Couleur initiale
    }

    private void Update()
    {
        timer += Time.deltaTime;

        switch (CurrentState)
        {
            case GrowthState.Seed when timer>= timeToRipe:
                SetState(GrowthState.Ripe);
                break;
            case GrowthState.Ripe when timer >= timeToRipe + timeToOverripe:
                SetState(GrowthState.Overripe);
                break;
            case GrowthState.Overripe when timer>= timeToRipe + timeToOverripe + timeToBurst:
                SetState(GrowthState.Burst);
                break;
        }
    }

    private void SetState(GrowthState newState)
    {
        CurrentState = newState;
        Debug.Log($"{name} -> {newState}", this);
        ApplyColor(newState);

        if (newState == GrowthState.Burst)
        {
            Burst();
        }
    }

    //Destruction de la graine (légume plus tard)
    private void Burst()
    {
        // Plus tard : son et particules d'éclatement ici
        Destroy(gameObject);
    }

    private void ApplyColor(GrowthState state)
    {
        rend.material.color = state switch
        {
            GrowthState.Ripe => ripeColor,
            GrowthState.Overripe => overripeColor,
            _ => seedColor
        };
    }
}
