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
    public UnityEvent EnemyAttack;
    public UnityEvent PlayerAttackOne;
    public UnityEvent PlayerAttackTwo;
    public UnityEvent PlayerAttackThree;
    public UnityEvent UnlockPattern;


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

        if (PlayerAttackOne == null)
        {
            PlayerAttackOne = new UnityEvent();
        }
        if (PlayerAttackTwo == null)
        {
            PlayerAttackTwo = new UnityEvent();
        }
        if (PlayerAttackThree == null)
        {
            PlayerAttackThree = new UnityEvent();
        }
        if (PatternUsed == null)
        {
            PatternUsed = new UnityEvent();
        }
        if(EnemyAttack == null)
        {
            EnemyAttack = new UnityEvent();
        }
        if(UnlockPattern == null)
        {
            UnlockPattern = new UnityEvent();
        }
    }




}
