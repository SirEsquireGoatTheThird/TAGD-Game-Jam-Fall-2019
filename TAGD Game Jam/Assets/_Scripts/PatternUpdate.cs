using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PatternUpdate : MonoBehaviour
{
    PlayGrid m_grid;
    Image[] temp = new Image[6];
    Image[] lockImages = new Image[3];
    Image[] patternImages = new Image[3];
    List<Image> patternImagesTemp = new List<Image>();
    List<Image> lockImagesTemp = new List<Image>();
    

    private void Awake()
    {
        m_grid = FindObjectOfType<PlayGrid>();
        temp = GetComponentsInChildren<Image>();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].transform.parent == transform)
            {
                patternImagesTemp.Add(temp[i]);
            }
            else
            {
                lockImagesTemp.Add(temp[i]);
            }
        }
        lockImages = lockImagesTemp.ToArray();
        patternImages = patternImagesTemp.ToArray();
        GameManager.Instance.UpdatePatternUI.AddListener(UpdatePattern);
        GameManager.Instance.PatternUsed.AddListener(LockPattern);
        GameManager.Instance.UnlockPattern.AddListener(UnLockPattern);
    }

    private void UpdatePattern()
    {
        for(int i = 0; i < m_grid.CurrentPatternSet.patternSet.Length; i++)
        {
            patternImages[i].sprite = m_grid.CurrentPatternSet.patternSet[i].icon;
        }
    }

    private void LockPattern()
    {
        for(int i = 0; i < m_grid.patterns.Length; i++)
        {
            if(m_grid.patterns[i].used)
            {
                lockImages[i].enabled = true;
            }
        }
    }
    
    private void UnLockPattern()
    {
        for (int i = 0; i < m_grid.patterns.Length; i++)
        {
            if (!m_grid.patterns[i].used)
            {
                lockImages[i].enabled = false;
            }
        }
    }
}
