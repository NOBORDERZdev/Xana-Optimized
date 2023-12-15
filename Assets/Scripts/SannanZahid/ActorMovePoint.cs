using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovePoint : MonoBehaviour
{
    public Transform LeftPoint, RightPoint, UpPoint, DownPoint;
    public bool IsInUse = false;
    public Vector2 GridPoint;
    public void Init(Vector2 gridPoint)
    {
        GridPoint = gridPoint;
    }
}
