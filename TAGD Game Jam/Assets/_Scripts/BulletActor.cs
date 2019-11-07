using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletActor : MonoBehaviour, IBullet
{
    Vector2Int m_direction;
    Vector3 m_position;
    [SerializeField]
    int[] m_indexOnGrid = new int[2];
    int m_order;
    [SerializeField]
    bool m_inPattern;
    public bool inPosition;

    public Vector2Int direction
    {
        get
        {
            return m_direction;
        }
        set
        {
            m_direction = value;
        }
    }
    public Vector3 position
    {
        get
        {
            return m_position;
        }
        set
        {
            inPosition = false;
            m_position = value;
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

    public bool inPattern
    {
        get
        {
            return m_inPattern;
        }
        set
        {
            m_inPattern = value;
        }
    }

    private void Update()
    {
       if(VectorDifference() < 0.01f)
        {
            transform.position = position;
            inPosition = true;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(position.x, position.y, 0), 2 * Time.deltaTime);
        }
        
    }

    public void SetTransform(Vector3 pos)
    {
        transform.position = pos;
    }

    private float VectorDifference()
    {
        Vector3 diff = position - transform.position;
        return diff.magnitude;
    }
}
