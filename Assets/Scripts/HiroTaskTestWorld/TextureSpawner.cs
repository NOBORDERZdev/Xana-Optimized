
using UnityEngine;
using BestHTTP;
using TMPro;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TextureSpawner : MonoBehaviour
{
    public Button button_Spawn;
    public TMP_InputField urlInputField, sizeInputFieldX, sizeInputFieldY, countInput;
    public MeshRenderer meshRendererPrefab;

    private int spawnCount = 10;

    private int maxDistance = 30;

    private int sizeX, sizeY;

    byte[] downloadedData;
    byte[] compressedData;
 
    // Start is called before the first frame update
    void Start()
    {
        urlInputField.text = "https://download.samplelib.com/png/sample-boat-400x300.png";
        //urlInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "https://download.samplelib.com/png/sample-boat-400x300.png";
        sizeInputFieldX.text = sizeInputFieldY.text = "2";
        sizeX = sizeY = 2;
        countInput.text = "10";
        spawnCount = 10;

        urlInputField.onEndEdit.AddListener(delegate { DownloadTexture(); });
        urlInputField.onValueChanged.AddListener(delegate { OnValueChanged(); });
        sizeInputFieldX.onValueChanged.AddListener(delegate { UpdateSizeXInputField(); });
        sizeInputFieldY.onValueChanged.AddListener(delegate { UpdateSizeYInputField(); });
        countInput.onValueChanged.AddListener(delegate { UpdateCountInputField(); });
        button_Spawn.onClick.AddListener(SpawnImages);
        DownloadTexture();
    }

    private void OnValueChanged()
    {
        button_Spawn.interactable = false;
        DownloadTexture();
    }

    public void UpdateSizeXInputField()
    {
        string data = sizeInputFieldX.text;
        if (int.TryParse(data, out sizeX))
        {
            Debug.Log("SizeX: " + sizeX);
        }
    }

    public void UpdateSizeYInputField()
    {
        string data = sizeInputFieldY.text;
        if (int.TryParse(data, out sizeY))
        {
            Debug.Log("SizeY: " + sizeY);
        }
    }
    public void UpdateCountInputField()
    {
        string data = countInput.text;
        if (int.TryParse(data, out spawnCount))
        {
            Debug.Log("spawnCount: " + spawnCount);
        }
    }
    public void DownloadTexture()
    {
        if(!IsUrlValid(urlInputField.text))
        {
            Debug.Log($"<color=red> Invalid URL </color>");
            button_Spawn.interactable = false;
            return;
        }
        Debug.Log($"<color=red> Downloading texture from {urlInputField.text} </color>");
        var request = new HTTPRequest(new System.Uri(urlInputField.text), OnImageDownloadFinished);
        request.Send();
    }
    void OnImageDownloadFinished(HTTPRequest req, HTTPResponse resp)
    {
        downloadedData = resp.Data;
        Debug.Log($"<color=red> Texture downloaded downloadedSize: {downloadedData.Length / 1024} Kbytes </color>");
        button_Spawn.interactable = true;
    }

    private void SpawnImages()
    {
        if (TestWorldCanvasManager.Instance.parentObj == null)
        {
            Debug.Log("<color=red> Parent object is null </color>");
            TestWorldCanvasManager.Instance.parentObj = Instantiate(new GameObject("TestObjectsParent"), Vector3.zero, Quaternion.identity);
            TestWorldCanvasManager.Instance.parentObj.transform.SetParent(null);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            float rotationAngle = UnityEngine.Random.Range(0f, 360f);
            Vector3 randPos = new Vector3(UnityEngine.Random.Range(-maxDistance, maxDistance), 1 + (sizeY / 2), UnityEngine.Random.Range(-maxDistance, maxDistance));
            MeshRenderer spawnedObject = Instantiate(meshRendererPrefab); //, randPos, Quaternion.identity
            spawnedObject.transform.localScale = new Vector3(sizeX, sizeY, 0);
            spawnedObject.transform.position = randPos;
            spawnedObject.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
            spawnedObject.transform.parent = TestWorldCanvasManager.Instance.parentObj.transform;

            spawnedObject.material = new Material(spawnedObject.material);

            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(downloadedData);
            tex.filterMode = FilterMode.Bilinear; //if no mipmap exists, this is same as bilinear
            tex.wrapMode = TextureWrapMode.Clamp;
#if UNITY_ANDROID
            tex.Compress(false); // ASTC 8x8 for Android
#else
            tex.Compress(true); // ETC2 RGBA8 for other platforms
#endif
            if(i == 0)
                compressedData = tex.GetRawTextureData();
            spawnedObject.material.mainTexture = tex;
        }

        TestWorldCanvasManager.Instance.AddTextureCount(spawnCount);
        Debug.Log($"<color=red> Texture compressed compressedSize: {compressedData.Length / 1024} Kbytes </color>");
        TestWorldCanvasManager.Instance.AddTextureEstimateMBCount(spawnCount * (compressedData.Length / 1024f / 1024f));
        Debug.Log($"<color=red>{spawnCount} Textures spawned </color>");
    }
    public bool IsUrlValid(string input)
    {
        // Define a regular expression pattern for URL validation.
        // This pattern checks for URLs starting with "http://", "https://", or "www." followed by a valid domain.
        string pattern = @"^(https?://|www\.)[A-Za-z0-9\-\.]+\.[A-Za-z]{2,}(\/\S*)?$";

        // Create a regular expression object and match the input against the pattern.
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
        Match match = regex.Match(input);

        // Check if the input matches the pattern and is a valid URL.
        return match.Success;
    }
}
