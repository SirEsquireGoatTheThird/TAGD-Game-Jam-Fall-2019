using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletActor : MonoBehaviour, IBullet
{
    Vector2Int m_direction;
    Vector2Int m_position;
    [SerializeField]
    int[] m_indexOnGrid = new int[2];
    int m_order;

    public Vector2Int direction
    {
        get
        {
            return m_direction;
        }
        set
        {
            m_direction = value;
            Vector3 actualAngle = Vector3.zero;
            // Up
            if(m_direction == new Vector2(0, 1))
            {
                actualAngle = new Vector3(0, 0, 0);
            }
            // Down
            if (m_direction == new Vector2(0, -1))
            {
                actualAngle = new Vector3(0, 0, 180);
            }
            // Right
            if (m_direction == new Vector2(1, 0))
            {
                actualAngle = new Vector3(0, 0, 270);
            }
            // Left
            if (m_direction == new Vector2(-1, 0))
            {
                actualAngle = new Vector3(0, 0, 90);
            }

            transform.rotation = Quaternion.Euler(actualAngle);
        }
    }
    public Vector2Int position
    {
        get
        {
            return m_position;
        }
        set
        {
            m_position = value;
            transform.position = new Vector3(m_position.x, m_position.y, 0);
        }
    }
    public int[] indexOnGrid
    {
        get
        {
            return m_indexOnGrid;
        }
        set
        {
            m_indexOnGrid = value;
        }
    }
    public int order
    {
        get
        {
            return m_order;
        }
        set
        {
            m_order = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
