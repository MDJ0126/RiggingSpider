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

        public bool isMove => this.Direction != Vector3.zero;
        public Vector3 Direction { get; private set; } = Vector3.zero;

        private void Awake()
        {
            _owner = GetComponent<Character>();
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
            this.Direction = Vector3.Normalize(this.Direction);
            _owner.Rigidbody.MovePosition(this.transform.position + this.Direction * moveSpeed * Time.deltaTime);
        }
    }
}