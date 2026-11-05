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
    [SerializeField] private float waterBonusPerSecond = 1f;

    [SerializeField] private Color seedColor = Color.white;
    [SerializeField] private Color ripeColor = Color.green;
    [SerializeField] private Color overripeColor = new Color(0.5f, 0.3f, 0f);

    //SCORE
    [SerializeField] private int scoreValue = 10;
    private ScoreManager scoreManager;
    private bool harvested;

    public GrowthState CurrentState { get; private set; } = GrowthState.Seed;

    private float timer;

    [SerializeField] private Animator animator;
    [SerializeField] private Renderer rend;

    private void Start()
    {
        ApplyColor(CurrentState); //Couleur initiale
    }

    private void Update()
    {
        if (harvested) return;

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

        if (animator != null)
        {
            animator.SetInteger("State", (int)newState);
        }

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
     
    public void Water()
    {
        if(CurrentState == GrowthState.Seed)
        {
            timer += waterBonusPerSecond * Time.deltaTime;
        }
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

    //SCORE
    // Appelée par le drone juste après l'Instantiate (injection)
    public void Initialize(ScoreManager manager)
    {
        scoreManager = manager;
    }

    private void OnTriggerStay(Collider other)
    {
        if (harvested || CurrentState != GrowthState.Ripe) return;
        if (other.GetComponentInParent<DroneController>() == null) return;

        Harvest();
    }

    private void Harvest()
    {
        harvested = true;
        if (scoreManager != null)
        {
            scoreManager.AddScore(scoreValue);
        }
        Destroy(gameObject);
    }
}
