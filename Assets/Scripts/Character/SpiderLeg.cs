using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Model.Character
{
    public class SpiderLeg : MonoBehaviour
    {
        private TwoBoneIKConstraint _twoBoneIKConstraint = null;
        private Transform _originalTarget;
        private Vector3 _dest = Vector3.zero;

        private void Awake()
        {
            _twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();
            if (_twoBoneIKConstraint != null)
            {
                _originalTarget = _twoBoneIKConstraint.data.target;
                _dest = _originalTarget.position;
                _twoBoneIKConstraint.data.target = Instantiate(_twoBoneIKConstraint.data.target);
                this.GetComponent<RigBuilder>()?.Build();
            }
        }

        // 기즈모를 씬 뷰에 표시하는 메서드
        void OnDrawGizmos()
        {
        }

        // 충돌 지점에 기즈모를 그리는 함수
        void DrawGizmoAtHitPoint(Vector3 point, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(point, 0.05f);
        }
    }
}