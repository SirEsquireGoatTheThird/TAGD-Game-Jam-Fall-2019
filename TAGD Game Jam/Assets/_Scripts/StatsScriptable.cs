using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Character Stats")]
public class StatsScriptable : ScriptableObject
{
    public int health;
    public int damage;
    public int attackTimer;
    public Sprite icon;
}
