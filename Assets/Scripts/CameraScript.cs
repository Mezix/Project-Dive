using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera cam;
    public Transform _FPSCameraShake;
    public float FPSShakeFactor;
    private void Awake()
    {
        REF.CamScript = this;
        cam = GetComponentInChildren<Camera>();
    }
    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    public void ResetCameraShake()
    {
        StopAllCoroutines();
        _FPSCameraShake.transform.localPosition = new Vector3(0,0,0);
    }
    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            cam.transform.localPosition = new Vector3(x, y, 0);
            _FPSCameraShake.transform.localPosition = new Vector3(x, y, 0) * FPSShakeFactor;
            elapsed += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        cam.transform.localPosition = originalPos;
        _FPSCameraShake.transform.localPosition = originalPos;
    }
}
