using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Model.Character
{
    [RequireComponent(typeof (TwoBoneIKConstraint))]
    public class SpiderLeg : MonoBehaviour
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

        [Header("Motion")]
        public Transform motionRange;
        public float footSpacing = 0.5f;
        public float motionSpeed = 7f;
        public float stepHeight = 0.2f;
        public float restoreIdleTime = 1f;

        [Header("Raycast")]
        public RaycastGroup innerRacastGroup;
        public RaycastGroup outerRacastGroup;

        #endregion

        private Spider _onwer = null;
        public TwoBoneIKConstraint ikConstraint { get; private set; } = null;

        private Vector3 _newPosition = Vector3.zero;
        private Vector3 _oldPosition = Vector3.zero;
        private Vector3 _nextNewPosition = Vector3.zero;
        public Vector3 CurrentFootPosition { get; private set; } = Vector3.zero;

        private float _lerp = 0f;

        private void Awake()
        {
            _onwer = GetComponentInParent<Spider>();
            ikConstraint = GetComponent<TwoBoneIKConstraint>();
            _newPosition = _oldPosition = this.CurrentFootPosition = ikConstraint.data.target.position;
            innerRacastGroup.isInner = true;
            outerRacastGroup.isInner = false;
        }

        private void FixedUpdate()
        {
            bool isDone = false;
            UpdateRaycast(outerRacastGroup, Color.red);
            UpdateRaycast(innerRacastGroup, Color.white);
            UpdateLegAnimation();

            void UpdateRaycast(RaycastGroup raycastGroup, Color color)
            {
                // 1. 다음으로 이동해야할 예측 위치값 가져오기
                bool isMove = false;
                if (Vector3.Distance(motionRange.position, _newPosition) > footSpacing)
                {
                    _nextNewPosition = motionRange.position + _onwer.MoveController.Direction * footSpacing;
                    Debug.Log(_onwer.MoveController.Direction);
                    isMove = true;
                }
                //if (!isMove) return;

                // 2. 다음 예측 위치값과 레이케스팅으로 제일 가장 가까운 거리 검사
                float minDistance = float.MaxValue;
                Vector3 newPosition = Vector3.zero;
                Vector3 startHeight = motionRange.position + motionRange.up * raycastGroup.height;
                for (int i = 0; i < raycastGroup.count; i++)
                {
                    Quaternion quaternion = Quaternion.Euler(0f, (float)(360f / raycastGroup.count * i), 0f);
                    Vector3 circleOutlinePoint = motionRange.position + motionRange.rotation * (quaternion * motionRange.position.normalized * raycastGroup.radius);
                    //Debug.DrawRay(motionRange.position, quaternion * motionRange.position.normalized * raycastGroup.radius, color);

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

                    if (isMove && !isDone)
                    {
                        if (Physics.Raycast(start, dir, out RaycastHit hit, raycastGroup.raycastLength))
                        {
                            float distance = Vector3.Distance(_nextNewPosition, hit.point);
                            if (minDistance > distance)
                            {
                                minDistance = distance;
                                newPosition = hit.point;
                            }
                        }
                    }
                }

                // 3. 위치 지정하여 실제 지점 변경
                if (newPosition != Vector3.zero)
                {
                    _lerp = 0f;
                    _oldPosition = _newPosition;
                    Vector3 direction = (newPosition - motionRange.position).normalized;
                    _newPosition = motionRange.position + direction * footSpacing;
                    isDone = true;
                }
            }

            // 이동 애니메이션 구간
            void UpdateLegAnimation()
            {
                if (_lerp < 1f)
                {
                    _lerp += Time.deltaTime * motionSpeed;
                    _lerp = Math.Min(_lerp, 1f);

                    Vector3 current = Vector3.Lerp(_oldPosition, _newPosition, _lerp);
                    current.y += (float)Math.Sin(Math.PI * _lerp) * stepHeight;
                    this.CurrentFootPosition = current;
                }
                ikConstraint.data.target.position = this.CurrentFootPosition;
            }
        }

        private void OnDrawGizmos()
        {
            if (motionRange != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(motionRange.position, footSpacing);

                if (innerRacastGroup != null)
                {
                    Vector3 startHeight = motionRange.position + motionRange.up * innerRacastGroup.height;
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(startHeight, 0.05f);
                }

                if (outerRacastGroup != null)
                {
                    Vector3 startHeight = motionRange.position + motionRange.up * outerRacastGroup.height;
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(startHeight, 0.05f);
                }
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_newPosition, 0.05f);

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(_nextNewPosition, 0.05f);
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

            var ikConstraint = GetComponent<TwoBoneIKConstraint>();
            motionRange.SetParent(this.transform);
            motionRange.position = ikConstraint.data.tip.position;
            motionRange.rotation = Quaternion.Euler(Vector3.zero);
            motionRange.localScale = Vector3.one;
        }
#endif
        #endregion
    }
}