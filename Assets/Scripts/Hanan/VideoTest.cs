using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoTest : MonoBehaviour
{
    public Sprite thunbNailImage;
    public GameObject spriteObject;
    public VideoPlayer videoPlayer;
    public string Link;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownloadVideo());
    }
   
    IEnumerator DownloadVideo()
    {
        UnityWebRequest www = UnityWebRequest.Get(Link);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("error "+www.error);
        }
        else
        {
            File.WriteAllBytes(Application.persistentDataPath + "/" + "testhanan", www.downloadHandler.data);

            spriteObject = new GameObject();
            spriteObject.name = "SpriteObject";
            spriteObject.transform.parent = this.transform;
            spriteObject.transform.position = this.transform.position;
            spriteObject.transform.rotation = this.transform.rotation;
            spriteObject.transform.localPosition = new Vector3(0, 0, .01f);
            spriteObject.AddComponent<SpriteRenderer>();
            spriteObject.GetComponent<SpriteRenderer>().sprite = thunbNailImage;

            yield return new WaitForSeconds(0.5f);
            videoPlayer = spriteObject.AddComponent<VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            spriteObject.GetComponent<VideoPlayer>().url = Link;
            spriteObject.GetComponent<VideoPlayer>().isLooping= true;

            thunbNailImage = Sprite.Create(new Texture2D((int)videoPlayer.height, (int)videoPlayer.width), new Rect(0f, 0f, (int)videoPlayer.height, (int)videoPlayer.width), new Vector2(0.5f, 0f));

           

            //spriteObject.GetComponent<VideoPlayer>().Play();





        }
    }
}
