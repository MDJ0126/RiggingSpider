using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Model.Character
{
    public class SpiderLeg : MonoBehaviour
    {
        private const float FOOT_SPACING = 0.35f;

        private ChainIKConstraint _ikConstraint = null;
        private Vector3 _footPosition = Vector3.zero;
        private Vector3 _footRotaion = Vector3.zero;

        private void Awake()
        {
            _ikConstraint = GetComponent<ChainIKConstraint>();
            _footPosition = _ikConstraint.data.target.position;
            _footRotaion = _ikConstraint.data.target.rotation.eulerAngles;
        }

        private void Update()
        {
            _ikConstraint.data.target.position = _footPosition;
            _ikConstraint.data.target.rotation = Quaternion.Euler(_footRotaion);
        }

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawSphere(_footPoint, 0.05f);S

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_footPosition, FOOT_SPACING);

            //Debug.DrawLine(_footPoint, _footDest);
        }
    }
}