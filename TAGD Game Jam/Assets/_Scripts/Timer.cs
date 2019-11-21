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
    bool enemyDead = false;

    public void Start()
    {
        fillImage.fillAmount = 1f;
        m_grid = FindObjectOfType<PlayGrid>();
        duration = m_grid.duration;
        GameManager.Instance.UpdateTimeDuration.AddListener(UpdateTimer);
        time = duration;
        GameManager.Instance.NextEnemy.AddListener(ResetTimer);
        GameManager.Instance.EnemyDied.AddListener(EnemyDied);
        GameManager.Instance.EnemyAlive.AddListener(EnemyAlive);
    }

    public void Update()
    {
        if(enemyDead)
        {
            return;
        }
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

    private void ResetTimer()
    {
        duration = m_grid.duration;
        time = duration;
    }

    private void EnemyDied()
    {
        enemyDead = true;
    }
    private void EnemyAlive()
    {
        enemyDead = false;
    }
}
