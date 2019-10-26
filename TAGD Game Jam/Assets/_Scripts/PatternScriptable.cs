using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Pattern")]
public class PatternScriptable : ScriptableObject
{
    public string patternName;
    public Vector2Int[] differenceFromOrigin;
    public Sprite icon;
}
