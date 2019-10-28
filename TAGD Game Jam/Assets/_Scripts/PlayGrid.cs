using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayGrid : MonoBehaviour
{
    // Grid Initialization Stuff
    [SerializeField]
    private int m_gridHeight = 3;
    [SerializeField]
    private int m_gridWidth = 3;
    private Node[,] m_grid;
    [SerializeField]
    [Range(1, 20)]
    private float m_gridScale = 1;

    // Raycast Stuff
    RaycastHit2D m_firstHit;
    int[] m_firstHitIndex;
    bool m_bulletSelected = false;
    RaycastHit2D m_secondHit;
    private Camera m_mainCamera;

    // Bullet Stuff
    private int m_numberOfBullets = 4;
    [SerializeField]
    private GameObject[] m_bulletPrefabs;
    private GameObject[] m_bullets;

    // Background scale
    [SerializeField]
    private GameObject m_backGroundObject;

    // Pattern Finding
    [SerializeField]
    private PatternSetScriptable m_currentPatternSet;
    public PatternSetScriptable CurrentPatternSet
    {
        get
        {
            return m_currentPatternSet;
        }
        set
        {
            m_currentPatternSet = value;
        }
    }
    private int m_numOfPatterns;
    private Vector2Int[,] bulletIndexDifference;


    // The node will act as grid points with data thats needed in each point.
    private struct Node
    {
        public Vector3 worldPosition;
        public bool isOccupied;
        public BoxCollider2D collider;
        public GameObject nodeObj;
    }

    private void Start()
    {
        m_mainCamera = Camera.main;
        if(m_backGroundObject != null)
        {
            m_backGroundObject.transform.localPosition = new Vector3(m_gridScale, m_gridScale, 0);
            m_backGroundObject.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
        }
        
        CreateGrid();
        SpawnBullets();
        ResetPatterns();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < m_bullets.Length; i++)
            {
                DirectionMove(m_bullets[i].GetComponent<BulletActor>(), m_bullets[i].GetComponent<BulletActor>().direction);
                PatternCheck();
                ResetPatterns();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(m_mainCamera.ScreenPointToRay(Input.mousePosition).origin, m_mainCamera.ScreenPointToRay(Input.mousePosition).direction);
            // Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction, Color.red, 10f);
            if (hitInfo)
            {
                int[] index = PosToIndex(hitInfo.transform.position);

                if (index != null)
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

                        if ((Mathf.Abs(m_indexDiff[0]) == 1 && Mathf.Abs(m_indexDiff[1]) == 1) || (Mathf.Abs(m_indexDiff[0]) == 2 && Mathf.Abs(m_indexDiff[1]) == 2))
                        {
                            return;
                        }
                        else if ((Mathf.Abs(m_indexDiff[0]) == 2 && Mathf.Abs(m_indexDiff[1]) == 1) || (Mathf.Abs(m_indexDiff[0]) == 1 && Mathf.Abs(m_indexDiff[1]) == 2))
                        {
                            return;
                        }
                        Vector2Int direciton = new Vector2Int(m_indexDiff[0], m_indexDiff[1]);
                        DirectionMove(IndexToBullet(m_firstHitIndex), direciton);
                        PatternCheck();
                        ResetPatterns();
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_bulletSelected = false;
        }
    }

    private void CreateGrid()
    {
        m_grid = new Node[m_gridWidth, m_gridHeight];
        for (int x = 0; x < m_gridWidth; x++)
        {
            for (int y = 0; y < m_gridHeight; y++)
            {
                // Create a new nodepoint at that value
                Node nodePoint = new Node();
                // Scale worldPosition by m_gridScale
                nodePoint.worldPosition = new Vector3((x * m_gridScale) + transform.position.x, (y * m_gridScale) + transform.position.y, 0);
                nodePoint.nodeObj = new GameObject();
                nodePoint.nodeObj.transform.position = new Vector3((x * m_gridScale) + transform.position.x, (y * m_gridScale) + transform.position.y, 0);
                nodePoint.nodeObj.transform.parent = gameObject.transform;
                nodePoint.nodeObj.gameObject.name = x.ToString() + " " + y.ToString();
                nodePoint.isOccupied = false;
                nodePoint.collider = nodePoint.nodeObj.AddComponent<BoxCollider2D>();
                nodePoint.collider.transform.position = new Vector3((x * m_gridScale) + transform.position.x, (y * m_gridScale) + transform.position.y, 0);
                nodePoint.collider.size = new Vector2(m_gridScale, m_gridScale);
                m_grid[x, y] = nodePoint;
            }
        }
    }

    private void SpawnBullets()
    {
        m_bullets = new GameObject[m_numberOfBullets];
        Vector2Int[] rotArray = new Vector2Int[]{
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0)
            };
        Vector2Int[] indexArray = new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 2),
            new Vector2Int(2, 0),
            new Vector2Int(2, 2)
        };
        for (int i = 0; i < m_numberOfBullets; i++)
        {

            GameObject bullet = Instantiate(m_bulletPrefabs[i], Vector3.zero, Quaternion.identity);
            BulletActor actor = bullet.GetComponent<BulletActor>();
            bullet.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
            actor.direction = rotArray[i];
            actor.SetTransform(m_grid[indexArray[i].x, indexArray[i].y].worldPosition);
            actor.position = m_grid[indexArray[i].x, indexArray[i].y].worldPosition;
            actor.indexOnGrid[0] = indexArray[i].x;
            actor.indexOnGrid[1] = indexArray[i].y;
            actor.order = i;
            m_grid[indexArray[i].x, indexArray[i].y].isOccupied = true;
            actor.inPattern = false;
            m_bullets[i] = bullet;

        }
    }

    private int[] PosToIndex(Vector3 position)
    {
        for (int i = 0; i < m_gridWidth; i++)
        {
            for (int ii = 0; ii < m_gridHeight; ii++)
            {
                if (m_grid[i, ii].worldPosition == position)
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
        foreach (GameObject obj in m_bullets)
        {
            if (obj.GetComponent<BulletActor>().indexOnGrid[0] == index[0] && obj.GetComponent<BulletActor>().indexOnGrid[1] == index[1])
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

        if (bullet.inPattern == true)
        {
            return;
        }

        if (bullet == null)
        {
            return;
        }

        int newXIndex = bullet.indexOnGrid[0] + direc.x;
        int newYIndex = bullet.indexOnGrid[1] + direc.y;

        if (newXIndex < 0)
        {
            newXIndex = m_gridWidth - Mathf.Abs(newXIndex);
        }
        else if (newXIndex > (m_gridWidth - 1))
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

    private void PatternCheck()
    {
        Vector2Int[][] bulletDiffArray = new Vector2Int[m_bullets.Length][];
        List<Vector2Int> value = new List<Vector2Int>();
        for (int i = 0; i < m_bullets.Length; i++)
        {
            for (int x = 0; x < m_bullets.Length; x++)
            {
                BulletActor firstAct = m_bullets[i].GetComponent<BulletActor>();
                BulletActor secondAct = m_bullets[x].GetComponent<BulletActor>();
                value.Add(new Vector2Int(secondAct.indexOnGrid[0] - firstAct.indexOnGrid[0], secondAct.indexOnGrid[1] - firstAct.indexOnGrid[1]));

            }
            bulletDiffArray[i] = value.ToArray();
            value = new List<Vector2Int>();
        }
        
        
        int patternCount = 0;

        for (int p = 0; p < m_currentPatternSet.patternSet.Length; p++)
        {
            for (int i = 0; i < bulletDiffArray.Length; i++)
            {
                if (bulletDiffArray[i].Intersect(m_currentPatternSet.patternSet[p].differenceFromOrigin).Count() == 2)
                {
                    patternCount++;
                }
            }
            
        }

        Debug.Log("Number of patterns found is " + patternCount);
    }

    private void ResetPatterns()
    {
        for(int i = 0; i < m_currentPatternSet.patternSet.Length; i++)
        {

        }
    }


    
    
    private void OnDrawGizmos()
    {
        if (m_grid != null)
        {
            foreach (Node x in m_grid)
            {
                Gizmos.DrawCube(new Vector3(x.worldPosition.x, x.worldPosition.y, 0), Vector3.one / 4);
            }
        }

    }


}
