using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Model.Character
{
    public class SpiderLeg : MonoBehaviour
    {
        private const float TARGET_TO_GROUND_LENGHT = 0.4784243f;
        private const float LEG_TO_FOOT_LENGTH = 0.5401024f;
        private float _footSpacing = 0.7f;
        private Vector3 _footPoint = Vector3.zero;
        private Vector3 _footDest = Vector3.zero;
        private Vector3 _targetDest = Vector3.zero;
        private TwoBoneIKConstraint _twoBoneIKConstraint = null;

        private void Awake()
        {
            _twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();
        }

        private void Start()
        {
            _footPoint = _footDest = _twoBoneIKConstraint.data.target.position + _twoBoneIKConstraint.data.target.up.normalized * LEG_TO_FOOT_LENGTH;
            _targetDest = _twoBoneIKConstraint.data.target.position;
        }

        private void Update()
        {
            bool isMove = false;

            if (Physics.Raycast(_twoBoneIKConstraint.data.tip.position, _twoBoneIKConstraint.data.tip.up, out RaycastHit hit, 10f))
            {
                _footDest = hit.point;

                if (Vector3.Distance(_footPoint, _footDest) > _footSpacing)
                {
                    _footPoint = _footDest + (_footDest - _footPoint).normalized * _footSpacing * 0.3f;
                    isMove = true;
                }
            }

            if (Physics.Raycast(_footPoint, Vector3.down, out RaycastHit hit2, 10f))
            {
                Debug.DrawRay(_footPoint, hit2.normal * TARGET_TO_GROUND_LENGHT);
                if (isMove)
                {
                    _targetDest = _footPoint + hit2.normal * TARGET_TO_GROUND_LENGHT;
                    _twoBoneIKConstraint.data.target.rotation = Quaternion.Euler(_twoBoneIKConstraint.data.target.up);
                }
            }
            //_twoBoneIKConstraint.data.target.position = _targetDest;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_footPoint, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_footDest, 0.05f);

            Debug.DrawLine(_footPoint, _footDest);
        }
    }
}