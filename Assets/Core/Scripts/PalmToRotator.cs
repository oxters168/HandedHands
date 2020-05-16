using UnityEngine;
using UnityHelpers;

public class PalmToRotator : MonoBehaviour
{
    public VRPhysicsHands.HandEmulator hand;
    public Rotator rotator;
    public PhysicsGadgets.InputWheel wheelInput;

    void Update()
    {
        if (hand != null)
            rotator.spinAxis = hand.palm.forward;
        rotator.angle = wheelInput.value * 360;
    }
}
