using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Health : MonoBehaviour
{
    public int health;
    public int numOfHearts;

    public Image[] hearts;
    public Image[] healthOrbs;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private int m_healthSets;
    private int m_currentTotalHealth;
    private int m_currentHealthInSet;
    [SerializeField]
    private Sprite m_brokenOrb;
    [SerializeField]
    private Sprite m_healthOrb;
    private int m_maxOrbs;


    private void Start()
    {
        m_currentTotalHealth = health;
        m_healthSets = Mathf.FloorToInt(m_currentTotalHealth / 5);
        m_maxOrbs = Mathf.FloorToInt(m_currentTotalHealth / 5) - 1;
        m_healthSets--;
        ShowInitialLOrbs();
    }

    private void ShowHealthOrbs()
    {
        for (int bulletNum = 0; bulletNum < m_healthSets; bulletNum++)
        {
            healthOrbs[bulletNum].enabled = true;
        }
        for (int bulletNum = 0; bulletNum < m_maxOrbs; bulletNum++)
        {
            if (bulletNum <= m_healthSets - 1)
            {
                healthOrbs[bulletNum].sprite = m_healthOrb;
            }
            else
            {
                healthOrbs[bulletNum].sprite = m_brokenOrb;
            }
        }
    }

    private void ShowInitialLOrbs()
    {
        for (int i = 0; i < healthOrbs.Length; i++)
        {
            healthOrbs[i].enabled = false;
        }
        for (int bulletNum = 0; bulletNum < m_maxOrbs; bulletNum++)
        {
            healthOrbs[bulletNum].enabled = true;
        }
    }

    public void Damage(int damage)
    {
        m_currentTotalHealth -= damage;
        m_currentHealthInSet = m_currentTotalHealth % 5;
        m_healthSets = Mathf.FloorToInt(m_currentTotalHealth / 5);
        if(m_currentHealthInSet == 0 && m_currentTotalHealth > 0)
        {
            m_currentHealthInSet = 5;
            m_healthSets--;
        }


        for (int i = 0; i < 5; i++)
        {
            if (i < (m_currentHealthInSet))
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

        ShowHealthOrbs();
    }

    public void UpdateOrbValues(int health)
    {
        m_currentTotalHealth = health;
        m_healthSets = Mathf.FloorToInt(m_currentTotalHealth / 5);
        m_healthSets--;
        m_maxOrbs = Mathf.FloorToInt(m_currentTotalHealth / 5) - 1;
        ShowInitialLOrbs();
        ShowHealthOrbs();
        for(int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = fullHeart;
        }
        for(int i = 0; i < m_maxOrbs; i++)
        {
            healthOrbs[i].sprite = m_healthOrb;
        }
    }
}
