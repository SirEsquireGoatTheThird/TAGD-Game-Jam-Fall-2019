using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float duration;
    public Image fillImage;
    private PlayGrid m_grid;
    float time;

    public void Start()
    {
        fillImage.fillAmount = 1f;
        m_grid = FindObjectOfType<PlayGrid>();
        duration = m_grid.duration;
        GameManager.Instance.UpdateTimeDuration.AddListener(UpdateTimer);
        time = duration;
    }

    public void Update()
    {

        time -= Time.deltaTime;

        if(time > 0)
        {
            float value = time / duration;
            fillImage.fillAmount = value;
        }
        else
        {
            time = duration;
        }

    }


    private void UpdateTimer()
    {
        time = Mathf.Clamp(time - 5, 0, duration);
    }
}
