using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FollowCamera : MonoBehaviour
{
    #region Inspector

    public Transform target;

    #endregion

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

    private void Update()
    {
        if (target == null) return;
        if (!Application.isPlaying)
        {
            this.MyTransform.position = target.position;
            this.MyTransform.rotation = target.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;
        if (Application.isPlaying)
        {
            this.MyTransform.position = Vector3.Lerp(this.MyTransform.position, target.position, 0.1f);
            this.MyTransform.rotation = Quaternion.Lerp(this.MyTransform.rotation, target.rotation, 0.1f);
        }
    }
}
