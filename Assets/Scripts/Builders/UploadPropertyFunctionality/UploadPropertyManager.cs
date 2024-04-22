using System.Collections.Generic;
using UnityEngine;

public class UploadPropertyManager : MonoBehaviour
{
    public GameObject upLoadPlayerPrefab;
    public List<GameObject> mediaScreens;
    Transform mediaParent;
    List<UploadData> uploadDatas = new List<UploadData>();

    private void OnEnable()
    {
        BuilderEventManager.UploadPropertiesData += UploadPropertiesData;
        BuilderEventManager.UploadPropertiesInit += UploadPropertiesInit;
    }
    private void OnDisable()
    {
        BuilderEventManager.UploadPropertiesData -= UploadPropertiesData;
        BuilderEventManager.UploadPropertiesInit -= UploadPropertiesInit;
    }

    void Start()
    {
        mediaParent = this.transform;
    }

    private void UploadPropertiesData(UploadProperties uploadProperties)
    {
        if (uploadProperties == null) return;

        uploadDatas = uploadProperties.uploadData;

    }

    void UploadPropertiesInit()
    {
        if (uploadDatas.Count > 0)
        {
            foreach (var data in uploadDatas)
            {
                AddScreen(data);
            }
        }
    }
    private void AddScreen(UploadData data)
    {
        var instantiatedPrefab = Instantiate(upLoadPlayerPrefab, data.position, data.rotation, mediaParent);
        instantiatedPrefab.transform.localScale = Vector3.one * data.scale;

        if (data.position.z == 0)
            instantiatedPrefab.transform.position = new Vector3(data.position.x, data.position.y, 2);

        var mediaPlayer = instantiatedPrefab.GetComponent<UploadPropertyBehaviour>();
        mediaPlayer.id = data.uploadMediaId;
        mediaPlayer.url = data.url;
        mediaPlayer.localFileName = data.localFileName;
        mediaPlayer.mediaType = data.mediaType;
        mediaPlayer.liveStream = data.isLivestream;
        // mediaPlayer.index = data.index;
        mediaPlayer.addFrame = data.addFrame;
        mediaPlayer.isRepeat = data.isRepeat;
        mediaScreens.Add(instantiatedPrefab);
    }
}