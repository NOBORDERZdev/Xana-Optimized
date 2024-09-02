using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SubWorldPrefab : MonoBehaviour
{
    public Button SubWorldPrefabButton;
    public int WorldId;
    public string SubWorldName;
    public string WorldDescription;
    public string CreatorName;
    public string WorldType;
    public string WorldCategory;
    public string WorldTimeEstimate;
    public string ThumbnailUrl;
    public Sprite WorldImage;
    public TMPro.TextMeshProUGUI WorldName;
    public Vector3 PlayerReturnPosition;

    public void Init()
    {
        StartCoroutine(DownloadTexture());
    }

    public void OnSubWorldPrefabClicked()
    {
        BuilderEventManager.LoadSceneByName?.Invoke(WorldId.ToString(), PlayerReturnPosition);
    }

    IEnumerator DownloadTexture()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(ThumbnailUrl);
        request.SendWebRequest();
        while(!request.isDone)
        {
            yield return null;
        }
        Texture2D texture2D = DownloadHandlerTexture.GetContent(request);
        WorldImage=SubWorldPrefabButton.GetComponent<Image>().sprite = ConvertToSprite(texture2D);
    }

    private Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
