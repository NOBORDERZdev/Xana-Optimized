using UnityEngine.UI;
using TMPro;
using UnityEngine;
using SuperStar.Helpers;
using System.Collections;

public class TutorialWorldItemView : MonoBehaviour
{
    public Image worldIcon;
    public TextMeshProUGUI eviroment_Name;
    public string m_EnvironmentName, ThumbnailDownloadURL;

    private void OnEnable()
    {
        StartCoroutine(DownloadAndLoadFeed());
        StartCoroutine(InitializeEnvironmentName());
    }
    public void Init(string _worldName,string _thumbnailDownloadURL)
    {
        this.gameObject.SetActive(true);
        m_EnvironmentName = _worldName;
        ThumbnailDownloadURL = _thumbnailDownloadURL;
    }
    public IEnumerator DownloadAndLoadFeed()
    {
        yield return null;
        if (AssetCache.Instance.HasFile(ThumbnailDownloadURL))
        {
            AssetCache.Instance.LoadSpriteIntoImage(worldIcon, ThumbnailDownloadURL, changeAspectRatio: true);
        }
        else
        {
            AssetCache.Instance.EnqueueOneResAndWait(ThumbnailDownloadURL, ThumbnailDownloadURL, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(worldIcon, ThumbnailDownloadURL, changeAspectRatio: true);
                }
            });
        }

    }
    public IEnumerator InitializeEnvironmentName()
    {
        if (m_EnvironmentName.Contains("Dubai"))
        {
            eviroment_Name.text = "DUBAI FESTIVAL STAGE.";
            eviroment_Name.GetComponent<UITextLocalization>().LocalizeTextText(eviroment_Name.text);
        }
        else
        {
            eviroment_Name.GetComponent<UITextLocalization>().LocalizeTextText(m_EnvironmentName);
        }
        eviroment_Name.text = eviroment_Name.text;
        yield return null;
    }
}
