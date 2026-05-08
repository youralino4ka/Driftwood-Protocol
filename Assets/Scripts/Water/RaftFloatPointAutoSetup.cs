using UnityEngine;

namespace Driftwood.Water
{
    [ExecuteAlways]
    [RequireComponent(typeof(RaftPlatformFloat))]
    public class RaftFloatPointAutoSetup : MonoBehaviour
    {
        [Header("Placement")]
        [SerializeField] private float insetPercent = 0.12f;
        [SerializeField] private float belowBottomOffset = 0.08f;
        [SerializeField] private bool addMiddleFrontBack = true;

        [Header("Generated Names")]
        [SerializeField] private string prefix = "float_";

        [ContextMenu("Generate Float Points From Model")]
        public void GenerateFloatPointsFromModel()
        {
            RaftPlatformFloat raftFloat = GetComponent<RaftPlatformFloat>();
            if (raftFloat == null)
            {
                Debug.LogWarning("[RaftFloatPointAutoSetup] Missing RaftPlatformFloat component.");
                return;
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning("[RaftFloatPointAutoSetup] No renderers found in children.");
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            float insetX = bounds.extents.x * insetPercent;
            float insetZ = bounds.extents.z * insetPercent;

            float left = bounds.min.x + insetX;
            float right = bounds.max.x - insetX;
            float back = bounds.min.z + insetZ;
            float front = bounds.max.z - insetZ;
            float y = bounds.min.y - belowBottomOffset;

            Transform[] points = addMiddleFrontBack
                ? new Transform[6]
                : new Transform[4];

            points[0] = CreateOrMovePoint(prefix + "fl", new Vector3(left, y, front));
            points[1] = CreateOrMovePoint(prefix + "fr", new Vector3(right, y, front));
            points[2] = CreateOrMovePoint(prefix + "bl", new Vector3(left, y, back));
            points[3] = CreateOrMovePoint(prefix + "br", new Vector3(right, y, back));

            if (addMiddleFrontBack)
            {
                float centerX = (left + right) * 0.5f;
                points[4] = CreateOrMovePoint(prefix + "fm", new Vector3(centerX, y, front));
                points[5] = CreateOrMovePoint(prefix + "bm", new Vector3(centerX, y, back));
            }

            raftFloat.ConfigureFloatPoints(points);

            Debug.Log("[RaftFloatPointAutoSetup] Float points generated and assigned.");
        }

        private Transform CreateOrMovePoint(string pointName, Vector3 worldPosition)
        {
            Transform point = transform.Find(pointName);
            if (point == null)
            {
                GameObject go = new GameObject(pointName);
                point = go.transform;
                point.SetParent(transform, true);
            }

            point.position = worldPosition;
            point.rotation = Quaternion.identity;
            point.localScale = Vector3.one;
            return point;
        }

    }
}
