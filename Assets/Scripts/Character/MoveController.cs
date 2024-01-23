using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Model.Character
{
    [RequireComponent(typeof(Character))]
    public class MoveController : MonoBehaviour
    {
        #region Inspector

        public float moveSpeed = 1f;
        public float rotateSpeed = 50f;

        #endregion

        private Character _owner = null;

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
            float h = Input.GetAxis("Horizontal");
            if (h != 0f)
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, h * rotateSpeed * Time.deltaTime, 0f);
                _owner.Rigidbody.MoveRotation(_owner.Rigidbody.rotation * deltaRotation);
            }

            float v = Input.GetAxis("Vertical");
            if (v != 0f)
            {
                Vector3 dir = _owner.MyTransform.TransformDirection(Vector3.forward) * v;
                _owner.Rigidbody.MovePosition(_owner.MyTransform.position + dir * moveSpeed * Time.deltaTime);
                _owner.Rigidbody.constraints = RigidbodyConstraints.None;
            }
            else
            {
                _owner.Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }

            //Debug.DrawRay(_owner.MyTransform.position + _owner.MyTransform.up * 0.5f, -_owner.MyTransform.up, Color.yellow);
            if (Physics.Raycast(_owner.MyTransform.position + _owner.MyTransform.up * 0.5f, -_owner.MyTransform.up, out RaycastHit hit, 0.5f))
            {
                Debug.DrawRay(hit.point, hit.normal, Color.blue);
            }
        }
    }
}