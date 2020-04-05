using UnityEngine;

public class SkyboxSetter : MonoBehaviour
{
    public Material onSkybox, offSkybox;

    public void SetSkybox(bool onOff)
    {
        RenderSettings.skybox = onOff ? onSkybox : offSkybox;
    }
}
