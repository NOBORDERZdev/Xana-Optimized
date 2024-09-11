using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmikageBoardContent : MonoBehaviour
{
    // Start is called before the first frame update
    private Image image;
    private TextMeshProUGUI headText;
    private TextMeshProUGUI bodyText;

    void Awake()
    {
        image = GetComponent<Image>();
        headText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        bodyText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void Init(string head, string body, Color color)
    {
        body = body.Replace("<p>", "").Replace("</p>", "");

        image.color = color;
        headText.SetText(head);
        bodyText.SetText(body);
    }
}
