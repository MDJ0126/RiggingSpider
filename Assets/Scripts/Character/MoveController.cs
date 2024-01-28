using UnityEngine;

namespace Model.Character
{
    [RequireComponent(typeof(Character))]
    public class MoveController : MonoBehaviour
    {
        #region Inspector

        public float moveSpeed = 1.5f;
        public float rotateSpeed = 150f;

        #endregion

        private Character _owner = null;

        public bool isMove = false;
        public Vector3 direction = Vector3.zero;

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
            isMove = false;
            float h = Input.GetAxis("Horizontal");
            if (h != 0f)
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, h * rotateSpeed * Time.deltaTime, 0f);
                _owner.Rigidbody.MoveRotation(_owner.Rigidbody.rotation * deltaRotation);
                isMove = true;
            }

            float v = Input.GetAxis("Vertical");
            if (v != 0f)
            {
                direction = Vector3.Normalize(_owner.MyTransform.TransformDirection(Vector3.forward) * v);
                _owner.Rigidbody.MovePosition(_owner.MyTransform.position + direction * moveSpeed * Time.deltaTime);
                _owner.Rigidbody.constraints = RigidbodyConstraints.None;
                isMove = true;
            }
            else
            {
                _owner.Rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
            _owner.Rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            Debug.DrawRay(_owner.MyTransform.position + _owner.MyTransform.up * 0.5f, -_owner.MyTransform.up, Color.yellow);
            if (Physics.Raycast(_owner.MyTransform.position + _owner.MyTransform.up * 0.5f, -_owner.MyTransform.up, out RaycastHit hit))
            {
                Debug.DrawRay(hit.point, hit.normal, Color.blue);
            }
        }
    }
}