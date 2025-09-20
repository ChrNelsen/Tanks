using System.Collections;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float minDelay = 1f;
    public float maxDelay = 5f;
    public float fadeDuration = 1f;

    [Header("Pulse Settings")]
    public bool pulsate = false;
    [SerializeField] float pulseSpeed = 2f;
    [SerializeField] float minIntensity = 0.1f;
    [SerializeField] float maxIntensity = 2f;

    [Tooltip("Material slot to apply the pulsating emission.")]
    [SerializeField] int materialIndex = 0; // safer default

    [Header("Variance Controls")]
    [SerializeField, Range(0f, 1f)] float colorVariance = 0f;     // how far hue/sat/val can shift

    private Renderer cubeRenderer;
    private Material[] materials;
    private Color[] baseColors;
    private Color[] baseEmissions;
    private Color emissionColor;
    private bool hasFadedIn;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null)
        {
            cubeRenderer.enabled = false; // Hide at start for fade effect

            materials = cubeRenderer.materials;
            baseColors = new Color[materials.Length];
            baseEmissions = new Color[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                baseColors[i] = materials[i].color;
                if (materials[i].HasProperty("_EmissionColor"))
                    baseEmissions[i] = materials[i].GetColor("_EmissionColor");
                else
                    baseEmissions[i] = Color.black;

                // Make sure emission starts at black
                if (materials[i].HasProperty("_EmissionColor"))
                    materials[i].SetColor("_EmissionColor", Color.black);
            }
        }

        StartCoroutine(FadeInAfterRandomDelay());
    }

    private IEnumerator FadeInAfterRandomDelay()
    {
        // wait a random time before starting fade
        float waitTime = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(waitTime);

        // Enable renderer now
        cubeRenderer.enabled = true;

        float elapsed = 0f;

        // Fade In Loop
        while (elapsed < fadeDuration)  // Lasts as long as fadeDuration input
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Seperate loop to apply fade to each material
            for (int i = 0; i < materials.Length; i++)
            {
                Color c = baseColors[i];    // Start with the original color
                c.a = t;                    // fade alpha from 0 → 1
                materials[i].color = c;

                // If the material supports emission, fade emission color from black to original
                if (materials[i].HasProperty("_EmissionColor"))
                {
                    // Multiply the original emission color by t (0→1)
                    // At t=0, color is black; at t=1, it's full emission
                    materials[i].SetColor("_EmissionColor", baseEmissions[i] * t);
                }
            }
            yield return null;
        }

        // Finalize
        for (int i = 0; i < materials.Length; i++)
        {
            // Make sure the material color ends exactly at the original color
            materials[i].color = baseColors[i];

            // If the material has emission, set it fully to the original value
            if (materials[i].HasProperty("_EmissionColor"))
            {
                // Set emission color to the original value (no longer multiplied by t)
                materials[i].SetColor("_EmissionColor", baseEmissions[i]);

                // Enable emission keyword so Unity knows this material uses emission
                materials[i].EnableKeyword("_EMISSION");

                // Set base Emissions color for pulsating effect
                emissionColor = baseEmissions[i];
            }
        }
        hasFadedIn = true;
        // ApplyVariance();  // Needs to be put in correct spot
    }

    private void Update()
    {
        if (hasFadedIn && pulsate)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // Normalize to 0-1
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);

            Color finalColor = emissionColor * intensity;
            materials[materialIndex].SetColor("_EmissionColor", finalColor);
        }
    }

    private void ApplyVariance()
    {
        int matIndex = 0;

        foreach (Color material in baseColors)
        {
            // Convert to HSV
            Color.RGBToHSV(material, out float h, out float s, out float v);

            // Apply random shifts in given ranges
            // Need to double check what this is doing, also decide if I just want difference in
            // shade vs color
            h = Mathf.Repeat(h + Random.Range(-colorVariance, colorVariance), 1f);
            s = Mathf.Clamp01(s + Random.Range(-colorVariance, colorVariance));
            v = Mathf.Clamp01(v + Random.Range(-colorVariance, colorVariance));

            // Convert back to RGB
            Color variedColor = Color.HSVToRGB(h, s, v);

            // Apply to material
            cubeRenderer.materials[matIndex].color = variedColor;

            matIndex++;
        }
    }
}