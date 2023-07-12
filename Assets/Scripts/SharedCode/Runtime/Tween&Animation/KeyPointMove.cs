using UnityEngine;
using System.Collections;

public class KeyPointMove : MonoBehaviour {
    [System.Serializable]
    public class Point {
        public Vector3 position;
        public float moveTime=1;
    }

    public Point[] points = new Point[] { };
    public bool loop=true;

    void OnEnable() {
        if (points.Length == 0) return;
        StartCoroutine("Move_c");
    }

    void OnDisable()
    {
        StopCoroutine("Move_c");
    }

    IEnumerator Move_c() {
        transform.localPosition = points[0].position;
        int nextPointIndex = 1;
        Interpolate.Position anim;
        while (true)
        {
            anim = new Interpolate.Position(transform, transform.localPosition, points[nextPointIndex].position, points[nextPointIndex].moveTime, true);
            yield return new WaitForSeconds(points[nextPointIndex].moveTime);
            anim.Stop();
            nextPointIndex++;
            if (nextPointIndex >= points.Length) {
                if (loop) nextPointIndex = 0;
                else yield break;
            }
        }
    }
}
