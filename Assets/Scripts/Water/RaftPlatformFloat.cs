using UnityEngine;

namespace Driftwood.Water
{
    [RequireComponent(typeof(Rigidbody))]
    public class RaftPlatformFloat : MonoBehaviour
    {
        [SerializeField] private WaterSurface waterSurface;
        [SerializeField] private Transform[] floatPoints;
        [SerializeField] private float buoyancyForce = 20f;
        [SerializeField] private float maxSubmergeDepth = 1.5f;
        [SerializeField] private float waterDrag = 1.5f;
        [SerializeField] private float waterAngularDrag = 1.2f;
        [Header("Simple Stable Mode")]
        [SerializeField] private bool useSimpleStableMode = true;
        [SerializeField] private float targetFloatOffset = 0.15f;
        [SerializeField] private float simpleLiftForce = 14f;
        [SerializeField] private float simpleVerticalDamping = 3f;
        [Header("Stability")]
        [SerializeField] private bool keepNearStartPosition = true;
        [SerializeField] private float planarRestoreForce = 1.75f;
        [SerializeField] private float planarVelocityDamping = 0.35f;
        [SerializeField] private bool stabilizeVerticalMotion = true;
        [SerializeField] private float verticalRestoreForce = 1.25f;
        [SerializeField] private float verticalVelocityDamping = 0.6f;
        [SerializeField] private float maxVerticalSpeed = 1.5f;
        [Header("Safety")]
        [SerializeField] private bool enableSafetyClamp = true;
        [SerializeField] private float maxPlanarDistanceFromStart = 10f;
        [SerializeField] private float maxVerticalDistanceFromStart = 3f;

        private Rigidbody _rigidbody;
        private float _defaultDrag;
        private float _defaultAngularDrag;
        private Vector3 _startPosition;

        public void ConfigureFloatPoints(Transform[] points)
        {
            floatPoints = points;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _defaultDrag = _rigidbody.linearDamping;
            _defaultAngularDrag = _rigidbody.angularDamping;
            _startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (waterSurface == null || floatPoints == null || floatPoints.Length == 0)
            {
                return;
            }

            if (useSimpleStableMode)
            {
                ApplySimpleStableMode();
                ApplySafetyClamp();
                return;
            }

            float submergedCount = 0f;

            foreach (Transform point in floatPoints)
            {
                if (point == null)
                {
                    continue;
                }

                Vector3 pointPosition = point.position;
                float waterHeight = waterSurface.GetHeight(pointPosition);
                float depth = waterHeight - pointPosition.y;

                if (depth <= 0f)
                {
                    continue;
                }

                float submergeFactor = Mathf.Clamp01(depth / maxSubmergeDepth);
                Vector3 force = Vector3.up * buoyancyForce * submergeFactor;
                _rigidbody.AddForceAtPosition(force, pointPosition, ForceMode.Acceleration);
                submergedCount += submergeFactor;
            }

            if (submergedCount > 0f)
            {
                float normalized = Mathf.Clamp01(submergedCount / floatPoints.Length);
                _rigidbody.linearDamping = Mathf.Lerp(_defaultDrag, waterDrag, normalized);
                _rigidbody.angularDamping = Mathf.Lerp(_defaultAngularDrag, waterAngularDrag, normalized);
                ApplyPlanarStability(normalized);
                ApplyVerticalStability(normalized);
                ClampVerticalSpeed();
            }
            else
            {
                _rigidbody.linearDamping = _defaultDrag;
                _rigidbody.angularDamping = _defaultAngularDrag;
            }

            ApplySafetyClamp();
        }

        private void ApplySimpleStableMode()
        {
            float waterY = waterSurface.GetHeight(transform.position);
            float targetY = waterY + targetFloatOffset;
            float currentY = transform.position.y;
            float verticalOffset = targetY - currentY;

            float verticalVelocity = _rigidbody.linearVelocity.y;
            float liftAccel = (verticalOffset * simpleLiftForce) - (verticalVelocity * simpleVerticalDamping);
            _rigidbody.AddForce(Vector3.up * liftAccel, ForceMode.Acceleration);

            float normalized = Mathf.Clamp01(Mathf.Abs(verticalOffset) / Mathf.Max(0.01f, maxSubmergeDepth));
            _rigidbody.linearDamping = Mathf.Lerp(_defaultDrag, waterDrag, normalized);
            _rigidbody.angularDamping = Mathf.Lerp(_defaultAngularDrag, waterAngularDrag, normalized);

            ApplyPlanarStability(1f);
            ClampVerticalSpeed();
        }

        private void ApplyPlanarStability(float normalizedSubmerge)
        {
            if (!keepNearStartPosition)
            {
                return;
            }

            Vector3 planarOffset = transform.position - _startPosition;
            planarOffset.y = 0f;

            Vector3 restoreAcceleration = -planarOffset * (planarRestoreForce * normalizedSubmerge);
            _rigidbody.AddForce(restoreAcceleration, ForceMode.Acceleration);

            Vector3 planarVelocity = _rigidbody.linearVelocity;
            planarVelocity.y = 0f;
            _rigidbody.AddForce(-planarVelocity * planarVelocityDamping, ForceMode.Acceleration);
        }

        private void ApplyVerticalStability(float normalizedSubmerge)
        {
            if (!stabilizeVerticalMotion || waterSurface == null || floatPoints == null || floatPoints.Length == 0)
            {
                return;
            }

            float avgWaterY = 0f;
            float avgPointY = 0f;
            int count = 0;

            foreach (Transform point in floatPoints)
            {
                if (point == null)
                {
                    continue;
                }

                Vector3 p = point.position;
                avgWaterY += waterSurface.GetHeight(p);
                avgPointY += p.y;
                count++;
            }

            if (count == 0)
            {
                return;
            }

            avgWaterY /= count;
            avgPointY /= count;

            float verticalOffset = avgPointY - avgWaterY;
            float verticalVelocity = _rigidbody.linearVelocity.y;

            float restoreAccel = -verticalOffset * (verticalRestoreForce * normalizedSubmerge);
            float dampAccel = -verticalVelocity * verticalVelocityDamping;
            _rigidbody.AddForce(Vector3.up * (restoreAccel + dampAccel), ForceMode.Acceleration);
        }

        private void ClampVerticalSpeed()
        {
            if (maxVerticalSpeed <= 0f)
            {
                return;
            }

            Vector3 v = _rigidbody.linearVelocity;
            v.y = Mathf.Clamp(v.y, -maxVerticalSpeed, maxVerticalSpeed);
            _rigidbody.linearVelocity = v;
        }

        private void ApplySafetyClamp()
        {
            if (!enableSafetyClamp)
            {
                return;
            }

            Vector3 pos = transform.position;
            Vector3 planarDelta = pos - _startPosition;
            planarDelta.y = 0f;
            bool planarTooFar = planarDelta.magnitude > maxPlanarDistanceFromStart;
            bool verticalTooFar = Mathf.Abs(pos.y - _startPosition.y) > maxVerticalDistanceFromStart;

            if (!planarTooFar && !verticalTooFar)
            {
                return;
            }

            Vector3 target = _startPosition;
            if (!verticalTooFar)
            {
                target.y = pos.y;
            }

            transform.position = target;
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            if (floatPoints == null)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            foreach (Transform point in floatPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.08f);
                }
            }
        }
    }
}
