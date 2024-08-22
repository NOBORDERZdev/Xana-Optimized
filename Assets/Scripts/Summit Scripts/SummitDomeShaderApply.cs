using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SummitDomeShaderApply : MonoBehaviour
{
    [HideInInspector]
    public int DomeId;
    [HideInInspector]
    public string ImageUrl;
    public GameObject DomeBannerParent;
    public GameObject DomeText;
    public GameObject Frame;
    public MeshRenderer DomeMeshRenderer;

    public void Init()
    {
        DomeBannerParent.SetActive(true);
        if (!string.IsNullOrEmpty(ImageUrl))
        {
            //ImageUrl = XANASummitDataContainer.GetDomeImage(DomeId);
            DownloadDomeTexture();
        }
    }

    async void DownloadDomeTexture()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(ImageUrl);
        await request.SendWebRequest();
        if ((request.result == UnityWebRequest.Result.ConnectionError) || (request.result == UnityWebRequest.Result.ProtocolError))
            Debug.Log(request.error);
        else
        {
            DomeMeshRenderer.material.mainTexture = DownloadHandlerTexture.GetContent(request);
            DomeMeshRenderer.gameObject.SetActive(true);
            Frame.SetActive(true);
        }

        request.Dispose();
    }
}
