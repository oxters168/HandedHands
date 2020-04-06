using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public PhysicsGadgets.Joystick joystick;
    public Transform objectToMove;
    public float speedMultiplier = 0.01f;

    private void FixedUpdate()
    {
        objectToMove.position += new Vector3(joystick.horizontal, 0, joystick.vertical) * speedMultiplier;
    }
}
