using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    private const float LEG_TO_FOOT_LENGTH = 0.5401024f;
    private float _footSpacing = 0.7f;
    private Vector3 _footPoint = Vector3.zero;
    private Vector3 _dest = Vector3.zero;

    private void Start()
    {
        _footPoint = _dest = transform.position + transform.up.normalized * LEG_TO_FOOT_LENGTH;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.up.normalized * LEG_TO_FOOT_LENGTH, Color.yellow);
        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 10f))
        {
            _dest = hit.point;

            if (Vector3.Distance(_footPoint, _dest) > _footSpacing)
            {
                _footPoint = _dest;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_footPoint, 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_dest, 0.05f);

        Debug.DrawLine(_footPoint, _dest);
    }
}
