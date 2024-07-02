using System.Collections.Generic;
using UnityEngine;

public class UploadPropertyManager : MonoBehaviour
{
    public GameObject UploadPlayerPrefab;
    public List<GameObject> MediaScreens;
    Transform _mediaParent;
    List<UploadData> _uploadDatas = new List<UploadData>();
    bool _isInitialize = false;

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
        _mediaParent = this.transform;
    }

    private void UploadPropertiesData(UploadProperties uploadProperties)
    {
        if (uploadProperties == null) return;

        _uploadDatas = uploadProperties.uploadData;

    }

    void UploadPropertiesInit()
    {
        if (_isInitialize)
            return;

        if (_uploadDatas.Count > 0)
        {
            foreach (var data in _uploadDatas)
            {
                AddScreen(data);
            }
            _isInitialize = true;
        }
    }
    private void AddScreen(UploadData data)
    {
        var instantiatedPrefab = Instantiate(UploadPlayerPrefab, data.position, data.rotation, _mediaParent);
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
        MediaScreens.Add(instantiatedPrefab);
    }
}