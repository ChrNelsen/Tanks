using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PulsatingEmission : MonoBehaviour
{
    [Tooltip("If left white, the script will use the material's current emission color.")]
    [SerializeField] Color emissionColor = Color.white; 

    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float minIntensity = 0.1f;
    [SerializeField] float maxIntensity = 2f;

    [Tooltip("Material slot to apply the pulsating emission.")]
    [SerializeField] int materialIndex = 1; 

    private Material mat;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        // Safety check for invalid material index
        if (materialIndex < 0 || materialIndex >= rend.materials.Length)
        {
            Debug.LogWarning($"{name}: Material index {materialIndex} is invalid. Using first material instead.");
            materialIndex = 0;
        }

        mat = rend.materials[materialIndex];
        mat.EnableKeyword("_EMISSION");

        // Use material's current emission color if user hasn't assigned one
        if (emissionColor == Color.white && mat.HasProperty("_EmissionColor"))
        {
            emissionColor = mat.GetColor("_EmissionColor");
        }
    }

    private void Update()
    {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // Normalize to 0-1
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);

        Color finalColor = emissionColor * intensity;
        mat.SetColor("_EmissionColor", finalColor);
        DynamicGI.SetEmissive(rend, finalColor); // Optional GI update
    }
}
