using System.Collections.Generic;
using UnityEngine;

public class UploadPropertyManager : MonoBehaviour
{
    public GameObject upLoadPlayerPrefab;
    public List<GameObject> mediaScreens;
    Transform mediaParent;

    private void OnEnable()
    {
        BuilderEventManager.UploadPropertiesLoad += UploadPropertiesLoad;
    }
    private void OnDisable()
    {
        BuilderEventManager.UploadPropertiesLoad -= UploadPropertiesLoad;
    }

    void Start()
    {
        mediaParent= this.transform;
    }

    private void UploadPropertiesLoad(UploadProperties uploadProperties)
    {
        if (uploadProperties == null) return;

        var uploadData = uploadProperties.uploadData;
        if (uploadData != null)
        {
            foreach (var data in uploadData)
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