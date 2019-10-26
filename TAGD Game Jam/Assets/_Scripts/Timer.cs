using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float duration;
    public Image fillImage;

    public void Update()
    {
        if ((int)Time.time % duration == 0)
        {
            fillImage.fillAmount = 1f;
            StartCoroutine(timer(duration));
        }
        Debug.Log(Time.time);
    }

    public IEnumerator timer(float duration)
    {
        float startTime = Time.time;
        float time = duration;
        float value = 0;

        while (Time.time - startTime < duration)
        {
            time -= Time.deltaTime;
            value = time / duration;
            fillImage.fillAmount = value;
            yield return null;
        }
    }
}
