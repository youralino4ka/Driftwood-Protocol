using UnityEngine;

namespace Driftwood.Water
{
    [RequireComponent(typeof(Renderer))]
    public class WaterScroller : MonoBehaviour
    {
        [SerializeField] private Vector2 mainTextureSpeed = new Vector2(0.02f, 0.01f);
        [SerializeField] private Vector2 normalTextureSpeed = new Vector2(-0.01f, 0.015f);

        private Renderer _renderer;
        private Material _instanceMaterial;
        private Vector2 _mainOffset;
        private Vector2 _normalOffset;

        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _instanceMaterial = _renderer.material;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            _mainOffset += mainTextureSpeed * dt;
            _normalOffset += normalTextureSpeed * dt;

            if (_instanceMaterial.HasProperty(MainTex))
            {
                _instanceMaterial.SetTextureOffset(MainTex, _mainOffset);
            }

            if (_instanceMaterial.HasProperty(BumpMap))
            {
                _instanceMaterial.SetTextureOffset(BumpMap, _normalOffset);
            }
        }

        private void OnDestroy()
        {
            if (_instanceMaterial != null)
            {
                Destroy(_instanceMaterial);
            }
        }
    }
}
