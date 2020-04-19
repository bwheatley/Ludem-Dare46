// 2016 - Damien Mayance (@Valryon)
// Source: https://github.com/valryon/water2d-unity/

using System.Collections;
using UnityEngine;

/// <summary>
///     Water surface script (update the shader properties).
/// </summary>
public class Water2DScript : MonoBehaviour {
    private Material mat;

    [Tooltip("By default use a mesh, other times use a sprite?")]
    public bool material = true;

    private Renderer rend;
    private MeshFilter mf;
    public Vector2 speed = new Vector2(0.01f, 0f);

    public Sprite HexSprite;
    public bool generateNewMesh = false;

    public bool runCoroutine = true;

    private void Awake() {

        //Convert the sprite to a mesh
        mf = this.GetComponent<MeshFilter>();

        if (!mf) {
            //Only set the shared mesh if it's not already set
            Debug.Log(string.Format("Mesh Null setting it"));

// #if UNITY_EDITOR
//             if (generateNewMesh) {
//                 var path = "Assets/wargame/Resources/Mesh/";
//                 var filename = string.Format("PointyHexMesh.asset");
//                 var fullpath = string.Format("{0}{1}", path, filename);
//                 UnityEditor.AssetDatabase.CreateAsset(Util.SpriteToMesh(HexSprite), fullpath);
//             }
// #endif
        }
        rend = GetComponent<Renderer>();
        mat = rend.material;

    }

    private void OnEnable() {
        //StartCoroutine(onCoroutine());
    }

    private void Start() {
    }

    IEnumerator onCoroutine() {
        while (runCoroutine) {
            if (GameManager.instance.showWaterMovement) {
                var scroll = Time.deltaTime * speed;
                mat.mainTextureOffset += scroll;
                //Util.WriteDebugLog("Run Coroutine");
            }

            yield return null;
            //yield return new WaitForSeconds(.1f);
        }
    }

    private void LateUpdate() {
        //TODO convert this to a shader for the movement and see if it works nicer
        if (GameManager.instance.showWaterMovement && runCoroutine) {
            var scroll = Time.deltaTime * speed;
            mat.mainTextureOffset += scroll;
        }

    }


}
