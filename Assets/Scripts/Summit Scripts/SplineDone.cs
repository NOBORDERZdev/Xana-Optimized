using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDone : MonoBehaviour {

    public static SplineDone Instance;
    private static readonly Vector3 normal2D = new Vector3(0, 0, -1f);

    public event EventHandler OnDirty;

    [SerializeField] private Transform dots = null;
    [SerializeField] private Vector3 normal = new Vector3(0, 0, -1);
    [SerializeField] private bool closedLoop;
    [SerializeField] private List<Anchor> anchorList;

    private float moveDistance;
    private float pointAmountInCurve;
    private float pointAmountPerUnitInCurve = 2f;


    private List<Point> pointList;
    private float splineLength;

    private void Awake() {
        Instance = this;
        splineLength = GetSplineLength(0.001f);
        SetupPointList();
    }

    private void Start() {
        //PrintPath();
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t) {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
        Vector3 abc = QuadraticLerp(a, b, c, t);
        Vector3 bcd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(abc, bcd, t);
    }

    public Vector3 GetPositionAt(float t) {
        if (t == 1) {
            // Full position, special case
            Anchor anchorA, anchorB;
            if (closedLoop) {
                anchorA = anchorList[anchorList.Count - 1];
                anchorB = anchorList[0];
            } else {
                anchorA = anchorList[anchorList.Count - 2];
                anchorB = anchorList[anchorList.Count - 1];
            }
            return transform.position + CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, t);
        } else {
            int addClosedLoop = (closedLoop ? 1 : 0);
            float tFull = t * (anchorList.Count - 1 + addClosedLoop);
            int anchorIndex = Mathf.FloorToInt(tFull);
            float tAnchor = tFull - anchorIndex;

            Anchor anchorA, anchorB;

            if (anchorIndex < anchorList.Count - 1) {
                anchorA = anchorList[anchorIndex + 0];
                anchorB = anchorList[anchorIndex + 1];
            } else {
                // anchorIndex is final one, either don't link to "next" one or loop back
                if (closedLoop) {
                    anchorA = anchorList[anchorList.Count - 1];
                    anchorB = anchorList[0];
                } else {
                    anchorA = anchorList[anchorIndex - 1];
                    anchorB = anchorList[anchorIndex + 0];
                    tAnchor = 1f;
                }
            }

            return transform.position + CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, tAnchor);
        }
    }

    public Vector3 GetForwardAt(float t) {
        Point pointA = GetPreviousPoint(t);

        int pointBIndex;

        pointBIndex = (pointList.IndexOf(pointA) + 1) % pointList.Count;
        Point pointB = pointList[pointBIndex];

        return Vector3.Lerp(pointA.forward, pointB.forward, (t - pointA.t) / Mathf.Abs(pointA.t - pointB.t));
    }

    public Point GetPreviousPoint(float t) {
        int previousIndex = 0;
        for (int i=1; i<pointList.Count; i++) {
            Point point = pointList[i];
            if (t < point.t) {
                return pointList[previousIndex];
            } else {
                previousIndex++;
            }
        }
        return pointList[previousIndex];
    }

    public Point GetClosestPoint(float t) {
        Point closestPoint = pointList[0];
        foreach (Point point in pointList) {
            if (Mathf.Abs(t - point.t) < Mathf.Abs(t - closestPoint.t)) {
                closestPoint = point;
            }
        }
        return closestPoint;
    }

    public Vector3 GetPositionAtUnits(float unitDistance, float stepSize = .0001f) {
        float splineUnitDistance = 0f;

        Vector3 lastPosition = GetPositionAt(0f);

        float incrementAmount = stepSize;
        for (float t = 0; t < 1f; t += incrementAmount) {
            splineUnitDistance += Vector3.Distance(lastPosition, GetPositionAt(t));

            lastPosition = GetPositionAt(t);

            if (splineUnitDistance >= unitDistance) {
                /*
                float remainingDistance = splineUnitDistance - unitDistance;
                Debug.Log(remainingDistance + " " + unitDistance + " " + splineUnitDistance + " " + t);
                Debug.Log(t - (remainingDistance / splineLength));
                return GetPositionAt(t - (remainingDistance / splineLength));
                */
                Vector3 direction = (GetPositionAt(t) - GetPositionAt(t - incrementAmount)).normalized;
                return GetPositionAt(t) + direction * (unitDistance - splineUnitDistance);
            }
        }

        // Default
        Anchor anchorA = anchorList[0];
        Anchor anchorB = anchorList[1];
        return CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, unitDistance / splineLength);
    }

    public Vector3 GetForwardAtUnits(float unitDistance, float stepSize = .0001f) {
        float splineUnitDistance = 0f;

        Vector3 lastPosition = GetPositionAt(0f);

        float incrementAmount = stepSize;
        float lastDistance = 0f;

        for (float t = 0; t < 1f; t += incrementAmount) {
            lastDistance = Vector3.Distance(lastPosition, GetPositionAt(t));
            splineUnitDistance += lastDistance;

            lastPosition = GetPositionAt(t);

            if (splineUnitDistance >= unitDistance) {
                float remainingDistance = splineUnitDistance - unitDistance;
                return GetForwardAt(t - ((remainingDistance / lastDistance) * incrementAmount));
            }

        }

        // Default
        Anchor anchorA = anchorList[0];
        Anchor anchorB = anchorList[1];
        return CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, unitDistance / splineLength);
    }

    private void SetupPointList() {
        pointList = new List<Point>();
        pointAmountInCurve = pointAmountPerUnitInCurve * splineLength;
        for (float t = 0; t < 1f; t += 1f / pointAmountInCurve) {
            pointList.Add(new Point {
                t = t,
                position = GetPositionAt(t),
                normal = normal,
            });
        }

        pointList.Add(new Point {
            t = 1f,
            position = GetPositionAt(1f),
        });

        UpdateForwardVectors();
    }

    private void UpdatePointList() {
        if (pointList == null) return;

        foreach (Point point in pointList) {
            point.position = GetPositionAt(point.t);
        }
        
        UpdateForwardVectors();
    }

    private void UpdateForwardVectors() {
        // Set forward vectors
        for (int i = 0; i < pointList.Count - 1; i++) {
            pointList[i].forward = (pointList[i + 1].position - pointList[i].position).normalized;
        }
        // Set final forward vector
        if (closedLoop) {
            pointList[pointList.Count - 1].forward = pointList[0].forward;
        } else {
            pointList[pointList.Count - 1].forward = pointList[pointList.Count - 2].forward;
        }
    }

    private void PrintPath() {
        foreach (Point point in pointList) {
            Transform dotTransform = Instantiate(dots, point.position, Quaternion.identity);
            FunctionUpdater.Create(() => {
                dotTransform.position = point.position;
            });
        }
    }

    public float GetSplineLength(float stepSize = .01f) {
        float splineLength = 0f;

        Vector3 lastPosition = GetPositionAt(0f);

        for (float t = 0; t < 1f; t += stepSize) {
            splineLength += Vector3.Distance(lastPosition, GetPositionAt(t));

            lastPosition = GetPositionAt(t);
        }

        splineLength += Vector3.Distance(lastPosition, GetPositionAt(1f));

        return splineLength;
    }

    public List<Anchor> GetAnchorList() {
        return anchorList;
    }

    public void AddAnchor() {
        if (anchorList == null) anchorList = new List<Anchor>();
        if (anchorList.Count == 0)
        {
            anchorList.Add(new Anchor
            {
                position =  new Vector3(0, 1, 0),
                handleAPosition =  new Vector3(1, 1, 0),
                handleBPosition =  new Vector3(0, 1, 1),
            });
            return;
        }
        Anchor lastAnchor = anchorList[anchorList.Count - 1];
        anchorList.Add(new Anchor {
            position = lastAnchor.position + new Vector3(1, 1, 0),
            handleAPosition = lastAnchor.handleAPosition + new Vector3(1, 1, 0),
            handleBPosition = lastAnchor.handleBPosition + new Vector3(1, 1, 0),
        });
    }

    public void RemoveLastAnchor() {
        if (anchorList == null) anchorList = new List<Anchor>();

        anchorList.RemoveAt(anchorList.Count - 1);
    }


    public List<Point> GetPointList() {
        return pointList;
    }

    public bool GetClosedLoop() {
        return closedLoop;
    }

    public void SetAllZZero() {
        foreach (Anchor anchor in anchorList) {
            anchor.position = new Vector3(anchor.position.x, anchor.position.y, 0f);
            anchor.handleAPosition = new Vector3(anchor.handleAPosition.x, anchor.handleAPosition.y, 0f);
            anchor.handleBPosition = new Vector3(anchor.handleBPosition.x, anchor.handleBPosition.y, 0f);
        }
    }

    public void SetAllYZero() {
        foreach (Anchor anchor in anchorList) {
            anchor.position = new Vector3(anchor.position.x, 0f, anchor.position.z);
            anchor.handleAPosition = new Vector3(anchor.handleAPosition.x, 0f, anchor.handleAPosition.z);
            anchor.handleBPosition = new Vector3(anchor.handleBPosition.x, 0f, anchor.handleBPosition.z);
        }
    }
    public void SetAllYOne()
    {
        foreach (Anchor anchor in anchorList)
        {
            anchor.position = new Vector3(anchor.position.x, 1f, anchor.position.z);
            anchor.handleAPosition = new Vector3(anchor.handleAPosition.x, 1f, anchor.handleAPosition.z);
            anchor.handleBPosition = new Vector3(anchor.handleBPosition.x, 1f, anchor.handleBPosition.z);
        }
    }
    public void SetDirty() {
        splineLength = GetSplineLength(0.001f);

        UpdatePointList();

        OnDirty?.Invoke(this, EventArgs.Empty);
    }


    [Serializable]
    public class Point {
        public float t;
        public Vector3 position;
        public Vector3 forward;
        public Vector3 normal;
    }

    [Serializable]
    public class Anchor {
        public Vector3 position;
        public Vector3 handleAPosition;
        public Vector3 handleBPosition;
    }

}

