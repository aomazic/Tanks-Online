using UnityEngine;

public class PlanetControl : MonoBehaviour 
{
    [Header("Planet Settings")]
    [Range(10, 200)]
    [SerializeField] private float pixels = 100f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 lightPosition = new Vector2(0.5f, 0.5f);
    
    [SerializeField] private GameObject planet;
    
    private float time;
    private IPlanet planetComponent;
    private Camera mainCamera;

    private void Start()
    {
        GenerateRandomSeed();
        planetComponent = planet?.GetComponent<IPlanet>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Application.isPlaying) return;

        if (Input.GetMouseButton(1))
        {
            lightPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            UpdateLightPosition();
        }

        time += Time.deltaTime;
        UpdateTime(time);
    }

    private void UpdateLightPosition() => planetComponent?.SetLight(lightPosition);

    private void UpdateTime(float currentTime) => planetComponent?.UpdateTime(currentTime);

    private void GenerateRandomSeed()
    {
        seed = Random.Range(0, int.MaxValue);
        planetComponent?.SetSeed(seed);
    }
}