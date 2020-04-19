// 2016 - Damien Mayance (@Valryon)
// Source: https://github.com/valryon/water2d-unity/

using UnityEngine;

/// <summary>
///     Automagically create a water reflect for a sprite.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class WaterReflectableScript : MonoBehaviour {
    #region Members

    [Header("Reflect properties")] public Vector3 localPosition = new Vector3(0, -0.25f, 0);

    public Vector3 localRotation = new Vector3(0, 0, -180);

    [Tooltip("Optionnal: force the reflected sprite. If null it will be a copy of the source.")] public Sprite sprite;

    public string spriteLayer = "Map";
    public int spriteLayerOrder = 2;


    private SpriteRenderer spriteSource;
    private SpriteRenderer spriteRenderer;
    public Material spriteMaterial;



    #endregion

    #region Timeline

    private void Awake() {
        var reflectGo = new GameObject("Water Reflect");
        reflectGo.transform.SetParent(transform);
        //reflectGo.transform.parent = transform;
        reflectGo.transform.localPosition = localPosition;
        reflectGo.transform.localRotation = Quaternion.Euler(localRotation);
        reflectGo.transform.localScale = new Vector3(reflectGo.transform.localScale.x, reflectGo.transform.localScale.y,
            reflectGo.transform.localScale.z);

        spriteRenderer = reflectGo.AddComponent<SpriteRenderer>();
        spriteRenderer.material = spriteMaterial;
        spriteRenderer.sortingLayerName = spriteLayer;
        spriteRenderer.sortingOrder = spriteLayerOrder;

        spriteSource = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy() {
        if (spriteRenderer != null)
            Destroy(spriteRenderer.gameObject);
    }

    private void LateUpdate() {
        if (spriteSource != null) {
            if (sprite == null)
                spriteRenderer.sprite = spriteSource.sprite;
            else
                spriteRenderer.sprite = sprite;
            spriteRenderer.flipX = spriteSource.flipX;
            spriteRenderer.flipY = spriteSource.flipY;
            spriteRenderer.color = spriteSource.color;
        }
    }

    #endregion
}