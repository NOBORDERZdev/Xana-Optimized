using Photon.Voice;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class CharacterOnScreenNameHandler : MonoBehaviour
{
    #region Private Veriables
    [SerializeField]
    RectTransform _onScreenSprite;
    [SerializeField]
    Camera _cameraToProcess;
    [SerializeField]
    public Vector3 _offsetPos;
    #endregion

    #region Initialization Function For Avatars
    public void setIndicators(RectTransform onScreenSprite, Camera camera)
    {
        _onScreenSprite = onScreenSprite;
        _cameraToProcess = camera;
    }
    #endregion

    #region Positioning Mechanics
    private void Start()
    {
        StartCoroutine(SetName());
    }
    void Update()
    {
        placeNameHolderToScreen();
    }
    void placeNameHolderToScreen()
    {
        Vector3 screenpos = _cameraToProcess.WorldToScreenPoint(transform.position);
        if (screenpos.z > 0 && screenpos.x < Screen.width && screenpos.x > 0 && screenpos.y < Screen.height && screenpos.y > 0)
        {
            _onScreenSprite.position = new Vector3(screenpos.x + _offsetPos.x, screenpos.y + _offsetPos.y, 0 + _offsetPos.z);
            _onScreenSprite.gameObject.SetActive(true);
        }
    }
    IEnumerator SetName()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("shownWelcome") == 1 || PlayerPrefs.GetString("UserNameAndPassword")!="")
            {
                break;
            }
        }
        yield return new WaitForSeconds(1f);
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            _onScreenSprite.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
        }
        else
        {
            StartCoroutine(IERequestGetUserDetails());
        }
    }
    public IEnumerator IERequestGetUserDetails()
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetUserDetails)))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                StartCoroutine(IERequestGetUserDetails());
            }
            else
            {
                GetUserDetailRoot tempMyProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(www.downloadHandler.text.ToString());
                _onScreenSprite.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = tempMyProfileDataRoot.data.name;
            }
        }
    }
    #endregion
}