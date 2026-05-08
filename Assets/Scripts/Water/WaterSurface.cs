using UnityEngine;

namespace Driftwood.Water
{
    public class WaterSurface : MonoBehaviour
    {
        [Header("Base Level")]
        [SerializeField] private float baseWaterLevel = 0f;

        [Header("Wave A")]
        [SerializeField] private float amplitudeA = 0.35f;
        [SerializeField] private float wavelengthA = 10f;
        [SerializeField] private float speedA = 1.2f;
        [SerializeField] private Vector2 directionA = new Vector2(1f, 0.35f);

        [Header("Wave B")]
        [SerializeField] private float amplitudeB = 0.2f;
        [SerializeField] private float wavelengthB = 5f;
        [SerializeField] private float speedB = 1.8f;
        [SerializeField] private Vector2 directionB = new Vector2(-0.25f, 1f);

        public float GetHeight(Vector3 worldPosition)
        {
            Vector2 p = new Vector2(worldPosition.x, worldPosition.z);
            float t = Time.time;
            float waveA = SampleWave(p, t, amplitudeA, wavelengthA, speedA, directionA);
            float waveB = SampleWave(p, t, amplitudeB, wavelengthB, speedB, directionB);
            return baseWaterLevel + waveA + waveB;
        }

        private static float SampleWave(
            Vector2 position,
            float time,
            float amplitude,
            float wavelength,
            float speed,
            Vector2 direction)
        {
            if (wavelength <= 0.01f)
            {
                return 0f;
            }

            Vector2 dir = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
            float waveNumber = (Mathf.PI * 2f) / wavelength;
            float phase = Vector2.Dot(position, dir) * waveNumber + time * speed;
            return Mathf.Sin(phase) * amplitude;
        }
    }
}
