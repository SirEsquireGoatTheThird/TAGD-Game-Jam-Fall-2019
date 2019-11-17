using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGrid : MonoBehaviour
{
    // Grid Initialization Stuff
    private int m_gridHeight = 3;
    private int m_gridWidth = 3;
    private Node[,] m_grid;
    private float m_gridScale = 1;


    // Raycast Stuff
    private int[] m_firstHitIndex;
    private bool m_bulletSelected = false;
    private RaycastHit2D m_secondHit;
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
    [HideInInspector]
    public Pattern[] patterns = new Pattern[3];
    private int patternCount = 0;
    [SerializeField]
    private PatternSetScriptable[] m_patternSets;

    [SerializeField]
    private bool m_actionPhase = false;


    //Variables used for enemy stuff 
    [SerializeField]
    //////////////////
    private StatsScriptable[] enemy;
    private int i = 0;
    //////////////////
    private StatsScriptable currentEnemy;
    [SerializeField]
    private GameObject other;
    Player_Health Health_UI;
    Enemy_Health Enemy_UI;
    public int player_health = 10;
    public int enemy_health = 5;
    public int enemy_damage = 1;
    public float duration = 30;
    private float time;

    // Variables used for storing the ghost prefab to show the player where they can move their bullet
    [SerializeField]
    private GameObject[] m_ghostBullets;
    private List<GameObject> m_spawnedGhostBullets = new List<GameObject>();

    // VFX Stuff
    [SerializeField]
    private GameObject m_patternMadeVFX;
    [SerializeField]
    private GameObject m_smallVape;
    [SerializeField]
    private GameObject m_bigVape;

    //End Game time
    [SerializeField]
    private float m_waitTime;

    [SerializeField]
    private Image m_enemyIcon;

    private bool enemyDied = false;


    #region Struct creations
    // The node will act as grid points with data thats needed in each point.
    private struct Node
    {
        public Vector3 worldPosition;
        public bool isOccupied;
        public BoxCollider2D collider;
        public GameObject nodeObj;
    }

    public struct Pattern
    {
        public bool used;
        public int index;
    }

    #endregion 

    private void Start()
    {
        m_currentPatternSet = m_patternSets[i];
        ///////////
        currentEnemy = enemy[0];
        ///////////
        m_mainCamera = Camera.main;
        GameManager.Instance.UpdatePatternUI.Invoke();
        SetScaleOfGridByScreenResolution();
        ScaleBackgroundImageToGrid();
        CreatePatternStructs();
        CreateGrid();
        SpawnBullets();

        /////////////////////////
        Health_UI = other.GetComponent<Player_Health>();
        Enemy_UI = other.GetComponent<Enemy_Health>();
        //////////////////////////
        enemy_health = currentEnemy.health;
        enemy_damage = currentEnemy.damage;
        duration = currentEnemy.attackTimer;
        time = duration;
        Enemy_UI.health = enemy_health;
        Enemy_UI.UpdateOrbValues(enemy_health);
    }

    private void Update()
    {
        if(enemy_health <= 0 && !enemyDied)
        {
            StartCoroutine(NextLevel());
            return;
        }
        if(player_health <= 0)
        {
            Debug.Log("You died");
        }


        if (Input.GetMouseButtonDown(0) && !m_actionPhase)
        {
            RayCastTarget();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_bulletSelected = false;
        }

        time -= Time.deltaTime;

        if (time < 0)
        {
            player_health -= enemy_damage;
            Health_UI.Damage(enemy_damage);
            time = duration;
        }
        
    }

    private IEnumerator patternTime()
    {
        yield return new WaitForSeconds(1f);
        ResetPattern();
    }

    private IEnumerator NextLevel()
    {
        enemyDied = true;
        GameObject smolVape = Instantiate(m_smallVape, m_enemyIcon.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(2f);
        Destroy(smolVape);

        GameObject bigVape = Instantiate(m_bigVape, m_enemyIcon.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);

        i += 1;
        if (i < enemy.Length)
        {
            currentEnemy = enemy[i];
            enemy_health = currentEnemy.health;
            enemy_damage = currentEnemy.damage;
            duration = currentEnemy.attackTimer;
            time = duration;
            Enemy_UI.health = enemy_health;
            Enemy_UI.UpdateOrbValues(enemy_health);
            m_enemyIcon.sprite = enemy[i].icon;
            GameManager.Instance.NextEnemy.Invoke();
            for(int index = 0; index < m_bullets.Length; index++)
            {
                Destroy(m_bullets[index]);
            }
            //Reset grid occupence
            for (int x = 0; x < m_gridWidth; x++)
            {
                for (int y = 0; y < m_gridHeight; y++)
                {
                    m_grid[x, y].isOccupied = false;
                }
            }
            m_actionPhase = false;
            enemyDied = false;
            SpawnBullets();

            //Change the pattern set
            m_currentPatternSet = m_patternSets[i];
            CreatePatternStructs();
            GameManager.Instance.UpdatePatternUI.Invoke();

        }
        Destroy(bigVape);

    }

    #region Game Initilization
    private void SetScaleOfGridByScreenResolution()
    {
        float y = m_mainCamera.pixelHeight;
        float x = m_mainCamera.pixelWidth;


        // First get the bounds of the screen via ratios for screen ratio to height and width
        float ratioY = y / 135;
        float ratioX = x / 240;

        // Used to determine the starting point on the bottom left of the object
        float xPos = ratioX * 15;
        float yPos = ratioY * 23;

        float xPosCenter = ratioX * 69;
        float yPosCenter = ratioY * 77;

        Vector3 origin = m_mainCamera.ScreenToWorldPoint(new Vector3(xPosCenter, yPosCenter, 0));
        transform.position = origin;

        // Adds the length of the container
        float width = xPos + (108 * ratioX);

        Vector3 startPos = m_mainCamera.ScreenToWorldPoint(new Vector3(xPos, yPos, 0));
        Vector3 endPos = m_mainCamera.ScreenToWorldPoint(new Vector3(width, yPos, 0));

        //Debug.Log(startPos);
        //Debug.Log(endPos);

        m_gridScale = Mathf.Abs(origin.x - startPos.x) / 1.75f;
        


    }
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
            actor.indexInArray = i;
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
        GameManager.Instance.UnlockPattern.Invoke();
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
        int[] wrappedIndex = WrapIndex(newXIndex, newYIndex);



        // Don't let the bullet move if the space it wants to move to is occupied
        if (m_grid[wrappedIndex[0], wrappedIndex[1]].isOccupied)
        {
            return;
        }


        bullet.position = m_grid[wrappedIndex[0], wrappedIndex[1]].worldPosition;
        m_grid[wrappedIndex[0], wrappedIndex[1]].isOccupied = true;
        m_grid[bullet.indexOnGrid[0], bullet.indexOnGrid[1]].isOccupied = false;
        bullet.indexOnGrid[0] = wrappedIndex[0];
        bullet.indexOnGrid[1] = wrappedIndex[1];
    }
    private BulletActor PatternCheck()
    {
        BulletActor bullet = null;
        for (int p = 0; p < m_currentPatternSet.patternSet.Length; p++)
        {
            if(patterns[p].used)
            {
                continue;
            }
            for (int i = 0; i < m_bullets.Length; i++)
            {
                // We get the bullet at i and get the index on grid. We add the differences from the patterns and check if that grid is occupied
                // If it is occupied get the bullets at that index and set their inPattern variable to true, the remaining index set it to false.
                // Case 1: loop through all the bullets. at the origin you add the differences, if both of the differnces at my_grid == true then
                // a pattern is found in the code. We can use IndexToBullet() to get the bullet at that index to find that value.

                int[] currentIndex = m_bullets[i].GetComponent<BulletActor>().indexOnGrid;
                Vector2Int firstDiff = m_currentPatternSet.patternSet[p].differenceFromOrigin[0];
                Vector2Int secondDiff = m_currentPatternSet.patternSet[p].differenceFromOrigin[1];

                int[] firstIndexDiff = new int[]{
                    currentIndex[0] + firstDiff.x,
                    currentIndex[1] + firstDiff.y
                    };

                int[] secondIndexDiff = new int[]{
                    currentIndex[0] + secondDiff.x,
                    currentIndex[1] + secondDiff.y
                };


                if(firstIndexDiff[0] >= m_gridWidth || firstIndexDiff[1] >= m_gridHeight)
                {
                    continue;
                }
                if(secondIndexDiff[0] >= m_gridWidth || secondIndexDiff[1] >= m_gridHeight)
                {
                    continue;
                }
                if (firstIndexDiff[0] < 0 || firstIndexDiff[1] < 0)
                {
                    continue;
                }
                if (secondIndexDiff[0] < 0 || secondIndexDiff[1] < 0)
                {
                    continue;
                }


                if (m_grid[firstIndexDiff[0], firstIndexDiff[1]].isOccupied && m_grid[secondIndexDiff[0], secondIndexDiff[1]].isOccupied)
                {
                    patterns[p].used = true;
                    BulletActor currentBullet = m_bullets[i].GetComponent<BulletActor>();
                    BulletActor firstBullet = IndexToBullet(firstIndexDiff);
                    BulletActor secondBullet = IndexToBullet(secondIndexDiff);
                    BulletActor bulletNotInPattern = GetLastBullet(currentBullet, firstBullet, secondBullet);

                    currentBullet.inPattern = true;
                    firstBullet.inPattern = true;
                    secondBullet.inPattern = true;
                    bulletNotInPattern.inPattern = false;
                    bullet = bulletNotInPattern;
                    GameManager.Instance.PatternUsed.Invoke();

                    //Spawn VFX of object
                    GameObject vfx1 = Instantiate(m_patternMadeVFX, currentBullet.transform);
                    GameObject vfx2 = Instantiate(m_patternMadeVFX, firstBullet.transform);
                    GameObject vfx3 = Instantiate(m_patternMadeVFX, secondBullet.transform);

                    Destroy(vfx1, 1f);
                    Destroy(vfx2, 1f);
                    Destroy(vfx3, 1f);

                }

            }

            if (patterns[p].used)
            {
                patternCount++;
            }
            
        }
        

        switch (patternCount)
        {
            case 1:
                AttackEnemy(1);
                break;
            case 2:
                AttackEnemy(3);
                break;
            case 3:
                AttackEnemy(6);
                break;
        }
        patternCount = 0;
        return bullet;
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

                m_firstHitIndex = WorldPosToIndex(hitInfo.transform.position);

                int[] upIndex = new int[]
                {
                index[0],
                WrapIndex(index[0], index[1] + 1)[1]
                };
                int[] downIndex = new int[]
                {
                index[0],
                WrapIndex(index[0], index[1] - 1)[1]
                };
                int[] rightIndex = new int[]
                {
                WrapIndex(index[0] + 1, index[1])[0],
                index[1]
                };
                int[] leftIndex = new int[]
                {
                WrapIndex(index[0] - 1, index[1])[0],
                index[1]
                };


                if (!m_grid[upIndex[0], upIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[upIndex[0], upIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
                if (!m_grid[downIndex[0], downIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[downIndex[0], downIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
                if (!m_grid[rightIndex[0], rightIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[rightIndex[0], rightIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
                if (!m_grid[leftIndex[0], leftIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[leftIndex[0], leftIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
            }

            //Spawn the ghost bullet at (+-1, 0), (0, +-1) index differences including wrapping as long as that index is no occupied


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
                BulletActor bullet = IndexToBullet(m_firstHitIndex);
                // Move the selected bullet to the next location
                DirectionMove(bullet, direction);

                // Start the Action Phase
                StartCoroutine(ActionPhase(bullet));
                GameManager.Instance.UpdateTimeDuration.Invoke();
                m_bulletSelected = false;

                //Clear out the bullet preview list
                foreach(GameObject obj in m_spawnedGhostBullets)
                {
                    Destroy(obj);
                }

                m_spawnedGhostBullets.Clear();

            }

            if (m_grid[index[0], index[1]].isOccupied && m_bulletSelected)
            {
                m_firstHitIndex = WorldPosToIndex(hitInfo.transform.position);

                foreach (GameObject obj in m_spawnedGhostBullets)
                {
                    Destroy(obj);
                }

                m_spawnedGhostBullets.Clear();

                int[] upIndex = new int[]
                {
                index[0],
                WrapIndex(index[0], index[1] + 1)[1]
                };
                int[] downIndex = new int[]
                {
                index[0],
                WrapIndex(index[0], index[1] - 1)[1]
                };
                int[] rightIndex = new int[]
                {
                WrapIndex(index[0] + 1, index[1])[0],
                index[1]
                };
                int[] leftIndex = new int[]
                {
                WrapIndex(index[0] - 1, index[1])[0],
                index[1]
                };


                if (!m_grid[upIndex[0], upIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[upIndex[0], upIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
                if (!m_grid[downIndex[0], downIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[downIndex[0], downIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
                if (!m_grid[rightIndex[0], rightIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[rightIndex[0], rightIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }
                if (!m_grid[leftIndex[0], leftIndex[1]].isOccupied)
                {
                    GameObject objSpawned = Instantiate(m_ghostBullets[IndexToBullet(index).indexInArray], m_grid[leftIndex[0], leftIndex[1]].worldPosition, Quaternion.identity);
                    objSpawned.transform.localScale = new Vector3(m_gridScale / 2, m_gridScale / 2, 1);
                    m_spawnedGhostBullets.Add(objSpawned);
                }

            }
        }
    }
    
    private bool PatternCheckWithoutMovement()
    {
        for (int p = 0; p < m_currentPatternSet.patternSet.Length; p++)
        {
            if(patterns[p].used)
            {
                continue;
            }
            for (int i = 0; i < m_bullets.Length; i++)
            {
                // We get the bullet at i and get the index on grid. We add the differences from the patterns and check if that grid is occupied
                // If it is occupied get the bullets at that index and set their inPattern variable to true, the remaining index set it to false.
                // Case 1: loop through all the bullets. at the origin you add the differences, if both of the differnces at my_grid == true then
                // a pattern is found in the code. We can use IndexToBullet() to get the bullet at that index to find that value.

                int[] currentIndex = m_bullets[i].GetComponent<BulletActor>().indexOnGrid;
                Vector2Int firstDiff = m_currentPatternSet.patternSet[p].differenceFromOrigin[0];
                Vector2Int secondDiff = m_currentPatternSet.patternSet[p].differenceFromOrigin[1];

                int[] firstIndexDiff = new int[]{
                    currentIndex[0] + firstDiff.x,
                    currentIndex[1] + firstDiff.y
                    };

                int[] secondIndexDiff = new int[]{
                    currentIndex[0] + secondDiff.x,
                    currentIndex[1] + secondDiff.y
                };


                if (firstIndexDiff[0] >= m_gridWidth || firstIndexDiff[1] >= m_gridHeight)
                {
                    continue;
                }
                if (secondIndexDiff[0] >= m_gridWidth || secondIndexDiff[1] >= m_gridHeight)
                {
                    continue;
                }
                if (firstIndexDiff[0] < 0 || firstIndexDiff[1] < 0)
                {
                    continue;
                }
                if (secondIndexDiff[0] < 0 || secondIndexDiff[1] < 0)
                {
                    continue;
                }


                if (m_grid[firstIndexDiff[0], firstIndexDiff[1]].isOccupied && m_grid[secondIndexDiff[0], secondIndexDiff[1]].isOccupied)
                {
                    return true;
                }

            }
        }
        return false;
    }
    private void AttackEnemy(int damage)
    {
        enemy_health -= damage;
        /////////////////////////////
        Enemy_UI.Damage(damage);
        /////////////////////////////
    }
    private int[] WrapIndex(int newXIndex, int newYIndex)
    {
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

        int[] returnValue = new int[2];
        returnValue[0] = newXIndex;
        returnValue[1] = newYIndex;

        return returnValue;
        
    }
    private BulletActor GetLastBullet(BulletActor one, BulletActor two, BulletActor three)
    {
        BulletActor bulletReference;
        for (int i = 0; i < m_bullets.Length; i++)
        {
            bulletReference = m_bullets[i].GetComponent<BulletActor>();
            if(bulletReference == one || bulletReference == two || bulletReference == three)
            {
                continue;

            }
            else
            {
                return bulletReference;
            }
        }
        return null;
    }
    // Idea. 
    // Make ActionPhase() a coroutine and have it wait for bullets to finish moving until its done then move
    // How to get which bullet is moveing?
    // Know which bullet will move in adavance? Add parameter to ActionPhase(BulletActor bullet?)
    // Could loop through bullets and see which one is moving, then from there wait until its in position to finish
    // If so then continue loop? Should work? I'll try the later first.
    private IEnumerator ActionPhase(BulletActor bullet)
    {
        m_actionPhase = true;
        time -= 5;
        int runTime = 0;
        if(bullet != null)
        {
            while(!bullet.inPosition)
            {
                yield return null;
            }
        }
        while(PatternCheckWithoutMovement())
        {
            BulletActor movingBullet = PatternCheck();
            DirectionMove(movingBullet, movingBullet.direction);
            if(movingBullet != null)
            {
                while(!movingBullet.inPosition)
                {
                    yield return null;
                }
            }
            runTime++;
        }
        ResetBulletsPattern();

        for (int i = 0; i < m_bullets.Length; i++)
        {
            SpriteRenderer bulletReference;
            bulletReference = m_bullets[i].GetComponent<SpriteRenderer>();
            bulletReference.color = Color.grey;

        }


        if (runTime > 0)
        {
            BulletActor bulletReference;
            for (int i = 0; i < m_bullets.Length; i++)
            {
                bulletReference = m_bullets[i].GetComponent<BulletActor>();
                DirectionMove(bulletReference, bulletReference.direction);
                if (bulletReference != null)
                {
                    while (!bulletReference.inPosition)
                    {
                        yield return null;
                    }
                }

            }
        }

        for (int i = 0; i < m_bullets.Length; i++)
        {
            SpriteRenderer bulletReference;
            bulletReference = m_bullets[i].GetComponent<SpriteRenderer>();
            bulletReference.color = Color.white;

        }

        StartCoroutine(patternTime());

        m_actionPhase = false;
    }
    private IEnumerator WaitForBullet(BulletActor bullet)
    {
        while(!bullet.inPosition)
        {
            yield return null;
        }
    }
    private BulletActor WhichBulletIsMoving()
    {
        BulletActor bulletReference;
        for (int i = 0; i < m_bullets.Length; i++)
        {
            bulletReference = m_bullets[i].GetComponent<BulletActor>();
            if(!bulletReference.inPosition)
            {
                return bulletReference;
            }

        }
        return null;
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
