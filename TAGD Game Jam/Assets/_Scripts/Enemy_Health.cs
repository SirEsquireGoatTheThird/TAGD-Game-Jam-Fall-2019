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


    private void Start()
    {
        m_healthSets = Mathf.FloorToInt(health / 5);
        m_currentTotalHealth = health;
    }

    private void ShowHealthOrbs()
    {

    }

    public void Damage(int damage)
    {
        health -= damage;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < (health))
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
    }
}
