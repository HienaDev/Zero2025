using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TMP_FakeSmokeFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;      // total fade length
    public float fadeSpeed = 2f;           // how fast each letter fades once triggered
    public float randomDelayRange = 0.5f;  // randomness per letter (seconds)

    private TMP_Text textMesh;
    private Coroutine fadeRoutine;

    void Awake() => textMesh = GetComponent<TMP_Text>();

    void OnEnable()
    {
        // optional auto fade-in on enable
        FadeIn();
    }

    // --- Public API ------------------------------------------------------

    public void FadeIn()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeLetters(true));
    }

    public void FadeOut()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeLetters(false));
    }

    // --- Core fade logic -------------------------------------------------

    IEnumerator FadeLetters(bool fadeIn)
    {
        textMesh.ForceMeshUpdate();
        var info = textMesh.textInfo;
        int count = info.characterCount;
        if (count == 0) yield break;

        // Assign a random delay for each visible character
        var delays = new float[count];
        for (int i = 0; i < count; i++)
            delays[i] = Random.Range(0f, randomDelayRange);

        float timer = 0f;
        float totalTime = fadeDuration + randomDelayRange;

        while (timer < totalTime)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < count; i++)
            {
                var c = info.characterInfo[i];
                if (!c.isVisible) continue;

                int matIndex = c.materialReferenceIndex;
                int vertIndex = c.vertexIndex;
                var colors = info.meshInfo[matIndex].colors32;

                // Calculate normalized fade 0-1 for this character
                float t = Mathf.InverseLerp(delays[i], delays[i] + fadeDuration / fadeSpeed, timer);
                t = Mathf.Clamp01(t);

                // Apply fade direction
                float alpha01 = fadeIn ? Mathf.SmoothStep(0, 1, t) : Mathf.SmoothStep(1, 0, t);
                byte alpha = (byte)(alpha01 * 255);

                for (int j = 0; j < 4; j++)
                    colors[vertIndex + j].a = alpha;
            }

            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }

        // Ensure final alpha values are correct at the end
        for (int i = 0; i < count; i++)
        {
            var c = info.characterInfo[i];
            if (!c.isVisible) continue;

            int matIndex = c.materialReferenceIndex;
            int vertIndex = c.vertexIndex;
            var colors = info.meshInfo[matIndex].colors32;

            byte alpha = (byte)(fadeIn ? 255 : 0);
            for (int j = 0; j < 4; j++)
                colors[vertIndex + j].a = alpha;
        }
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        if(!fadeIn) gameObject.SetActive(false);
    }
}
