using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteUVScroller : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.5f, 0f); // UV units per second
    [SerializeField] private Vector2 uvTiling = new Vector2(1f, 1f);

    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;
    private Vector2 uvOffset;

    static readonly int UVOffsetID = Shader.PropertyToID("_UVOffset");
    static readonly int UVTilingID = Shader.PropertyToID("_UVTiling");

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        uvOffset = Vector2.zero;
    }

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Texture tex = sr.sprite.texture;
        Debug.Log($"Sprite texture in use: {tex.name}");
    }

    void OnEnable()
    {
        sr.GetPropertyBlock(mpb);
        mpb.SetVector(UVOffsetID, uvOffset);
        mpb.SetVector(UVTilingID, uvTiling);
        sr.SetPropertyBlock(mpb);
    }

    void Update()
    {
        uvOffset += scrollSpeed * Time.deltaTime;

        // Keep values from creeping to infinity (optional but nice)
        if (uvOffset.x > 1000f || uvOffset.y > 1000f) uvOffset = new Vector2(Mathf.Repeat(uvOffset.x, 1f), Mathf.Repeat(uvOffset.y, 1f));

        sr.GetPropertyBlock(mpb);
        mpb.SetVector(UVOffsetID, uvOffset);
        sr.SetPropertyBlock(mpb);
    }
}
