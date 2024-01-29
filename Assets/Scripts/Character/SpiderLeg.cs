using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Model.Character
{
    [RequireComponent(typeof (ChainIKConstraint))]
    public class SpiderLeg : MonoBehaviour
    {
        private const float FOOT_SPACING = 0.5f;
        private const float MOTION_SPEED = 7f;
        private const float STEP_HEIGHT = 0.2f;
        private const float RESTORE_IDLE_TIME = 1f;

        #region Inspector

        public bool isFirstMoveLeg = false;
        public Transform motionRange;

        #endregion

        private Spider _owner = null;
        public ChainIKConstraint ikConstraint = null;

        private Vector3 _newPosition = Vector3.zero;
        private Vector3 _oldPosition = Vector3.zero;
        private Vector3 _currentFootPosition = Vector3.zero;

        private float _lerp = 0f;
        private DateTime _lastUpdateTime = DateTime.MaxValue;
        private bool IsAnimation => _lerp < 1f;
        private float _ownerMoveSpeed => _owner.MoveController.moveSpeed;
        private Vector3 _direction => _owner.MoveController.direction;
        private bool _isMoving => _owner.MoveController.isMove;
        private bool _isIdle = true;

        private void Awake()
        {
            _owner = GetComponentInParent<Spider>();
            ikConstraint = GetComponent<ChainIKConstraint>();
            _oldPosition = _newPosition = ikConstraint.data.target.position;
        }

        private void Update()
        {
            SetPosition();
            PositionAnimtion();
            FixTarget();

            // 포지션 변경 처리
            void SetPosition()
            {
                if (!IsAnimation)
                {
                    if (_isIdle && _isMoving && isFirstMoveLeg || Vector3.Distance(motionRange.position, ikConstraint.data.target.position) > FOOT_SPACING)
                    {
                        Vector3 targetPos = motionRange.position + _direction * FOOT_SPACING;
                        const float RAYCAST_DISTANCE = 10f;
                        if (Physics.Raycast(targetPos + Vector3.up * RAYCAST_DISTANCE, Vector3.down, out RaycastHit hit))
                        {
                            _lerp = 0f;
                            if (Vector3.Distance(motionRange.position, hit.point) <= FOOT_SPACING + 0.1f)
                            {
                                _oldPosition = _newPosition;
                                _newPosition = hit.point;
                            }
                            else
                            {
                                _oldPosition = _newPosition;
                                _newPosition = motionRange.position;
                            }
                            _lastUpdateTime = DateTime.Now;
                            _isIdle = false;
                        }
                    }
                }

                if (!_isIdle)
                {
                    if (_lastUpdateTime.AddSeconds(RESTORE_IDLE_TIME) < DateTime.Now)
                    {
                        Vector3 targetPos = motionRange.position;
                        const float RAYCAST_DISTANCE = 10f;
                        if (Physics.Raycast(targetPos + Vector3.up * RAYCAST_DISTANCE, Vector3.down, out RaycastHit hit))
                        {
                            _lerp = 0f;
                            _oldPosition = _newPosition;
                            _newPosition = hit.point;
                            _lastUpdateTime = DateTime.Now;
                            _isIdle = true;
                        }
                    }
                }
            }

            // 다리 위치 애니메이션
            void PositionAnimtion()
            {
                if (IsAnimation)
                {
                    _lerp += Time.deltaTime * MOTION_SPEED * _ownerMoveSpeed;
                    _lerp = Mathf.Min(1f, _lerp);
                    _currentFootPosition = Vector3.Lerp(_oldPosition, _newPosition, _lerp);
                    _currentFootPosition.y = _currentFootPosition.y + Mathf.Sin(_lerp * Mathf.PI) * STEP_HEIGHT;
                    _lastUpdateTime = DateTime.Now;
                }
            }

            void FixTarget()
            {
                ikConstraint.data.target.position = _currentFootPosition;
            }
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

        #region EDITOR FUNCTION

#if UNITY_EDITOR
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
#endif

        #endregion
    }
}