using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Transform shake_transform;
    private float shakeDuration = 0f;
    private float maxDuration = 0f;
    private float shakeMagnitude = 0.35f;
    Vector3 initialPosition;
    Vector3 pos;
    Vector2 random_pos;

    public void TriggerShake_Damage()
    {
        shakeDuration = 1.0f;
        shakeMagnitude = 0.6f;
        maxDuration = shakeDuration;
    }

    public void TriggerShake_PlayerShoot()
    {
        shakeDuration = 0.3f;
        shakeMagnitude = 0.05f;
        maxDuration = shakeDuration;
    }

    void Start()
    {

        GameManager.Instance.PlayerDamaged.AddListener(TriggerShake_Damage);
        GameManager.Instance.PatternUsed.AddListener(TriggerShake_PlayerShoot);
        shake_transform = GetComponent(typeof(Transform)) as Transform;
        initialPosition = shake_transform.position;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {

            pos = initialPosition;
            random_pos = new Vector2(shakeMagnitude * Mathf.PerlinNoise(Time.time*100, 0.0f) * shakeDuration / maxDuration, shakeMagnitude * Mathf.PerlinNoise(0.0f, Time.time*100) * shakeDuration / maxDuration);
            pos.x = initialPosition.x + random_pos.x;
            pos.y = initialPosition.y + random_pos.y;
            shake_transform.position = pos;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            shake_transform.position = initialPosition;
        }
    }
}
