using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public enum GrowthState
    {
        Seed,
        Ripe,
        Overripe
    }

    [SerializeField] private float timeToRipe = 5f; //Secondes avant d'être mûr
    [SerializeField] private float timeToOverripe = 5f; //Secondes de plus avant de pourrir

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
        }
    }

    private void SetState(GrowthState newState)
    {
        CurrentState = newState;
        Debug.Log($"{name} -> {newState}", this);
        ApplyColor(newState);
    }

    private void ApplyColor(GrowthState state)
    {
        rend.material.color = state switch
        {
            GrowthState.Ripe => Color.green,
            GrowthState.Overripe => new Color(0.5f, 0.3f, 0f), //marron
            _ => Color.white //Couleur par défaut pour l'état Seed
        };
    }
}
