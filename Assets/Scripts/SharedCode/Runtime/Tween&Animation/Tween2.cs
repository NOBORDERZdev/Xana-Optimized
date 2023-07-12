using UnityEngine;
using System.Collections;

public class Tween2 : MonoBehaviour {
    public enum TweenType {Position, Eulers, Scale}

    [System.Serializable]
    public class KeyPoint
    {
        public Vector3 position;
        public float moveTime = 1;
    }

    public TweenType type;
    public KeyPoint[] keyPoints = new KeyPoint[] { };
    public bool loop = true;

    void OnEnable()
    {
        if (keyPoints.Length == 0) return;
        StartCoroutine("Move_c");
    }

    void OnDisable()
    {
        StopCoroutine("Move_c");
    }

    IEnumerator Move_c()
    {
        transform.localPosition = keyPoints[0].position;
        int nextPointIndex = 1;
        Interpolate.Position posAnim = null;
        Interpolate.Eulers eulAnim = null;
        Interpolate.Scale scaleAnim = null;
        while (true)
        {
            switch (type)
            {
                case TweenType.Position:
                    posAnim = new Interpolate.Position(transform, transform.localPosition, keyPoints[nextPointIndex].position, keyPoints[nextPointIndex].moveTime, true);
                    break;
                case TweenType.Eulers:
                    eulAnim = new Interpolate.Eulers(transform, transform.localPosition, keyPoints[nextPointIndex].position, keyPoints[nextPointIndex].moveTime, true);
                    break;
                case TweenType.Scale:
                    scaleAnim = new Interpolate.Scale(transform, transform.localPosition, keyPoints[nextPointIndex].position, keyPoints[nextPointIndex].moveTime);
                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(keyPoints[nextPointIndex].moveTime);

            if (posAnim != null) posAnim.Stop();
            if (eulAnim != null) eulAnim.Stop();
            if (scaleAnim != null) scaleAnim.Stop();

            nextPointIndex++;
            if (nextPointIndex >= keyPoints.Length)
            {
                if (loop) nextPointIndex = 0;
                else yield break;
            }
        }
    }
}
