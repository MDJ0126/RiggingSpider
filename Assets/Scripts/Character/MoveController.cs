using System;
using UnityEngine;

namespace Model.Character
{
    [RequireComponent(typeof(Character))]
    public class MoveController : MonoBehaviour
    {
        [Serializable]
        public class RaycastGroup
        {
            [HideInInspector] public bool isInner = false;
            public float radius = 0.2f;
            public float height = 0.5f;
            public int count = 5;
            public float raycastLength = 1f;
        }

        #region Inspector

        public float moveSpeed = 2f;
        public float rotationSpeed = 150f;

        [Header("Raycast")]
        public RaycastGroup innerRacastGroup;
        public RaycastGroup outerRacastGroup;

        #endregion

        private Character _owner = null;
        private SphereCollider _sphereCollider;

        public bool isMove => this.Direction != Vector3.zero;
        public Vector3 Direction { get; private set; } = Vector3.zero;

        private void Awake()
        {
            _owner = GetComponent<Character>();
            _sphereCollider = GetComponent<SphereCollider>();
        }

        private void FixedUpdate()
        {
            UpdateMove();
        }

        private void UpdateMove()
        {
            this.Direction = Vector3.zero;

            float v = Input.GetAxis("Vertical");
            if (v != 0f)
            {
                this.Direction += Vector3.Normalize(_owner.MyTransform.forward * v);
            }

            float h = Input.GetAxis("Horizontal");
            if (h != 0f)
            {
                this.Direction += Vector3.Normalize(_owner.MyTransform.right * h);
            }

            float e = 0f;
            if (Input.GetKey(KeyCode.E))
            {
                e = 1f;
            }
            else if (Input.GetKey(KeyCode.Q))
            { 
                e = -1f;
            }

            if (e != 0f)
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, e * rotationSpeed * Time.deltaTime, 0f);
                _owner.Rigidbody.MoveRotation(_owner.Rigidbody.rotation * deltaRotation);
            }

            if (this.Direction != Vector3.zero)
            {
                _owner.Rigidbody.constraints = RigidbodyConstraints.None;
            }
            else
            {
                _owner.Rigidbody.constraints = RigidbodyConstraints.FreezePosition;
            }


            int raycastHItCount = 0;
            Vector3 raycastAveragePoint = Vector3.zero;
            UpdateRaycast(outerRacastGroup, Color.red);
            UpdateRaycast(innerRacastGroup, Color.white);
            raycastAveragePoint /= raycastHItCount;

            this.Direction = Vector3.Normalize(this.Direction);
            _owner.Rigidbody.MovePosition(raycastAveragePoint + this.Direction * moveSpeed * Time.deltaTime);

            //// 법선 벡터 구하기
            //Vector3 normal = Vector3.Normalize(raycastAveragePoint);

            //// 현재의 "위쪽" 벡터를 법선 벡터와 일치시키는 회전 계산
            //Quaternion toRotation = Quaternion.FromToRotation(transform.up, normal);

            //// 부드럽게 회전 적용
            //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation * transform.rotation, Time.deltaTime * rotationSpeed);

            void UpdateRaycast(RaycastGroup raycastGroup, Color color)
            {
                Vector3 startHeight = this.transform.position + this.transform.up * raycastGroup.height;
                for (int i = 0; i < raycastGroup.count; i++)
                {
                    Quaternion quaternion = Quaternion.Euler(0f, (float)(360f / raycastGroup.count * i), 0f);
                    Vector3 circleOutlinePoint = this.transform.position + this.transform.rotation * (quaternion * this.transform.position.normalized * raycastGroup.radius);
                    //Debug.DrawRay(startHeight, (circleOutlinePoint - startHeight).normalized * 10f, color);

                    Vector3 start, dir;
                    if (raycastGroup.isInner)
                    {
                        start = circleOutlinePoint + (circleOutlinePoint - startHeight).normalized * 0.5f;
                        dir = (startHeight - circleOutlinePoint).normalized * raycastGroup.raycastLength;
                    }
                    else
                    {
                        start = circleOutlinePoint + (startHeight - circleOutlinePoint).normalized * 0.5f;
                        dir = (circleOutlinePoint - startHeight).normalized * raycastGroup.raycastLength;
                    }
                    Debug.DrawRay(start, dir, color);

                    if (Physics.Raycast(start, dir, out RaycastHit hit, raycastGroup.raycastLength))
                    {
                        ++raycastHItCount;
                        raycastAveragePoint += hit.point;
                    }
                }
            }
        }
    }
}