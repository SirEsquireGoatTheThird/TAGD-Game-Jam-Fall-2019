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
    private int m_currentHealthSet;
    private int m_currentTotalHealth;
    private int m_currentHealthInSet;


    private void Start()
    {
        
        m_currentTotalHealth = health;
        m_healthSets = Mathf.FloorToInt(m_currentTotalHealth / 5);
        m_healthSets--;
        ShowHealthOrbs();
    }

    private void ShowHealthOrbs()
    {
        for (int i = 0; i < healthOrbs.Length; i++)
        {
            healthOrbs[i].enabled = false;
        }
        for (int bulletNum = 0; bulletNum < m_healthSets; bulletNum++)
        {
            healthOrbs[bulletNum].enabled = true;
        }
    }

    public void Damage(int damage)
    {
        m_currentTotalHealth -= damage;
        m_currentHealthInSet = m_currentTotalHealth % 5;
        m_healthSets = Mathf.FloorToInt(m_currentTotalHealth / 5);
        if(m_currentHealthInSet == 0)
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

    public void UpdateOrbValues()
    {

    }
}
