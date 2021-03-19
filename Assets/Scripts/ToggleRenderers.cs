using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRenderers : MonoBehaviour
{
    public Camera normalCamera;
    public Camera noRenderersCamera;
    public bool nodRenderersOnStart;

    public void Start()
    {
        normalCamera.enabled = !nodRenderersOnStart;
        noRenderersCamera.enabled = nodRenderersOnStart;
    }

    public void Toggle()
    {
        normalCamera.enabled = !normalCamera.enabled;
        noRenderersCamera.enabled =!noRenderersCamera.enabled;
    }

    public void ForceEnabled(bool enabled)
    {
        normalCamera.enabled = !enabled;
        noRenderersCamera.enabled = enabled;
    }
}
