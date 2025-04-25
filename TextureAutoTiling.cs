using UnityEngine;

namespace RimuruDev
{
    public enum TilingAxis : byte
    {
        XY = 0,
        XZ = 1,
        YZ = 2
    }

    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    [HelpURL("https://github.com/RimuruDev/Unity-TextureAutoTiling-Prototype")]
    public class TextureAutoTiling : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int MainTexSt = Shader.PropertyToID("_MainTex_ST");

        [Header("Settings")] 
        public Texture Texture;
        public float TileSize = 1f;
        public TilingAxis TilingAxis = TilingAxis.XY;

        private void Start() =>
            ApplyTiling();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                ApplyTiling();
        }
#endif
        private void ApplyTiling()
        {
            var targetRenderer = GetComponent<Renderer>();
            if (targetRenderer == null || targetRenderer.sharedMaterial == null)
                return;

            var block = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(block);

            if (Texture != null)
                block.SetTexture(MainTex, Texture);

            var bounds = targetRenderer.bounds;
            var size = bounds.size;

            var tiling = TilingAxis switch
            {
                TilingAxis.XY => new Vector2(size.x / TileSize, size.y / TileSize),
                TilingAxis.XZ => new Vector2(size.x / TileSize, size.z / TileSize),
                TilingAxis.YZ => new Vector2(size.y / TileSize, size.z / TileSize),
                _ => Vector2.one
            };

            block.SetVector(MainTexSt, new Vector4(tiling.x, tiling.y, 0, 0));
            targetRenderer.SetPropertyBlock(block);
        }
    }
}
