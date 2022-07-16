using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    // Copied from https://medium.com/@hendrikk1/camera-shake-effect-in-unity-4ccf2ea527ea
    public float shake = 0;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1;
    public Vector3 startTransform;

    void Update()
    {
        if (shake > 0)
        {
            this.transform.localPosition = startTransform + Random.insideUnitSphere * shakeAmount;
            shake -= Time.unscaledDeltaTime * decreaseFactor;
            if (shake <= 0)
            {
                this.transform.localPosition = startTransform;
            }
        }
        else
        {
            startTransform = this.transform.localPosition;
            shake = 0;
        }
    }
}
