using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float duration;
    public Image fillImage;
    private PlayGrid m_grid;
    bool timeStarted = false;

    public void Start()
    {
        fillImage.fillAmount = 1f;
        m_grid = FindObjectOfType<PlayGrid>();
        duration = m_grid.duration;
        timeStarted = false;
    }

    public void Update()
    {
        if(!timeStarted)
        {
            StartCoroutine(timer(duration));
            timeStarted = true;
        }
    }

    public IEnumerator timer(float duration)
    {
        float startTime = Time.time;
        float time = duration;
        float value = 0;

        while ((Time.time - startTime) < duration)
        {
            time -= Time.deltaTime;
            value = time / duration;
            fillImage.fillAmount = value;
            yield return null;
        }

        timeStarted = false;
        // Event happens here(basically send out unity event of attack happening then have a listener listen for it,
        // such as the enemy)

    }
}
