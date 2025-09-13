using System.Collections;
using UnityEngine;

public class DissolveTest : MonoBehaviour
{
    [Header("Dissolve Settings")]
    [SerializeField] private Material dissolveMaterialTemplate; // Dissolve shader material template
    [SerializeField] private float dissolveDuration = 2f;       // Duration of the dissolve effect

    private Renderer rend;                 
    private Material[] originalMaterials;  
    private Material[] dissolveMaterials;  

    // Cache edge color info from the originals
    private Color edgeEmissionColor;
    private float edgeEmissionStrength;
    private Texture edgeEmissionMap;

    // Shader property IDs for performance
    private static readonly int DissolveAmountID = Shader.PropertyToID("_Dissolve_Amount");
    private static readonly int MainColorID      = Shader.PropertyToID("_Main_Color");
    private static readonly int EmissionColorID  = Shader.PropertyToID("_EmissionColor");
    private static readonly int EmissionStrengthID = Shader.PropertyToID("_EmissionStrength");
    private static readonly int EmissionMapID    = Shader.PropertyToID("_EmissionMap");
    private static readonly int EdgeColorID      = Shader.PropertyToID("_Edge_Color");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterials = rend.materials;
    }

    private void Start()
    {
        PlayDissolve();
    }

    public void PlayDissolve()
    {
        CacheEmissionData();
        BuildDissolveMaterials();

        rend.materials = dissolveMaterials;
        StartCoroutine(DissolveRoutine());
    }

    private void CacheEmissionData()
    {
        foreach (var original in originalMaterials)
        {
            if (original.HasProperty(EmissionColorID))
            {
                edgeEmissionColor = original.GetColor(EmissionColorID);
                edgeEmissionStrength = Mathf.Max(edgeEmissionColor.r, edgeEmissionColor.g, edgeEmissionColor.b);
            }

            if (original.HasProperty(EmissionMapID))
            {
                edgeEmissionMap = original.GetTexture(EmissionMapID);
            }
        }
    }

    private void BuildDissolveMaterials()
    {
        dissolveMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            var original = originalMaterials[i];
            var dissolveMat = new Material(dissolveMaterialTemplate);

            CopyBaseColor(original, dissolveMat);
            CopyEmission(original, dissolveMat);
            CopyEmissionMap(original, dissolveMat);

            // Always apply cached edge emission color
            if (dissolveMat.HasProperty(EdgeColorID))
                dissolveMat.SetColor(EdgeColorID, edgeEmissionColor);

            dissolveMaterials[i] = dissolveMat;
        }
    }

    private void CopyBaseColor(Material original, Material dissolveMat)
    {
        if (original.HasProperty("_Color") && dissolveMat.HasProperty(MainColorID))
            dissolveMat.SetColor(MainColorID, original.GetColor("_Color"));
    }

    private void CopyEmission(Material original, Material dissolveMat)
    {
        if (original.HasProperty(EmissionColorID) && dissolveMat.HasProperty(EmissionColorID))
        {
            var emissionColor = original.GetColor(EmissionColorID);
            var emissionStrength = Mathf.Max(emissionColor.r, emissionColor.g, emissionColor.b);

            dissolveMat.SetColor(EmissionColorID, emissionColor);
            dissolveMat.SetFloat(EmissionStrengthID, emissionStrength);
            dissolveMat.EnableKeyword("_EMISSION");
        }
    }

    private void CopyEmissionMap(Material original, Material dissolveMat)
    {
        if (original.HasProperty(EmissionMapID) && original.GetTexture(EmissionMapID) != null &&
            dissolveMat.HasProperty(EmissionMapID))
        {
            var emissionMap = original.GetTexture(EmissionMapID);
            dissolveMat.SetTexture(EmissionMapID, emissionMap);
            dissolveMat.SetTextureScale(EmissionMapID, original.GetTextureScale(EmissionMapID));
            dissolveMat.SetTextureOffset(EmissionMapID, original.GetTextureOffset(EmissionMapID));
        }
    }

    private IEnumerator DissolveRoutine()
    {
        float time = 0f;

        while (time < dissolveDuration)
        {
            float t = time / dissolveDuration;

            foreach (var mat in dissolveMaterials)
            {
                if (mat.HasProperty(DissolveAmountID))
                    mat.SetVector(DissolveAmountID, new Vector4(t, 0f, 0f, 0f));
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure fully dissolved
        foreach (var mat in dissolveMaterials)
        {
            if (mat.HasProperty(DissolveAmountID))
                mat.SetVector(DissolveAmountID, new Vector4(1f, 0f, 0f, 0f));
        }

        // Restore original materials
        rend.materials = originalMaterials;
    }
}
