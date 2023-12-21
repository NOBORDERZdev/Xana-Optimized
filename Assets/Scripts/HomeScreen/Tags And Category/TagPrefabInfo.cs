using TMPro;
using UnityEngine;
using System.Collections;

public class TagPrefabInfo : MonoBehaviour
{

    public TextMeshProUGUI tagName;
    public TextMeshProUGUI tagNameHighlighter;
    public GameObject hightLighter;
    [HideInInspector]
    public GameObject descriptionPanel;

    public void ClickOnTag()
    {
        WorldSearchManager.OpenSearchPanel?.Invoke(tagName.text);
        WorldSearchManager.SearchWorld?.Invoke(tagName.text);
        descriptionPanel.SetActive(false);
    }
}
