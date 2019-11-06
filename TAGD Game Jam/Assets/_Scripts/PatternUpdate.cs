using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PatternUpdate : MonoBehaviour
{
    PlayGrid m_grid;
    [SerializeField]
    Image[] patternImages = new Image[3];
    GameObject[] imageChildren = new GameObject[3];

    private void Awake()
    {
        GameManager.Instance.UpdatePatternUI.AddListener(UpdatePattern);
        m_grid = FindObjectOfType<PlayGrid>();
        patternImages = GetComponentsInChildren<Image>();
    }

    private void UpdatePattern()
    {
        for(int i = 0; i < m_grid.CurrentPatternSet.patternSet.Length; i++)
        {
            patternImages[i].sprite = m_grid.CurrentPatternSet.patternSet[i].icon;
        }
    }


}
