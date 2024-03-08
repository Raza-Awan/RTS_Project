using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundScript : MonoBehaviour
{
    public static CameraBoundScript instance;

    [SerializeField] private Rect _Bound;

    public Rect Bound
    {
        get
        {
            return _Bound;
        }
        set
        {
            _Bound = value;
            _UpdateBoundPositions();
        }
    }

    public Vector3 CameraClampTopLeftPosition;
    public Vector3 CameraClampBottomRightPosition;
    

    private void Awake()
    {
        instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(Bound.width, Bound.height, 0));
    }

    private void LateUpdate()
    {
        if (transform.hasChanged)
        {
            _UpdateBoundPositions();
            transform.hasChanged = false;
        }
    }

    private void _UpdateBoundPositions()
    {
        Vector3 delta = transform.TransformVector(new Vector3(-Bound.width, Bound.height, 0) / 2f);
        CameraClampTopLeftPosition = transform.position + delta;
        CameraClampBottomRightPosition = transform.position - delta;
    }

    private void OnValidate()
    {
        _UpdateBoundPositions();
    }
}
