using UnityEngine;
using System.Collections;

public class FlashOnCollision : MonoBehaviour
{
    [SerializeField] private float flashMultiplier = 3f;
    [SerializeField] private float fadeDuration = 0.3f;

    private Renderer rend;
    private Material[] mats;
    private Coroutine[] flashes;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mats = rend.materials;
        flashes = new Coroutine[mats.Length];
    }

    public void Flash()
    {
        for (int i = 0; i < mats.Length; i++)
        {
            if (!mats[i].HasProperty("_EmissionColor"))
                continue;

            mats[i].EnableKeyword("_EMISSION");

            // Stop any ongoing flash to restart
            if (flashes[i] != null)
                StopCoroutine(flashes[i]);

            flashes[i] = StartCoroutine(FlashEmission(mats[i], i));
        }
    }

    private IEnumerator FlashEmission(Material mat, int index)
    {
        // Always start from the original emission
        Color original = mat.GetColor("_EmissionColor");
        Color flash = original * flashMultiplier;

        mat.SetColor("_EmissionColor", flash);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            mat.SetColor("_EmissionColor", Color.Lerp(flash, original, elapsed / fadeDuration));
            yield return null;
        }

        mat.SetColor("_EmissionColor", original);
        flashes[index] = null;
    }
}
