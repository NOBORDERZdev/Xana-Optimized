using TMPro;
using UnityEngine;
using System.Collections;

public class TagPrefabInfo : MonoBehaviour
{

    public TextMeshProUGUI tagName;
    public TextMeshProUGUI tagNameHighlighter;
    public GameObject descriptionPanel;
    public GameObject hightLighter;

    public void ClickOnTag()
    {
        hightLighter.SetActive(true);
        //WorldSearchManager.OpenSearchPanel?.Invoke(tagName.text);
        StartCoroutine(DisableHighlighter());
        descriptionPanel.SetActive(false);
    }

    IEnumerator DisableHighlighter()
    {
        yield return new WaitForSeconds(1);
        hightLighter.SetActive(false);
    }
}
