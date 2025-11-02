using UnityEngine;
using UnityEngine.UI;

public class NoiseOverlayScroll : MonoBehaviour
{
    public RawImage noiseImage;
    public float scrollSpeed = 0.1f;
    public float fadeSpeed = 0.5f;

    void Update()
    {
        // move the noise texture to simulate smoke
        noiseImage.uvRect = new Rect(Time.time * scrollSpeed, 0, 1, 1);

        // gradually fade out to reveal the text
        var col = noiseImage.color;
        col.a = Mathf.Lerp(col.a, 0, Time.deltaTime * fadeSpeed);
        noiseImage.color = col;
    }
}