public class FunctionUpdater
{

    /*
     * Class to hook Actions into MonoBehaviour
     * */
    private class MonoBehaviourHook : MonoBehaviour
    {

        public Action OnUpdate;

        private void Update()
        {
            if (OnUpdate != null) OnUpdate();
        }

    }

    private static List<FunctionUpdater> updaterList; // Holds a reference to all active updaters
    private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FunctionUpdater_Global");
            updaterList = new List<FunctionUpdater>();
        }
    }




    public static FunctionUpdater Create(Action updateFunc)
    {
        return Create(() => { updateFunc(); return false; }, "", true, false);
    }
    public static FunctionUpdater Create(Func<bool> updateFunc)
    {
        return Create(updateFunc, "", true, false);
    }
    public static FunctionUpdater Create(Func<bool> updateFunc, string functionName)
    {
        return Create(updateFunc, functionName, true, false);
    }
    public static FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active)
    {
        return Create(updateFunc, functionName, active, false);
    }
    public static FunctionUpdater Create(Func<bool> updateFunc, string functionName, bool active, bool stopAllWithSameName)
    {
        InitIfNeeded();

        if (stopAllWithSameName)
        {
            StopAllUpdatersWithName(functionName);
        }

        GameObject gameObject = new GameObject("FunctionUpdater Object " + functionName, typeof(MonoBehaviourHook));
        FunctionUpdater functionUpdater = new FunctionUpdater(gameObject, updateFunc, functionName, active);
        gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = functionUpdater.Update;

        updaterList.Add(functionUpdater);
        return functionUpdater;
    }
    private static void RemoveUpdater(FunctionUpdater funcUpdater)
    {
        InitIfNeeded();
        updaterList.Remove(funcUpdater);
    }
    public static void DestroyUpdater(FunctionUpdater funcUpdater)
    {
        InitIfNeeded();
        if (funcUpdater != null)
        {
            funcUpdater.DestroySelf();
        }
    }
    public static void StopUpdaterWithName(string functionName)
    {
        InitIfNeeded();
        for (int i = 0; i < updaterList.Count; i++)
        {
            if (updaterList[i].functionName == functionName)
            {
                updaterList[i].DestroySelf();
                return;
            }
        }
    }
    public static void StopAllUpdatersWithName(string functionName)
    {
        InitIfNeeded();
        for (int i = 0; i < updaterList.Count; i++)
        {
            if (updaterList[i].functionName == functionName)
            {
                updaterList[i].DestroySelf();
                i--;
            }
        }
    }





    private GameObject gameObject;
    private string functionName;
    private bool active;
    private Func<bool> updateFunc; // Destroy Updater if return true;

    public FunctionUpdater(GameObject gameObject, Func<bool> updateFunc, string functionName, bool active)
    {
        this.gameObject = gameObject;
        this.updateFunc = updateFunc;
        this.functionName = functionName;
        this.active = active;
    }
    public void Pause()
    {
        active = false;
    }
    public void Resume()
    {
        active = true;
    }

    private void Update()
    {
        if (!active) return;
        if (updateFunc())
        {
            DestroySelf();
        }
    }
    public void DestroySelf()
    {
        RemoveUpdater(this);
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}

