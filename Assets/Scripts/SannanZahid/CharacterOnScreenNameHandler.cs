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
        if (name.Contains("Home"))
        {
            UpdateNameText(PlayerPrefs.GetString("UserName"));
        }
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
        if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
        {
            _onScreenName.text = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
        }
        if (PlayerPrefs.GetInt("WalletConnect") == 0)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (PlayerPrefs.GetString("UserNameAndPassword").IsNotEmpty() || (ConstantsHolder.xanaConstants != null && !ConstantsHolder.xanaConstants.LoginasGustprofile))
                {
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
                _onScreenName.text = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
                while (PlayerPrefs.GetString("UserNameAndPassword") == "")
                    yield return new WaitForSeconds(0.5f);
                StartCoroutine(IERequestGetUserDetails());
            }
            else
            {
                StartCoroutine(IERequestGetUserDetails());
            }
        }
        else
        {
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
    }
    public IEnumerator IERequestGetUserDetails()
    {
        using (UnityWebRequest www = UnityWebRequest.Get((ConstantsGod.API_BASEURL + ConstantsGod.r_url_GetUserDetails)))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                StartCoroutine(IERequestGetUserDetails());
            }
            else
            {
                GetUserDetailRoot tempMyProfileDataRoot = JsonUtility.FromJson<GetUserDetailRoot>(www.downloadHandler.text.ToString());
                UpdateNameText(tempMyProfileDataRoot.data.name);
                UpdatePlayerNameRef(tempMyProfileDataRoot.data.name);
            }
        }
    }
    private void UpdatePlayerNameRef(string localUsername)
    {
        PlayerPrefs.SetString("PlayerName", localUsername);
        PlayerPrefs.SetString("UserName", localUsername);
        ConstantsHolder.userName = localUsername;
    }

    #endregion
    }