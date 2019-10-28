using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pattern Set")]
public class PatternSetScriptable : ScriptableObject
{
    public string setName;
    public PatternScriptable[] patternSet;
    
}
