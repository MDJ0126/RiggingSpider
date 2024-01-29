using UnityEngine;

namespace Model.Character
{
    [RequireComponent(typeof(Character))]
    public class MoveController : MonoBehaviour
    {
        #region Inspector

        public float moveSpeed = 2f;
        public float rotateSpeed = 150f;

        #endregion

        private Character _owner = null;

        public bool isMove => direction != Vector3.zero;
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
            direction = Vector3.zero;

            float v = Input.GetAxis("Vertical");
            if (v != 0f)
            {
                direction += Vector3.Normalize(_owner.MyTransform.forward * v);
            }

            float h = Input.GetAxis("Horizontal");
            if (h != 0f)
            {
                direction += Vector3.Normalize(_owner.MyTransform.right * h);
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
                Quaternion deltaRotation = Quaternion.Euler(0f, e * rotateSpeed * Time.deltaTime, 0f);
                _owner.Rigidbody.MoveRotation(_owner.Rigidbody.rotation * deltaRotation);
            }

            if (direction != Vector3.zero)
            {
                direction = Vector3.Normalize(direction);
                _owner.Rigidbody.MovePosition(_owner.MyTransform.position + direction * moveSpeed * Time.deltaTime);
            }
        }
    }
}