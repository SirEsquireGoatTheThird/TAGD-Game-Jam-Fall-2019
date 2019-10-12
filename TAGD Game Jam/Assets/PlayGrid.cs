using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayGrid : MonoBehaviour
{
    [SerializeField]
    private int m_gridSize = 3;
    private GridLayout m_gridLayout;

    private void Awake()
    {
        m_gridLayout = new GridLayout(); 
    }




}
