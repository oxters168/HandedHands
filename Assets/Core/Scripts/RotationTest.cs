using UnityEngine;
using UnityHelpers;

public class RotationTest : MonoBehaviour
{
    public Transform anchor;

    public bool localToWorld;
    public bool worldToLocal;

    public Vector3 eulerRotation;

    void Update()
    {
        if (localToWorld)
            transform.rotation = anchor.TransformRotation(Quaternion.Euler(eulerRotation));
        else if (worldToLocal)
            transform.localRotation = anchor.InverseTransformRotation(Quaternion.Euler(eulerRotation));
    }
}
