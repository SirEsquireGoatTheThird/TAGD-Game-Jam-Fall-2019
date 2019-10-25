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
    RaycastHit2D m_firstHit;
    int[] m_firstHitIndex;
    bool m_bulletSelected = false;
    RaycastHit2D m_secondHit;
    private Camera m_mainCamera;

    // The node will act as grid points with data thats needed in each point.
    private struct Node
    {
        public Vector2Int worldPosition;
        public bool isOccupied;
        public BoxCollider2D collider;
        public GameObject nodeObj;
    }

    private void Start()
    {
        m_mainCamera = Camera.main;
        CreateGrid();
        SpawnBullets();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < m_bulletsArray.Length; i++)
            {
                DirectionMove(m_bulletsArray[i].GetComponent<BulletActor>(), m_bulletsArray[i].GetComponent<BulletActor>().direction);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo  = Physics2D.Raycast(m_mainCamera.ScreenPointToRay(Input.mousePosition).origin, m_mainCamera.ScreenPointToRay(Input.mousePosition).direction);
            // Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction, Color.red, 10f);
            if(hitInfo)
            {
                int[] index = PosToIndex(hitInfo.transform.position);

                if(index != null)
                {
                    if (m_grid[index[0], index[1]].isOccupied == true && m_bulletSelected == false)
                    {
                        m_firstHit = hitInfo;
                        m_firstHitIndex = PosToIndex(hitInfo.transform.position);
                        m_bulletSelected = true;
                    }

                    if (m_grid[index[0], index[1]].isOccupied == false && m_bulletSelected == true)
                    {
                        m_secondHit = hitInfo;
                        int[] m_firstIndex = PosToIndex(m_firstHit.transform.position);
                        int[] m_secondIndex = PosToIndex(m_secondHit.transform.position);
                        int[] m_indexDiff = new int[2];
                        m_indexDiff[0] = m_secondIndex[0] - m_firstIndex[0];
                        m_indexDiff[1] = m_secondIndex[1] - m_firstIndex[1];
                        //Debug.Log(m_indexDiff[0] + " " + m_indexDiff[1]);
                        //Debug.Log("Index its suppose to be at now: " + (m_firstIndex[0] + m_indexDiff[0]) + " " + (m_firstIndex[1] + m_indexDiff[1]));
                        //if ((Mathf.Abs(m_indexDiff[0]) > 1 || Mathf.Abs(m_indexDiff[1]) > 1))
                        //{
                        //    return;
                        //}
                        if((Mathf.Abs(m_indexDiff[0]) == 1 && Mathf.Abs(m_indexDiff[1]) == 1) || (Mathf.Abs(m_indexDiff[0]) == 2 && Mathf.Abs(m_indexDiff[1]) == 2))
                        {
                            return;
                        }
                        else if((Mathf.Abs(m_indexDiff[0]) == 2 && Mathf.Abs(m_indexDiff[1]) == 1) || (Mathf.Abs(m_indexDiff[0]) == 1 && Mathf.Abs(m_indexDiff[1]) == 2))
                        {
                            return;
                        }
                        Vector2Int direciton = new Vector2Int(m_indexDiff[0], m_indexDiff[1]);
                        DirectionMove(IndexToBullet(m_firstHitIndex), direciton);
                        m_bulletSelected = false;
                    }

                    if (m_grid[index[0], index[1]].isOccupied == true && m_bulletSelected == true)
                    {
                        m_firstHit = hitInfo;
                        m_firstHitIndex = PosToIndex(hitInfo.transform.position);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_bulletSelected = false;
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
                nodePoint.worldPosition = new Vector2Int(x * m_gridScale * 2, y * m_gridScale * 2);
                nodePoint.nodeObj = new GameObject();
                nodePoint.nodeObj.transform.position = new Vector3(x * m_gridScale * 2, y * m_gridScale * 2, 0);
                nodePoint.nodeObj.transform.parent = gameObject.transform;
                nodePoint.nodeObj.gameObject.name = x.ToString() + " " + y.ToString();
                nodePoint.isOccupied = false;
                nodePoint.collider = nodePoint.nodeObj.AddComponent<BoxCollider2D>();
                nodePoint.collider.size = new Vector2(m_gridScale * 2, m_gridScale * 2);
                m_grid[x, y] = nodePoint;
            }
        }
    }

    private void SpawnBullets()
    {
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

                if (m_grid[randXIndex, randYIndex].isOccupied == false)
                {
                    GameObject bullet = Instantiate(m_bulletPrefabArray[i], Vector3.zero, Quaternion.identity);
                    BulletActor actor = bullet.GetComponent<BulletActor>();
                    actor.direction = rotArray[i];
                    actor.position = new Vector2Int(m_grid[randXIndex, randYIndex].worldPosition.x, m_grid[randXIndex, randYIndex].worldPosition.y);
                    actor.indexOnGrid[0] = randXIndex;
                    actor.indexOnGrid[1] = randYIndex;
                    actor.order = i;
                    m_grid[randXIndex, randYIndex].isOccupied = true;
                    m_grid[randXIndex, randYIndex].isOccupied = true;
                    m_bulletsArray[i] = bullet;
                    break;
                }

            }
        }
    }

    private int[] PosToIndex(Vector3 position)
    {
        Vector2Int convertedVector = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        for(int i = 0; i < m_gridWidth; i++)
        {
            for (int ii = 0; ii < m_gridHeight; ii++)
            {
                if(m_grid[i,ii].worldPosition == convertedVector)
                {
                    int[] index = new int[2];
                    index[0] = i;
                    index[1] = ii;
                    return index;
                }
            }
        }
        return null;
    }

    private BulletActor IndexToBullet(int[] index)
    {
        foreach(GameObject obj in m_bulletsArray)
        {
            if(obj.GetComponent<BulletActor>().indexOnGrid[0] == index[0] && obj.GetComponent<BulletActor>().indexOnGrid[1] == index[1])
            {
                return obj.GetComponent<BulletActor>();
            }
        }
        return null;
    }
    private void DirectionMove(BulletActor bullet, Vector2Int direc = new Vector2Int())
    {
        // bullet.indexOnGrid is the position of the bullet on the grid
        // bullet.direction is the direction the bullet faces as a vector 2, can seperate as a change in x or y
        // When bullet moves I got to update bullets indexOnGrid value
        // Update position of bullet using scaled position values
        // Cap indexs within range of 0 to 2
        // Bullet wont move if another is in its way

        if(bullet == null)
        {
            return;
        }

        int newXIndex = bullet.indexOnGrid[0] + direc.x;
        int newYIndex = bullet.indexOnGrid[1] + direc.y;

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
