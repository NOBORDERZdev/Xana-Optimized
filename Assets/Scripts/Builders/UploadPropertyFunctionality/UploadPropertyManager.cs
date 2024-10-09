using System.Collections.Generic;
using UnityEngine;

public class UploadPropertyManager : MonoBehaviour
{
    public GameObject UploadPlayerPrefab;
    public List<GameObject> MediaScreens = new List<GameObject>();
    private Transform _mediaParent;
    private List<UploadData> _uploadDatas = new List<UploadData>();
    private bool _isInitialize = false;
    private Queue<GameObject> _objectPool = new Queue<GameObject>();
    private int _initialPoolSize = 100; // Adjust this value based on your needs

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

    private void Start()
    {
        _mediaParent = this.transform;
        InitializeObjectPool();
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            var obj = Instantiate(UploadPlayerPrefab);
            obj.SetActive(false);
            _objectPool.Enqueue(obj);
        }
    }

    private GameObject GetPooledObject()
    {
        if (_objectPool.Count > 0)
        {
            var obj = _objectPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            var obj = Instantiate(UploadPlayerPrefab);
            return obj;
        }
    }

    private void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        _objectPool.Enqueue(obj);
    }

    private void UploadPropertiesData(UploadProperties uploadProperties)
    {
        if (uploadProperties == null || uploadProperties.uploadData == null) return;

        _uploadDatas = uploadProperties.uploadData;
    }

    private void UploadPropertiesInit()
    {
        if (_isInitialize || _uploadDatas == null || _uploadDatas.Count == 0) return;

        foreach (var data in _uploadDatas)
        {
            AddScreen(data);
        }
        _isInitialize = true;
    }

    private void AddScreen(UploadData data)
    {
        if (data == null) return;

        var instantiatedPrefab = GetPooledObject();
        instantiatedPrefab.transform.SetParent(_mediaParent);
        instantiatedPrefab.transform.position = data.position;
        instantiatedPrefab.transform.rotation = data.rotation;
        instantiatedPrefab.transform.localScale = Vector3.one * data.scale;

        if (data.position.z == 0)
        {
            instantiatedPrefab.transform.position = new Vector3(data.position.x, data.position.y, 2);
        }

        var mediaPlayer = instantiatedPrefab.GetComponent<UploadPropertyBehaviour>();
        if (mediaPlayer != null)
        {
            mediaPlayer.id = data.uploadMediaId;
            mediaPlayer.url = data.url;
            mediaPlayer.localFileName = data.localFileName;
            mediaPlayer.mediaType = data.mediaType;
            mediaPlayer.liveStream = data.isLivestream;
            mediaPlayer.addFrame = data.addFrame;
            mediaPlayer.isRepeat = data.isRepeat;
        }

        MediaScreens.Add(instantiatedPrefab);
    }
}
