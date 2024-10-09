using System.Collections;
using TMPro;
using UnityEngine;

public class HyperlinkPanelResizer : MonoBehaviour
{
    Vector3 pos;
    [HideInInspector]
    public Transform target;
    float val;
    Camera cam;

    [SerializeField] private RectTransform viewportRectT;
    [SerializeField] private TextMeshProUGUI text;
    private int rightPosition = 23;
    private int bottomPosition = -9;

    private void OnEnable()
    {
        StartCoroutine(CheckJapaneseRoutine());
    }

    private IEnumerator CheckJapaneseRoutine()
    {
        yield return new WaitForSeconds(0.15f); //Wait for the text to be set
        switch (LocalizationManager._instance.IsJapanese(text.text))
        {
            case false:
                viewportRectT.offsetMin = new Vector2(0, 0);
                viewportRectT.offsetMax = new Vector2(0, 0);
                break;
            case true:
                viewportRectT.offsetMin = new Vector2(viewportRectT.offsetMin.x, bottomPosition);
                viewportRectT.offsetMax = new Vector2(-rightPosition, viewportRectT.offsetMax.y);
                break;
        }
        StopCoroutine(CheckJapaneseRoutine());
    }

    private void OnDisable()
    {
        cam = null;
        target = null;
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (GamificationComponentData.instance.playerControllerNew == null)
            return;

        if (target == null)
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
        if (distance > 3.5f)
        {
            val = 1 - (0.5f / 27) * (distance - 3.5f);
            val = Mathf.Clamp(val, 0, 1);
            transform.localScale = Vector3.one * val;
        }
        else
            transform.localScale = Vector3.one;
        transform.position = pos;
    }
}
