using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
            GameManager.Instance.UpdatePatternUI.Invoke();
            m_currentPatternSet = value;
        }
    }
    private Pattern[] patterns = new Pattern[3];
    int patternCount = 0;


    #region Struct creations
    // The node will act as grid points with data thats needed in each point.
    private struct Node
    {
        public Vector3 worldPosition;
        public bool isOccupied;
        public BoxCollider2D collider;
        public GameObject nodeObj;
    }

    private struct Pattern
    {
        public bool used;
        public int index;
    }

    #endregion 

    private void Start()
    {
        m_mainCamera = Camera.main;
        GameManager.Instance.UpdatePatternUI.Invoke();
        ScaleBackgroundImageToGrid();
        CreatePatternStructs();
        CreateGrid();
        SpawnBullets();
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RayCastTarget();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_bulletSelected = false;
        }
    }



    #region Game Initilization
    private void CreateGrid()
    {
        m_grid = new Node[m_gridWidth, m_gridHeight];
        for (int x = -1; x < m_gridWidth - 1; x++)
        {
            for (int y = -1; y < m_gridHeight - 1; y++)
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
                m_grid[x + 1, y + 1] = nodePoint;
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
    private void CreatePatternStructs()
    {
        for(int i = 0; i < m_currentPatternSet.patternSet.Length; i++)
        {
            Pattern pattern = new Pattern();
            pattern.index = i;
            pattern.used = false;
        }
    }
    private void ScaleBackgroundImageToGrid()
    {
        if (m_backGroundObject != null)
        {
            m_backGroundObject.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
        }
    }
    #endregion

    #region Reset Game Values
    private void ResetPattern()
    {
        for (int i = 0; i < patterns.Length; i++)
        {
            patterns[i].used = false;
        }
    }
    private void ResetBulletsPattern()
    {
        BulletActor reference;

        for (int i = 0; i < m_bullets.Length; i++)
        {
            reference = m_bullets[i].GetComponent<BulletActor>();
            reference.inPattern = false;
        }
    }
    #endregion


    private int[] WorldPosToIndex(Vector3 position)
    {
        for (int x = 0; x < m_gridWidth; x++)
        {
            for (int y = 0; y < m_gridHeight; y++)
            {
                if (m_grid[x, y].worldPosition == position)
                {
                    int[] index = new int[2];
                    index[0] = x;
                    index[1] = y;
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
            BulletActor comparable = obj.GetComponent<BulletActor>();
            if (comparable.indexOnGrid[0] == index[0] && comparable.indexOnGrid[1] == index[1])
            {
                return comparable;
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

        if (bullet == null || bullet.inPattern)
        {
            return;
        }

        // Apply vector difference of indexs on grid
        int newXIndex = bullet.indexOnGrid[0] + direc.x;
        int newYIndex = bullet.indexOnGrid[1] + direc.y;

        // This code if block is for wrapping of the bullet
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

        // Don't let the bullet move if the space it wants to move to is occupied
        if (m_grid[newXIndex, newYIndex].isOccupied)
        {
            return;
        }


        bullet.position = m_grid[newXIndex, newYIndex].worldPosition;
        m_grid[newXIndex, newYIndex].isOccupied = true;
        m_grid[bullet.indexOnGrid[0], bullet.indexOnGrid[1]].isOccupied = false;
        bullet.indexOnGrid[0] = newXIndex;
        bullet.indexOnGrid[1] = newYIndex;

        if(PatternCheckWithoutMovement())
        {
            PatternCheck();
        }

    }
    private void PatternCheck()
    {
        Vector2Int[][] bulletDiffArray = BulletIndexDifferences();

        // What I need to do.
        // Concept: 
        // The player has moved the bullet, so check if that bullet is in a pattern
        // If the bullet is in a pattern then lock the current bullets in position
        // Then move the unlocked bullet


        for (int p = 0; p < m_currentPatternSet.patternSet.Length; p++)
        {

            if(patterns[p].used)
            {
                ResetBulletsPattern();
                continue;
            }

            for (int i = 0; i < bulletDiffArray.Length; i++)
            {
                var intersectionValues = bulletDiffArray[i].Intersect(m_currentPatternSet.patternSet[p].differenceFromOrigin);

                if (intersectionValues.Count() == 2)
                {
                    Vector2Int[] intersecComp = intersectionValues.ToArray();
                    
                    for (int x = 0; x < m_bullets.Length; x++)
                    {
                        BulletActor firstAct = m_bullets[i].GetComponent<BulletActor>();
                        BulletActor secondAct = m_bullets[x].GetComponent<BulletActor>();
                        Vector2Int gridDiff = new Vector2Int(secondAct.indexOnGrid[0] - firstAct.indexOnGrid[0], secondAct.indexOnGrid[1] - firstAct.indexOnGrid[1]);

                        if (intersecComp[0] == gridDiff || intersecComp[1] == gridDiff)
                        {
                            firstAct.inPattern = true;
                            secondAct.inPattern = true;
                            patterns[p].used = true;
                        }
                       
                    }

                    
                }

            }
        }

        for(int i = 0; i < patterns.Length; i++)
        {
            if(patterns[i].used)
            {
                patternCount++;
            }
        }
        if(patternCount == 3 || patternCount == 0)
        {
            return;
        }
        patternCount = 0;
        MoveRemainingBullet();

    }
    private void MoveRemainingBullet()
    {
        BulletActor bulletReference;
        for (int i = 0; i < m_bullets.Length; i++)
        {
            bulletReference = m_bullets[i].GetComponent<BulletActor>();
            if(!bulletReference.inPattern)
            {
                DirectionMove(bulletReference, bulletReference.direction);;
            }
        }
    }
    private void RayCastTarget()
    {
        Ray mousePos = m_mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(mousePos.origin, mousePos.direction);

        if (hitInfo)
        {
            int[] index = WorldPosToIndex(hitInfo.transform.position);

            if (index == null)
            {
                return;
            }

            if (m_grid[index[0], index[1]].isOccupied && !m_bulletSelected)
            {
                m_firstHitIndex = WorldPosToIndex(hitInfo.transform.position);
                m_bulletSelected = true;
            }

            if (!m_grid[index[0], index[1]].isOccupied && m_bulletSelected)
            {
                m_secondHit = hitInfo;
                int[] m_secondIndex = WorldPosToIndex(m_secondHit.transform.position);
                int[] m_indexDiff = new int[2];
                m_indexDiff[0] = m_secondIndex[0] - m_firstHitIndex[0];
                m_indexDiff[1] = m_secondIndex[1] - m_firstHitIndex[1];
                // These just check for diagonal movement and 2 - 1 or 1 - 2 movements. Think of restricting chess movement of the knight
                if((Mathf.Abs(m_indexDiff[0]) == 1 && Mathf.Abs(m_indexDiff[1]) == 1) || (Mathf.Abs(m_indexDiff[0]) == 2 && Mathf.Abs(m_indexDiff[1]) == 2))
                {
                    return;
                }
                else if((Mathf.Abs(m_indexDiff[0]) == 2 && Mathf.Abs(m_indexDiff[1]) == 1) || (Mathf.Abs(m_indexDiff[0]) == 1 && Mathf.Abs(m_indexDiff[1]) == 2))
                {
                    return;
                }
                Vector2Int direction = new Vector2Int(m_indexDiff[0], m_indexDiff[1]);
                DirectionMove(IndexToBullet(m_firstHitIndex), direction);
                m_bulletSelected = false;
            }

            if (m_grid[index[0], index[1]].isOccupied && m_bulletSelected)
            {
                m_firstHitIndex = WorldPosToIndex(hitInfo.transform.position);
            }
        }
    }
    private Vector2Int[][] BulletIndexDifferences()
    {
        Vector2Int[][] bulletDiffArray = new Vector2Int[4][];
        Vector2Int[] caiou = new Vector2Int[4];
        for (int i = 0; i < 4; i++)
        {
            for (int x = 0; x < 4; x++)
            {
                BulletActor firstAct = m_bullets[i].GetComponent<BulletActor>();
                BulletActor secondAct = m_bullets[x].GetComponent<BulletActor>();
                Vector2Int indexDiff = new Vector2Int(secondAct.indexOnGrid[0] - firstAct.indexOnGrid[0], secondAct.indexOnGrid[1] - firstAct.indexOnGrid[1]);
                caiou[x] = indexDiff;

            }
            bulletDiffArray[i] = caiou;
            caiou = new Vector2Int[4];
        }

        return bulletDiffArray;
    }
    private bool PatternCheckWithoutMovement()
    {
        Vector2Int[][] bulletDiffArray = BulletIndexDifferences();

        // What I need to do.
        // Concept: 
        // The player has moved the bullet, so check if that bullet is in a pattern
        // If the bullet is in a pattern then lock the current bullets in position
        // Then move the unlocked bullet


        for (int p = 0; p < m_currentPatternSet.patternSet.Length; p++)
        {

            if(patterns[p].used)
            {
                continue;
            }

            for (int i = 0; i < bulletDiffArray.Length; i++)
            {
                var intersectionValues = bulletDiffArray[i].Intersect(m_currentPatternSet.patternSet[p].differenceFromOrigin);

                if (intersectionValues.Count() == 2)
                {
                    Vector2Int[] intersecComp = intersectionValues.ToArray();

                    for (int x = 0; x < m_bullets.Length; x++)
                    {
                        BulletActor firstAct = m_bullets[i].GetComponent<BulletActor>();
                        BulletActor secondAct = m_bullets[x].GetComponent<BulletActor>();
                        Vector2Int gridDiff = new Vector2Int(secondAct.indexOnGrid[0] - firstAct.indexOnGrid[0], secondAct.indexOnGrid[1] - firstAct.indexOnGrid[1]);

                        if (intersecComp[0] == gridDiff || intersecComp[1] == gridDiff)
                        {
                            return true;
                        }

                    }


                }

            }
        }
        return false;
    }




    private void OnDrawGizmos()
    {
        if (m_grid != null)
        {
            foreach (Node x in m_grid)
            {
                Color color = Color.grey;
                if(x.isOccupied)
                {
                    color = Color.red;
                }
                Gizmos.color = color;
                Gizmos.DrawCube(new Vector3(x.worldPosition.x, x.worldPosition.y, 0), Vector3.one / 4);
            }
        }

        // This is to simply show where the grid will be so scaling isnt a guessing process
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Vector3.one * m_gridScale * 3);

    }


}
