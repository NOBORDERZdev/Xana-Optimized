using Jint.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class SplineDone : MonoBehaviour {

    public static SplineDone Instance;
  

    public event EventHandler OnDirty;

    [SerializeField] private Transform _dots = null;
    [SerializeField] private Vector3 _normal = new Vector3(0, 0, -1);
    [SerializeField] private bool _closedLoop;
    [SerializeField] private List<Anchor> _anchorList;

    private float _moveDistance;
    private float _pointAmountInCurve;
    private float _pointAmountPerUnitInCurve = 2f;

    private static readonly Vector3 _normal2D = new Vector3(0, 0, -1f);
    private List<Point> _pointList;
    private float _splineLength;
    float _previoust = 0;
    Dictionary<float, Vector3> _positioUnit = new Dictionary<float, Vector3>();
    Dictionary<float, Vector3> _postionAtT = new Dictionary<float, Vector3>();
    Dictionary<Vector3, float> _distanceatTpos = new Dictionary<Vector3, float>();
    Dictionary<float, float> _totalDistance = new Dictionary<float, float>();


    private void Awake() {
        Instance = this;
        _splineLength = GetSplineLength(0.001f);
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
            if (_closedLoop) {
                anchorA = _anchorList[_anchorList.Count - 1];
                anchorB = _anchorList[0];
            } else {
                anchorA = _anchorList[_anchorList.Count - 2];
                anchorB = _anchorList[_anchorList.Count - 1];
            }
            return transform.position + CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, t);
        } else {
            int addClosedLoop = (_closedLoop ? 1 : 0);
            float tFull = t * (_anchorList.Count - 1 + addClosedLoop);
            int anchorIndex = Mathf.FloorToInt(tFull);
            float tAnchor = tFull - anchorIndex;

            Anchor anchorA, anchorB;

            if (anchorIndex < _anchorList.Count - 1) {
                anchorA = _anchorList[anchorIndex + 0];
                anchorB = _anchorList[anchorIndex + 1];
            } else {
                // anchorIndex is final one, either don't link to "next" one or loop back
                if (_closedLoop) {
                    anchorA = _anchorList[_anchorList.Count - 1];
                    anchorB = _anchorList[0];
                } else {
                    anchorA = _anchorList[anchorIndex - 1];
                    anchorB = _anchorList[anchorIndex + 0];
                    tAnchor = 1f;
                }
            }

            return transform.position + CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, tAnchor);
        }
    }

    public Vector3 GetForwardAt(float t) {
        Point pointA = GetPreviousPoint(t);

        int pointBIndex;

        pointBIndex = (_pointList.IndexOf(pointA) + 1) % _pointList.Count;
        Point pointB = _pointList[pointBIndex];

        return Vector3.Lerp(pointA.forward, pointB.forward, (t - pointA.t) / Mathf.Abs(pointA.t - pointB.t));
    }

    public Point GetPreviousPoint(float t) {
        int previousIndex = 0;
        for (int i=1; i<_pointList.Count; i++) {
            Point point = _pointList[i];
            if (t < point.t) {
                return _pointList[previousIndex];
            } else {
                previousIndex++;
            }
        }
        return _pointList[previousIndex];
    }

    public Point GetClosestPoint(float t) {
        Point closestPoint = _pointList[0];
        foreach (Point point in _pointList) {
            if (Mathf.Abs(t - point.t) < Mathf.Abs(t - closestPoint.t)) {
                closestPoint = point;
            }
        }
        return closestPoint;
    }
 
    public Vector3 GetPositionAtUnits(float unitDistance,SplineFollower CAR, float stepSize = .0005f) {
       
        
        float splineUnitDistance = 0f;
        
        Vector3 lastPosition = GetPositionAt(0f);
        if (_previoust >= 0) { _previoust = 0; }
        float incrementAmount = stepSize;
        var t = CAR._carT;
        while (CAR._carT<1) {
        
            if(!_postionAtT.TryGetValue(t, out var tpos)) return Vector3.zero;

            _totalDistance.TryGetValue(t, out splineUnitDistance);

            lastPosition = tpos;

            if (splineUnitDistance >= unitDistance) {
                /*
                float remainingDistance = _splineUnitDistance - unitDistance;
                Debug.Log(remainingDistance + " " + unitDistance + " " + _splineUnitDistance + " " + t);
                Debug.Log(t - (remainingDistance / _splineLength));
                return GetPositionAt(t - (remainingDistance / _splineLength));
                */
                
                CAR._splineUnitDistance = splineUnitDistance;
                CAR._lastPosition = lastPosition;
                Vector3 direction = (tpos - GetPositionAt(t - incrementAmount)).normalized;
                _previoust = t;
                Vector3 pos = tpos + direction * (unitDistance - splineUnitDistance);
                _positioUnit.TryAdd(unitDistance, pos);
                return pos;
            }
            CAR._carT = t;
            t += incrementAmount;
        }
        Debug.Log("HERE..........");
        // Default
        Anchor anchorA = _anchorList[0];
        Anchor anchorB = _anchorList[1];
        return CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, unitDistance / _splineLength);
    }

    public Vector3 GetForwardAtUnits(float unitDistance,SplineFollower CAR ,float stepSize = .0005f) {
        float splineUnitDistance = 0f;

        Vector3 lastPosition = GetPositionAt(0f);

        float incrementAmount = stepSize;
        float lastDistance = 0f;
        var t = CAR._carTF;
        while (CAR._carT < 1)
        {

            if (!_postionAtT.TryGetValue(t, out var tpos)) return Vector3.zero;

            _totalDistance.TryGetValue(t, out splineUnitDistance); 
            lastDistance = Vector3.Distance(lastPosition, tpos);
         //   _splineUnitDistance += lastDistance;

            lastPosition = tpos;

            if (splineUnitDistance >= unitDistance) {
                float remainingDistance = splineUnitDistance - unitDistance;
                return GetForwardAt(t - ((remainingDistance / lastDistance) * incrementAmount));
            }
            CAR._carTF = t;
            t += incrementAmount;
        }

        // Default
        Anchor anchorA = _anchorList[0];
        Anchor anchorB = _anchorList[1];
        return CubicLerp(anchorA.position, anchorA.handleBPosition, anchorB.handleAPosition, anchorB.position, unitDistance / _splineLength);
    }

    private void SetupPointList() {
        _pointList = new List<Point>();
        _pointAmountInCurve = _pointAmountPerUnitInCurve * _splineLength;
        for (float t = 0; t < 1f; t += 1f / _pointAmountInCurve) {
            _pointList.Add(new Point {
                t = t,
                position = GetPositionAt(t),
                normal = _normal,
            });
        }

        _pointList.Add(new Point {
            t = 1f,
            position = GetPositionAt(1f),
        });

        UpdateForwardVectors();
    }

    private void UpdatePointList() {
        if (_pointList == null) return;

        foreach (Point point in _pointList) {
            point.position = GetPositionAt(point.t);
        }
        
        UpdateForwardVectors();
    }

    private void UpdateForwardVectors() {
        // Set forward vectors
        for (int i = 0; i < _pointList.Count - 1; i++) {
            _pointList[i].forward = (_pointList[i + 1].position - _pointList[i].position).normalized;
        }
        // Set final forward vector
        if (_closedLoop) {
            _pointList[_pointList.Count - 1].forward = _pointList[0].forward;
        } else {
            _pointList[_pointList.Count - 1].forward = _pointList[_pointList.Count - 2].forward;
        }
    }

    private void PrintPath() {
        foreach (Point point in _pointList) {
            Transform dotTransform = Instantiate(_dots, point.position, Quaternion.identity);
            FunctionUpdater.Create(() => {
                dotTransform.position = point.position;
            });
        }
    }

  
    public float GetSplineLength(float stepSize = .0005f) 
    {
        float splineLength = 0f;

        Vector3 lastPosition = GetPositionAt(0f);

        for (float t = 0; t < 1f; t += stepSize) {

            Vector3 tpos = Vector3.zero;

            if (!_postionAtT.TryGetValue(t, out tpos))
            {
                tpos = GetPositionAt(t);
            }
            float distance = Vector3.Distance(lastPosition, tpos);
            splineLength += distance;

            lastPosition =tpos;
            _postionAtT.TryAdd(t, tpos);
            _distanceatTpos.TryAdd(tpos, distance);
            _totalDistance.TryAdd(t, splineLength);

        }

        splineLength += Vector3.Distance(lastPosition, GetPositionAt(1f));

        return splineLength;
    }

    public List<Anchor> GetAnchorList() {
        return _anchorList;
    }

    public void AddAnchor() {
        if (_anchorList == null) _anchorList = new List<Anchor>();
        if (_anchorList.Count == 0)
        {
            _anchorList.Add(new Anchor
            {
                position =  new Vector3(0, 1, 0),
                handleAPosition =  new Vector3(1, 1, 0),
                handleBPosition =  new Vector3(0, 1, 1),
            });
            return;
        }
        Anchor lastAnchor = _anchorList[_anchorList.Count - 1];
        _anchorList.Add(new Anchor {
            position = lastAnchor.position + new Vector3(1, 1, 0),
            handleAPosition = lastAnchor.handleAPosition + new Vector3(1, 1, 0),
            handleBPosition = lastAnchor.handleBPosition + new Vector3(1, 1, 0),
        });
    }

    public void RemoveLastAnchor() {
        if (_anchorList == null) _anchorList = new List<Anchor>();

        _anchorList.RemoveAt(_anchorList.Count - 1);
    }


    public List<Point> GetPointList() {
        return _pointList;
    }

    public bool GetClosedLoop() {
        return _closedLoop;
    }

    public void SetAllZZero() {
        foreach (Anchor anchor in _anchorList) {
            anchor.position = new Vector3(anchor.position.x, anchor.position.y, 0f);
            anchor.handleAPosition = new Vector3(anchor.handleAPosition.x, anchor.handleAPosition.y, 0f);
            anchor.handleBPosition = new Vector3(anchor.handleBPosition.x, anchor.handleBPosition.y, 0f);
        }
    }

    public void SetAllYZero() {
        foreach (Anchor anchor in _anchorList) {
            anchor.position = new Vector3(anchor.position.x, 0f, anchor.position.z);
            anchor.handleAPosition = new Vector3(anchor.handleAPosition.x, 0f, anchor.handleAPosition.z);
            anchor.handleBPosition = new Vector3(anchor.handleBPosition.x, 0f, anchor.handleBPosition.z);
        }
    }
    public void SetAllYOne()
    {
        foreach (Anchor anchor in _anchorList)
        {
            anchor.position = new Vector3(anchor.position.x, 1f, anchor.position.z);
            anchor.handleAPosition = new Vector3(anchor.handleAPosition.x, 1f, anchor.handleAPosition.z);
            anchor.handleBPosition = new Vector3(anchor.handleBPosition.x, 1f, anchor.handleBPosition.z);
        }
    }
    public void SetDirty() {
        _splineLength = GetSplineLength(0.001f);
        SetupPointList();
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

