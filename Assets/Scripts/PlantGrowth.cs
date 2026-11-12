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

    //AUDIO
    [Header("Audio")]
    [SerializeField] private AudioClip ripeSound;    //Ding quand le légume devient mûr
    [SerializeField] private AudioClip burstSound;   //Son d'explosion
    [SerializeField] private AudioClip harvestSound; //Son de récolte
    [SerializeField][Range(0f, 1f)] private float soundVolume = 0.7f;

    public GrowthState CurrentState { get; private set; } = GrowthState.Seed;

    private float timer;

    //Animation
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer[] renderers;

    //Explosion et récolte
    [SerializeField] private GameObject burstEffectPrefab;
    [SerializeField] private GameObject harvestEffectPrefab;

    [Header("Visuels par état")]
    [SerializeField] private GameObject seedModel;
    [SerializeField] private GameObject carrotModel;

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
            case GrowthState.Seed when timer >= timeToRipe:
                SetState(GrowthState.Ripe);
                break;
            case GrowthState.Ripe when timer >= timeToRipe + timeToOverripe:
                SetState(GrowthState.Overripe);
                break;
            case GrowthState.Overripe when timer >= timeToRipe + timeToOverripe + timeToBurst:
                SetState(GrowthState.Burst);
                break;
        }
    }

    private void SetState(GrowthState newState)
    {
        CurrentState = newState;
        ApplyColor(newState);

        if (animator != null)
        {
            animator.SetInteger("State", (int)newState);
        }

        //Sons d'état (PlayClipAtPoint : pas besoin d'AudioSource sur l'objet)
        switch (newState)
        {
            case GrowthState.Ripe:
                PlaySound(ripeSound);
                break;
            case GrowthState.Burst:
                PlaySound(burstSound);
                break;
        }

        if (newState == GrowthState.Burst)
        {
            Burst();
        }
    }

    //Joue un clip à la position de la plante (crée un AudioSource temporaire)
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }

    //Destruction de la graine
    private void Burst()
    {
        if (burstEffectPrefab != null)
        {
            Instantiate(burstEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void Water()
    {
        if (CurrentState == GrowthState.Seed)
        {
            timer += waterBonusPerSecond * Time.deltaTime;
        }
    }

    private void ApplyColor(GrowthState state)
    {
        // Quel modèle est visible
        if (seedModel != null)
        {
            seedModel.SetActive(state == GrowthState.Seed);
        }
        if (carrotModel != null)
        {
            carrotModel.SetActive(state != GrowthState.Seed);
        }

        // On ne teinte que la carotte pourrie ; sinon on garde les couleurs d'origine
        if (state != GrowthState.Overripe)
        {
            return;
        }

        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                m.color = overripeColor;
            }
        }
    }

    //SCORE
    // Appelée par le drone juste après l'Instantiate (injection)
    public void Initialize(ScoreManager manager)
    {
        scoreManager = manager;
    }

    private void OnTriggerStay(Collider other)
    {
        if (harvested || CurrentState != GrowthState.Ripe)
        {
            return;
        }
        if (other.GetComponentInParent<DroneController>() == null)
        {
            return;
        }
        Harvest();
    }

    private void Harvest()
    {
        harvested = true;

        if (scoreManager != null)
        {
            scoreManager.AddScore(scoreValue);
        }

        //Son de récolte AVANT la destruction (sinon il serait coupé)
        PlaySound(harvestSound);

        if (harvestEffectPrefab != null)
        {
            Instantiate(harvestEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}