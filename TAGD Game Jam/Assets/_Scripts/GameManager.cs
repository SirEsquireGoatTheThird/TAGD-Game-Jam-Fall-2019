using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This is for the singleton pattern (so the object can be accessed across all stuff)
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    [TextArea]
    [Tooltip("")]
    public string note = "This unity events can be assigned via the inspect and code, we are assigning them via code rather than inspector. Please don't touch it in inspector.";
    public UnityEvent UpdatePatternUI;
    public UnityEvent PatternUsed;
    public UnityEvent UnlockPattern;
    public UnityEvent UpdateTimeDuration;
    public UnityEvent PlayerDamaged;
    public UnityEvent NextEnemy;
    public UnityEvent EnemyAlive;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        if(UpdatePatternUI == null)
        {
            UpdatePatternUI = new UnityEvent();
        }
        if (UpdateTimeDuration == null)
        {
            UpdateTimeDuration = new UnityEvent();
        }
        if(UnlockPattern == null)
        {
            UnlockPattern = new UnityEvent();
        }
        if (PlayerDamaged == null)
        {
            PlayerDamaged = new UnityEvent();
        }
        if (NextEnemy == null)
        {
            NextEnemy = new UnityEvent();
        }
        if (EnemyAlive == null)
        {
            EnemyAlive = new UnityEvent();
        }
    }




}
