using UnityEngine;

public class DisplayAngle : MonoBehaviour
{
    public TMPro.TextMeshProUGUI label;
    public PhysicsGadgets.InputWheel inputWheel;

    void Update()
    {
        label.text = "Angle\n" + Mathf.RoundToInt(inputWheel.value * 360);
    }
}
