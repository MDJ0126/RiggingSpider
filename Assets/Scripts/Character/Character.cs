using UnityEngine;

namespace Model.Character
{
    public class Character : MonoBehaviour
    {
        private Transform _myTransform = null;
        public Transform MyTransform
        {
            get
            {
                if (_myTransform == null)
                    _myTransform = GetComponent<Transform>();
                return _myTransform;
            }
        }

        public Rigidbody Rigidbody { get; private set; } = null;

        public MoveController MoveController { get; private set; } = null;

        private void Awake()
        {
            this.Rigidbody = GetComponent<Rigidbody>();
            this.MoveController = GetComponent<MoveController>();
        }
    }
}