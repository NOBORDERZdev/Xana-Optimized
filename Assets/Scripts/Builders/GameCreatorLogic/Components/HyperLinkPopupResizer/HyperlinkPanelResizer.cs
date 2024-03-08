using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputManagerEntry;

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
    public bool isJP = false;

    private void OnEnable()
    {
        StartCoroutine(CheckJapaneseRoutine());
    }

    private IEnumerator CheckJapaneseRoutine()
    {
        yield return new WaitForSeconds(1f); //Wait for the text to be set
        switch (IsJapanese(text.text))
        {
            case false:
                viewportRectT.offsetMin = new Vector2(0, 0);
                viewportRectT.offsetMax = new Vector2(0, 0);
                isJP = false;
                break;
            case true:
                viewportRectT.offsetMin = new Vector2(viewportRectT.offsetMin.x, bottomPosition);
                viewportRectT.offsetMax = new Vector2(-rightPosition, viewportRectT.offsetMax.y);
                isJP = true;
                break;
        }
        StopCoroutine(CheckJapaneseRoutine());

    }

    private bool IsJapanese(string text) //Detect Japanese characters
    {
        int count = 0;
        foreach (char c in text)
        {
            // Check if the character is in the Japanese Hiragana, Katakana, or Kanji ranges
            if ((c >= '\u3040' && c <= '\u309F') || // Hiragana
                (c >= '\u30A0' && c <= '\u30FF') || // Katakana
                (c >= '\u4E00' && c <= '\u9FAF'))   // Kanji
            {
                // If any Japanese character is found, return true
                count++;
                if (count >= 5)
                {
                    return true;
                }
            }
        }

        // If no Japanese characters are found, return false
        return false;
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
