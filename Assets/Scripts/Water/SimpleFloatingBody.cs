using UnityEngine;

namespace Driftwood.Water
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleFloatingBody : MonoBehaviour
    {
        [SerializeField] private WaterSurface waterSurface;
        [SerializeField] private float waterLevel = 0f;
        [SerializeField] private float buoyancyForce = 15f;
        [SerializeField] private float waterDrag = 2f;
        [SerializeField] private float waterAngularDrag = 1.5f;
        [SerializeField] private float maxSubmergeDepth = 1.5f;

        private Rigidbody _rigidbody;
        private float _defaultDrag;
        private float _defaultAngularDrag;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _defaultDrag = _rigidbody.linearDamping;
            _defaultAngularDrag = _rigidbody.angularDamping;
        }

        private void FixedUpdate()
        {
            float currentWaterLevel = waterSurface != null
                ? waterSurface.GetHeight(transform.position)
                : waterLevel;

            float depth = currentWaterLevel - transform.position.y;
            bool isSubmerged = depth > 0f;

            if (!isSubmerged)
            {
                _rigidbody.linearDamping = _defaultDrag;
                _rigidbody.angularDamping = _defaultAngularDrag;
                return;
            }

            float submergeFactor = Mathf.Clamp01(depth / maxSubmergeDepth);
            Vector3 uplift = Vector3.up * buoyancyForce * submergeFactor;
            _rigidbody.AddForce(uplift, ForceMode.Acceleration);

            _rigidbody.linearDamping = Mathf.Lerp(_defaultDrag, waterDrag, submergeFactor);
            _rigidbody.angularDamping = Mathf.Lerp(_defaultAngularDrag, waterAngularDrag, submergeFactor);
        }
    }
}
