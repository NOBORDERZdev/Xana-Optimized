using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class CharacterOnScreenNameHandler : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text _onScreenName;

    #region Positioning Mechanics
    private void Start()
    {
        StartCoroutine(SetName());
    }
    public void SetNameOfPlayerAgain()
    {
        StartCoroutine(SetName());
    }
    public void UpdateNameText(string newName)
    {
        _onScreenName.text = newName;
    }
    IEnumerator SetName()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("shownWelcome") == 1 && PlayerPrefs.GetString("UserNameAndPassword").IsNotEmpty())
            {
                break;
            }
        }
        yield return new WaitForSeconds(1f);
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            _onScreenName.text = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
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
                UpdateNameText(tempMyProfileDataRoot.data.name);
            }
        }
    }
    #endregion
}