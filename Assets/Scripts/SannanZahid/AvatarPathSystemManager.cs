using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPathSystemManager : MonoBehaviour
{
    [SerializeField]
    Transform _startPoint;
    //[SerializeField]
    int _row , _col;
    Transform[,] points;
    float offset=1;
    void Awake()
    {
        _row = 8;
        _col = 3;
        points = new Transform[_row, _col];
        GeneratePoints();
    }
    public Transform GetStartPoint()
    {
        return _startPoint;
    }
   void GeneratePoints()
    {
        Transform tempTransform;
        for (int i = 0; i < _row; i++)
            for (int j = 0; j < _col; j++)
            {
                if(j%2==0)
                {
                    tempTransform = Instantiate(_startPoint.gameObject,
                  new Vector3((float)i / 1.2f + _startPoint.position.x, 0, (float)-j / .9f + _startPoint.position.z),
                  Quaternion.identity).transform;
                }
                else
                {
                    tempTransform = Instantiate(_startPoint.gameObject,
                  new Vector3((float)(i+0.5) / 1.4f + _startPoint.position.x, 0, (float)-j / .9f + _startPoint.position.z),
                  Quaternion.identity).transform;
                }
                points[i,j]=(tempTransform);
                tempTransform.name = "{ " + i + " - " + j+" }";
                tempTransform.gameObject.GetComponent<ActorMovePoint>().Init( new Vector2(i,j) );
            }
        ConnectPoints(points);
    }
    void ConnectPoints(Transform[,] points)
    {
        ActorMovePoint tempPoint;
        for (int i = 0; i < _row; i++)
            for (int j = 0; j < _col; j++)
            {
                tempPoint = points[i,j].GetComponent<ActorMovePoint>();
                if (j - 1 >= 0) tempPoint.LeftPoint = points[i ,j - 1];
                if (j + 1 <= _col - 1) tempPoint.RightPoint = points[i ,j + 1];
                if (i - 1 >= 0) tempPoint.UpPoint = points[i - 1, j];
                if (i + 1 <= _row - 1) tempPoint.DownPoint = points[i + 1, j];
            }
    }
    ActorMovePoint tempActorMovePoint;
    public Transform GetNextPoint(MoveBehaviour.Behaviour behaviour,Transform CurrentTarget)
    {
        int count = 0;
        while(count <= 4)
        {
            if (MoveBehaviour.Behaviour.Left == behaviour)
            {
                if (CurrentTarget.GetComponent<ActorMovePoint>().LeftPoint != null)
                {
                    tempActorMovePoint = CurrentTarget.GetComponent<ActorMovePoint>().LeftPoint.GetComponent<ActorMovePoint>();
                    if (!tempActorMovePoint.IsInUse)
                    {
                        ClearFlags(tempActorMovePoint, CurrentTarget.GetComponent<ActorMovePoint>());
                        return tempActorMovePoint.transform;
                    }
                }
                behaviour = GetRandomBehaviour();
                count++;
            }
            if (MoveBehaviour.Behaviour.Right == behaviour)
            {
                if (CurrentTarget.GetComponent<ActorMovePoint>().RightPoint != null)
                {
                    tempActorMovePoint = CurrentTarget.GetComponent<ActorMovePoint>().RightPoint.GetComponent<ActorMovePoint>();
                    if (!tempActorMovePoint.IsInUse)
                    {
                        ClearFlags(tempActorMovePoint, CurrentTarget.GetComponent<ActorMovePoint>());
                        return tempActorMovePoint.transform;
                    }
                }
                behaviour = GetRandomBehaviour();
                count++;
            }
            if (MoveBehaviour.Behaviour.Up == behaviour)
            {
                if (CurrentTarget.GetComponent<ActorMovePoint>().UpPoint != null)
                {
                    tempActorMovePoint = CurrentTarget.GetComponent<ActorMovePoint>().UpPoint.GetComponent<ActorMovePoint>();
                    if (!tempActorMovePoint.IsInUse)
                    {
                        ClearFlags(tempActorMovePoint, CurrentTarget.GetComponent<ActorMovePoint>());
                        return tempActorMovePoint.transform;
                    }
                }
                behaviour = GetRandomBehaviour();
                count++;
            }
            if (MoveBehaviour.Behaviour.Down == behaviour)
            {
                if (CurrentTarget.GetComponent<ActorMovePoint>().DownPoint != null)
                {
                    tempActorMovePoint = CurrentTarget.GetComponent<ActorMovePoint>().DownPoint.GetComponent<ActorMovePoint>();
                    if (!tempActorMovePoint.IsInUse)
                    {
                        ClearFlags(tempActorMovePoint, CurrentTarget.GetComponent<ActorMovePoint>());
                        return tempActorMovePoint.transform;
                    }
                }
                behaviour = GetRandomBehaviour();
                count++;
            }
        }
        return CurrentTarget;
    }
    public void ClearFlags(ActorMovePoint sendPoint, ActorMovePoint previousPoint)
    {
        sendPoint.IsInUse = true;
        previousPoint.IsInUse = false;
    }
    public MoveBehaviour.Behaviour GetRandomBehaviour()
    {
        while (true)
        {
            int i = UnityEngine.Random.Range(0, 4);
            switch (i)
            {
                case 0:
                    return MoveBehaviour.Behaviour.Left;
                case 1:
                    return MoveBehaviour.Behaviour.Right;
                case 2:
                    return MoveBehaviour.Behaviour.Up;
                case 3:
                    return MoveBehaviour.Behaviour.Down;
            }
        }
    }
      public float threshold = 1.0f;
    public Transform GetAvatarSpawnPoint()
    {
        
        List<Transform> nearPoints = new List<Transform>();
        foreach (Transform point in points)
        {
            if (Vector3.Distance(point.position, points[3,1].position) < threshold)
            {
                nearPoints.Add(point);
            }
        }
        Transform transform = null;
        foreach (var point in nearPoints)
        {
            if (!point.GetComponent<ActorMovePoint>().IsInUse)
            {
                point.GetComponent<ActorMovePoint>().IsInUse = true;
                transform= point;
            }
        }

        if (transform!=null)
        {
            return transform;
        }
        else
        {
            while (true)
            {
                int i = UnityEngine.Random.Range(0, _row);
                int j = UnityEngine.Random.Range(0, _col);
                if (!points[i, j].GetComponent<ActorMovePoint>().IsInUse)
                {
                    points[i, j].GetComponent<ActorMovePoint>().IsInUse = true;
                    return points[i, j];
                }
            }
        }

    }

    public Transform  GetGridCenterPoint()
    {
        //int centerRow = Mathf.FloorToInt( _row / 2);
        //int centerCol =  Mathf.FloorToInt(_col / 2);
        //if (!points[centerRow, centerCol].GetComponent<ActorMovePoint>().IsInUse)
        //{
        //    points[centerRow, centerCol].GetComponent<ActorMovePoint>().IsInUse = true;
        //    print("centerRow : "+ centerRow +"centerCol"+centerCol);
        //    return points[centerRow, centerCol];
        //}
        //else
        //{
        //    return points[0, 0];
        //}
        points[3, 1].GetComponent<ActorMovePoint>().IsInUse = true;
        return points[3,1];
    }
}
[Serializable]
public class MoveBehaviour
{
    public enum Behaviour 
    {
        Left,
        Right,
        Up,
        Down,
        Action
    };
    public Behaviour behaviour;
    public float Speed = 0f;
}