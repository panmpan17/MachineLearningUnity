using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float timeScale;

    public float targetFPS;

    public float timeScaleCap;

    public float recalculateTime;
    private float recalculateTimer;

    private float fixedDeltaTime;

    private void Start() {
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        recalculateTimer += Time.unscaledDeltaTime;

        if (recalculateTimer >= recalculateTime)
        {
            recalculateTimer = 0;

            int FPS = Mathf.RoundToInt(1 / TimeControl.deltaTime);

            timeScale = FPS / targetFPS;
            if (timeScale > timeScaleCap) timeScale = timeScaleCap;
            else if (timeScale < 0.1f) timeScale = 0.1f;

            Time.timeScale = timeScale;
            Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        }
    }
}
