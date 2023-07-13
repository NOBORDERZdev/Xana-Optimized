using System.Collections;
using UnityEngine;

public class HyperlinkPanelResizer : MonoBehaviour
{
    Vector3 pos;
    [HideInInspector]
    public Transform target;
    Camera cam;

    private void OnDisable()
    {
        cam = null;
    }
    float val;
    void Update()
    {
        if (GamificationComponentData.instance.playerControllerNew == null)
            return;
        if (cam == null)
            cam = GamificationComponentData.instance.playerControllerNew.ActiveCamera.GetComponent<Camera>();
        pos = transform.position;
        pos.x = cam.WorldToScreenPoint(target.position).x;
        //if (tpc.defaultDistance > 2.9f)
        //{
        //    val = 0.5f - (0.5f / 27) * (tpc.defaultDistance - 3);
        //    val = Mathf.Clamp(val, 0, 1);
        //    transform.localScale = new Vector3(val, val, val);
        //}
        float distance = Vector3.Distance(GamificationComponentData.instance.playerControllerNew.transform.position, GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform.position);
        if (distance > 3)
        {
            val = 0.5f - (0.5f / 27) * (distance - 3);
            val = Mathf.Clamp(val, 0, 1);
            transform.localScale = Vector3.one * val;
        }

        transform.position = pos;
    }
}
