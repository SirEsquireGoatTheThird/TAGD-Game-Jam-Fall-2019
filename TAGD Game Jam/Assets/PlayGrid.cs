using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayGrid : MonoBehaviour
{
    [SerializeField]
    private int m_gridHeight = 3;
    [SerializeField]
    private int m_gridWidth = 3;
    private Node[,] m_grid;
    [SerializeField]
    [Range(1, 20)]
    private int m_gridScale = 1;
    private int m_numberOfBullets = 4;
    [SerializeField]
    private GameObject[] m_bulletPrefabArray;
    private GameObject[] m_bulletsArray;

    // The node will act as grid points with data thats needed in each point.
    private struct Node
    {
        public Vector2Int worldPosition;
        public bool isOccupied;
    }


    private void Start()
    { 
        CreateGrid();
        m_grid = ScalePositions;
        SpawnBullets();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < m_bulletsArray.Length; i++)
            {
                DirectionMove(m_bulletsArray[i].GetComponent<BulletActor>());
            }
        }
    }

    private void CreateGrid()
    {
        m_grid = new Node[m_gridWidth, m_gridHeight];
        for(int x = 0; x < m_gridWidth; x++)
        {
            for(int y = 0; y < m_gridHeight; y++)
            {
                Node nodePoint = new Node();
                nodePoint.worldPosition = new Vector2Int(x, y);
                nodePoint.isOccupied = false;
                m_grid[x, y] = nodePoint;
            }
        }
    }

    private Node[,] ScalePositions
    {
        get
        {
            Node[,] scaledNodes = m_grid;

            for (int x = 0; x < m_gridWidth; x++)
            {
                for (int y = 0; y < m_gridHeight; y++)
                {
                    scaledNodes[x, y].worldPosition = m_grid[x, y].worldPosition * m_gridScale;
                }
            }

            return scaledNodes;
        }
    }

    private void SpawnBullets()
    {
        Node[,] spawnGrid = ScalePositions;
        m_bulletsArray = new GameObject[m_numberOfBullets];
        Vector2Int[] rotArray = new Vector2Int[]{
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
            };
        for (int i = 0; i < m_numberOfBullets; i++)
        {
            while(true)
            { 
                int randXIndex = Random.Range(0, m_gridWidth);
                int randYIndex = Random.Range(0, m_gridHeight);

                if (spawnGrid[randXIndex, randYIndex].isOccupied == false)
                {
                    GameObject bullet = Instantiate(m_bulletPrefabArray[i], Vector3.zero, Quaternion.identity);
                    BulletActor actor = bullet.GetComponent<BulletActor>();
                    actor.direction = rotArray[i];
                    actor.position = new Vector2Int(spawnGrid[randXIndex, randYIndex].worldPosition.x, spawnGrid[randXIndex, randYIndex].worldPosition.y);
                    actor.indexOnGrid[0] = randXIndex;
                    actor.indexOnGrid[1] = randYIndex;
                    actor.order = i;
                    m_grid[randXIndex, randYIndex].isOccupied = true;
                    spawnGrid[randXIndex, randYIndex].isOccupied = true;
                    m_bulletsArray[i] = bullet;
                    break;
                }

            }
        }
    }

    private void DirectionMove(BulletActor bullet)
    {
        // bullet.indexOnGrid is the position of the bullet on the grid
        // bullet.direction is the direction the bullet faces as a vector 2, can seperate as a change in x or y
        // When bullet moves I got to update bullets indexOnGrid value
        // Update position of bullet using scaled position values
        // Cap indexs within range of 0 to 2
        // Bullet wont move if another is in its way


        int newXIndex = bullet.indexOnGrid[0] + bullet.direction.x;
        int newYIndex = bullet.indexOnGrid[1] + bullet.direction.y;

        if(newXIndex < 0)
        {
            newXIndex = m_gridWidth - Mathf.Abs(newXIndex);
        }
        else if(newXIndex > (m_gridWidth - 1))
        {
            newXIndex = Mathf.Abs(newXIndex) - m_gridWidth;
        }

        if (newYIndex < 0)
        {
            newYIndex = m_gridHeight - Mathf.Abs(newYIndex);
        }
        else if (newYIndex > (m_gridHeight - 1))
        {
            newYIndex = Mathf.Abs(newYIndex) - m_gridHeight;
        }

        if (m_grid[newXIndex, newYIndex].isOccupied == true)
        {
            return;
        }


        bullet.position = m_grid[newXIndex, newYIndex].worldPosition;
        m_grid[newXIndex, newYIndex].isOccupied = true;
        m_grid[bullet.indexOnGrid[0], bullet.indexOnGrid[1]].isOccupied = false;
        bullet.indexOnGrid[0] = newXIndex;
        bullet.indexOnGrid[1] = newYIndex;

    }

    private void OnDrawGizmos()
    {
        if(m_grid != null)
        {
            foreach(Node x in m_grid)
            {
                Gizmos.DrawCube(new Vector3(x.worldPosition.x, x.worldPosition.y, 0), Vector3.one / 4);
            }
        }
            
    }


}
