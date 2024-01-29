using UnityEngine;

namespace Model.Character
{
    public class Spider : Character
    {
        public SpiderLeg[] legs;

        private SphereCollider _sphereCollider;

        [ContextMenu("Get 'SpiderLeg' Components")]
        private void GetSpiderLegComponents()
        {
            legs = this.GetComponentsInChildren<SpiderLeg>(true);
        }

        private void Start()
        {
            _sphereCollider = GetComponent<SphereCollider>();
        }

        private Vector3 position = Vector3.zero;
        private void FixedUpdate()
        {
            // 각 다리의 위치 확인
            position = Vector3.zero;
            for (int i = 0; i < legs.Length; i++)
            {
                position += legs[i].ikConstraint?.data.target.position ?? Vector3.zero;
            }
            position /= legs.Length;

            if (Physics.Raycast(position, -this.transform.up, out var hit))
            {
                Debug.DrawRay(hit.point, hit.normal * 10f, Color.red);
                this.transform.rotation = Quaternion.FromToRotation(this.transform.up, hit.normal) * this.transform.rotation;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(position, 0.1f);
        }
    }
}