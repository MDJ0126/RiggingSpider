using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(TwoBoneIKConstraint))]
public class IKFootRotateFixer : MonoBehaviour
{
    private TwoBoneIKConstraint _twoBoneIKConstraint = null;

    private void Awake()
    {
        _twoBoneIKConstraint = GetComponent<TwoBoneIKConstraint>();
    }

    private void FixedUpdate()
    {
        if (_twoBoneIKConstraint != null)
        {
            _twoBoneIKConstraint.data.target.rotation = _twoBoneIKConstraint.data.mid.rotation;
        }
    }
}
