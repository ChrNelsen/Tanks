using UnityEngine;

public class SpawnEffect2 : MonoBehaviour
{
    [Header("Effect Settings")]
    public float spawnEffectTime = 2f;          // Duration of the dissolve/fade-in
    public AnimationCurve fadeIn;               // Curve controlling the fade

    private Renderer _renderer;
    private int shaderProperty;
    private float timer = 0f;

    void Start()
    {
        // Cache shader property ID for performance
        shaderProperty = Shader.PropertyToID("_cutoff");

        // Get the renderer of this object
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("No Renderer found on this GameObject!");
        }
    }

    void Update()
    {
        if (_renderer == null || fadeIn == null)
            return;

        // Increment timer until spawnEffectTime
        if (timer < spawnEffectTime)
        {
            timer += Time.deltaTime;

            // Normalize timer to 0-1 and evaluate fadeIn curve
            float value = fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer));

            // Apply to shader
            _renderer.material.SetFloat(shaderProperty, value);
        }
    }
}