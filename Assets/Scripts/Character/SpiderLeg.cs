using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Model.Character
{
    [RequireComponent(typeof (ChainIKConstraint))]
    public class SpiderLeg : MonoBehaviour
    {
        private const float FOOT_SPACING = 0.5f;
        private const float MOTION_SPEED = 3f;
        private const float STEP_HEIGHT = 0.5f;
        private const float MOTION_INIT_DELAY = 1f;

        #region Inspector

        public Transform motionRange;
        public bool isStartLeg = false;

        #endregion

        private Spider _owner = null;
        private ChainIKConstraint _ikConstraint = null;
        private Transform _legJointsTransform = null;

        private Vector3 _newPosition = Vector3.zero;
        private Vector3 _oldPosition = Vector3.zero;
        private Vector3 _currentFootPosition = Vector3.zero;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        private bool _isUpdatePosition = false;
        private float _lerp = 1f;

        private bool _isStartMove => _owner.MoveController.isMove && !_isUpdatePosition;

        [ContextMenu("Auto Set Motion Range Transform")]
        private void AutoSetMotionRangeTransform()
        {
            if (motionRange != null)
            {
                DestroyImmediate(motionRange.gameObject);
                motionRange = null;
            }
            GameObject go = new GameObject("motion_range");
            motionRange = go.transform;
            var ikConstraint = GetComponent<ChainIKConstraint>();
            motionRange.SetParent(this.transform);
            motionRange.position = ikConstraint.data.tip.position;
            motionRange.rotation = ikConstraint.data.tip.rotation;
            motionRange.localScale = Vector3.one;
        }

        private void Awake()
        {
            _owner = GetComponentInParent<Spider>();
            _ikConstraint = GetComponent<ChainIKConstraint>();
            _legJointsTransform = _ikConstraint.data.tip.transform.parent;
            _oldPosition = _newPosition = _ikConstraint.data.target.position;
        }

        private void Update()
        {
            if (_lerp == 1f)
            {
                if (_isStartMove && isStartLeg || Vector3.Distance(motionRange.position, _ikConstraint.data.target.position) > FOOT_SPACING)
                {
                    Vector3 targetPos = motionRange.position + _owner.MoveController.direction * (FOOT_SPACING - 0.01f);
                    if (Physics.Raycast(targetPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit))
                    {
                        _lerp = 0f;
                        _oldPosition = _newPosition;
                        _newPosition = hit.point;
                        _isUpdatePosition = true;
                        _lastUpdateTime = DateTime.Now;
                    }
                }

                if (_isUpdatePosition && _lastUpdateTime.AddSeconds(MOTION_INIT_DELAY) < DateTime.Now)
                {
                    _isUpdatePosition = false;
                    Vector3 targetPos = motionRange.position;
                    if (Physics.Raycast(targetPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit))
                    {
                        _lerp = 0f;
                        _oldPosition = _newPosition;
                        _newPosition = hit.point;
                    }
                }
            }

            if (_lerp < 1f)
            {
                // 0f ~ 1f Sin공식을 통한 발 이동 높이 애니메이션 처리
                _lerp += Time.deltaTime * MOTION_SPEED;
                _lerp = Mathf.Min(1f, _lerp);
                _currentFootPosition = Vector3.Lerp(_oldPosition, _newPosition, _lerp);
                _currentFootPosition.y = _currentFootPosition.y + Mathf.Sin(_lerp * Mathf.PI) * STEP_HEIGHT;
                _lastUpdateTime = DateTime.Now;
            }
            _ikConstraint.data.target.position = _currentFootPosition;
            _ikConstraint.data.target.rotation = Quaternion.Euler((_legJointsTransform.position - _ikConstraint.data.target.position).normalized);
            Debug.DrawRay(_ikConstraint.data.target.position, _legJointsTransform.position - _ikConstraint.data.target.position);
        }

        private void OnDrawGizmos()
        {
            if (motionRange != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(motionRange.position, FOOT_SPACING);
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_newPosition, 0.1f);
            }
        }
    }
}