using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpButtonComponentResizer : MonoBehaviour
{
    Vector3 pos;
    public Transform target;
    public Camera cam;
    public bool isAlwaysOn = true;
    public TextMeshProUGUI titleText, contentText;
    public ScrollRect scrollView;
    public GameObject scrollbar;
    float val;
    void Update()
    {
        if (GamificationComponentData.instance.playerControllerNew == null)
            return;
        if (cam == null)
            cam = GamificationComponentData.instance.playerControllerNew.ActiveCamera.GetComponent<Camera>();

        if (!isAlwaysOn)
        {
            pos = transform.position;
            pos.x = cam.WorldToScreenPoint(target.position).x;
            float distance = Vector3.Distance(GamificationComponentData.instance.playerControllerNew.transform.position, GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform.position);
            if (distance > 3)
            {
                val = 0.5f - (0.5f / 27) * (distance - 3);
                val = Mathf.Clamp(val, 0, 1);
                transform.localScale = Vector3.one * val;
            }
            transform.position = pos;
        }
        else
        {
            transform.LookAt(GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform);
            transform.Rotate(new Vector3(0, 1, 0), 180f);
        }

    }
}