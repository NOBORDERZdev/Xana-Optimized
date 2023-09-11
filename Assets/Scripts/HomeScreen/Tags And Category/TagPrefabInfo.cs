using TMPro;
using UnityEngine;

public class TagPrefabInfo : MonoBehaviour
{

    public TextMeshProUGUI tagName;
    public TextMeshProUGUI tagNameHighlighter;

    public GameObject hightLighter;

    public void ClickOnTag()
    {
        hightLighter.SetActive(true);
        WorldSearchManager.OpenSearchPanel?.Invoke(tagName.text);
    }
}
