using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NftdetailsPage : MonoBehaviour
{
    public Image NftThumb;
    public GameObject PlayBtn;
   // public TMPro.TextMeshProUGUI NftOwner;
    public TMPro.TextMeshProUGUI NftCreator;
    public TMPro.TextMeshProUGUI NftDes;
    private Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        NftThumb.sprite = OwnedNftLoadButton.nftImage;
        //StartCoroutine(LoadSpriteEnv(PlayerPrefs.GetString(ConstantsGod.NFTTHUMB), NftThumb));
        // NftOwner.text = PlayerPrefs.GetString(ConstantsGod.NFTOWNER).Substring(0,6);
        NftCreator.text = PlayerPrefs.GetString(ConstantsGod.NFTCREATOR);
        NftDes.text = PlayerPrefs.GetString(ConstantsGod.NFTDES);

        if (PlayerPrefs.GetString(ConstantsGod.NFTTYPE).Equals("movie"))
        {
            PlayBtn.SetActive(true);
        }
        else
        {
            PlayBtn.SetActive(false);
        }

    }

    public void DataReset()
    {
        NftThumb.sprite=null;
       // NftOwner.text = string.Empty;
        NftCreator.text = string.Empty;
        NftDes.text = string.Empty;
    }

    IEnumerator LoadSpriteEnv(string ImageUrl, Image thumbnail)
    {
        Debug.Log("image url====" + ImageUrl);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

        }
        else
        {
            if (ImageUrl.Equals(""))
            {
            }
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(ImageUrl);
                www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }
                Texture2D thumbnailTexture = DownloadHandlerTexture.GetContent(www);
                thumbnailTexture.Compress(true);
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {

                }
                else
                {
                    sprite = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0, 0));
                    if (thumbnail != null)
                    {
                        thumbnail.sprite = sprite;
                    }
                    else
                    {
                    }
                }
                www.Dispose();
            }
        }
    }
    public void nftDetailsBtnClick()
    {
        Application.OpenURL(PlayerPrefs.GetString(ConstantsGod.NFTLINK));
    }
}
