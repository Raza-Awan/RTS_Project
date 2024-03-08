using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private float _shakeAmount = 5.0f;
    private float _shakeDuration = 1.0f;
    private bool _isShaking = false;

    private Vector3 _originalPos;

    public void ShakeCamera()
    {
        ShakeCamera(0.5f, 0.5f);
    }

    public void ShakeCamera(float amount, float duration)
    {
        _originalPos = transform.localPosition;
        _shakeAmount = amount;
        _shakeDuration = duration;

        if (!_isShaking)
            StartCoroutine(_Shake());
    }


    private IEnumerator _Shake()
    {
        _isShaking = true;
        while (_shakeDuration > 0.01f)
        {
            this.transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
            _shakeDuration -= Time.deltaTime;
            yield return null;
        }

        _shakeDuration = 0f;
        this.transform.localPosition = _originalPos;

        _isShaking = false;
    }
}
