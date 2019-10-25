using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    Vector2Int direction { get; set; }
    Vector3 position { get; set; }
    int[] indexOnGrid { get; set; }

    int order { get; set; }

    bool inPattern { get; set; }
}
