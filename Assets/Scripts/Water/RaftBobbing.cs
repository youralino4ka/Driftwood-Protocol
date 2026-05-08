using UnityEngine;

namespace Driftwood.Water
{
    public class RaftBobbing : MonoBehaviour
    {
        [Header("Vertical Bob")]
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float bobFrequency = 0.35f;

        [Header("Tilt")]
        [SerializeField] private float tiltAngle = 3f;
        [SerializeField] private float tiltFrequencyX = 0.22f;
        [SerializeField] private float tiltFrequencyZ = 0.29f;

        [Header("Optional Smoothing")]
        [SerializeField] private bool smoothMotion = true;
        [SerializeField] private float smoothSpeed = 4f;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private float _seedX;
        private float _seedZ;

        private void Start()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _seedX = Random.Range(0f, 100f);
            _seedZ = Random.Range(0f, 100f);
        }

        private void Update()
        {
            float t = Time.time;

            float yOffset = Mathf.Sin((t + _seedX) * bobFrequency * Mathf.PI * 2f) * bobHeight;
            float xTilt = Mathf.Sin((t + _seedX) * tiltFrequencyX * Mathf.PI * 2f) * tiltAngle;
            float zTilt = Mathf.Sin((t + _seedZ) * tiltFrequencyZ * Mathf.PI * 2f) * tiltAngle;

            Vector3 targetPos = _startPosition + new Vector3(0f, yOffset, 0f);
            Quaternion targetRot = _startRotation * Quaternion.Euler(xTilt, 0f, zTilt);

            if (!smoothMotion)
            {
                transform.position = targetPos;
                transform.rotation = targetRot;
                return;
            }

            float lerp = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, lerp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, lerp);
        }
    }
}
