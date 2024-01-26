using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class CharacterOnScreenNameHandler : MonoBehaviour
{
    public static CharacterOnScreenNameHandler instance;
    [SerializeField]
    TMPro.TMP_Text _onScreenName;
    
    #region Positioning Mechanics
    private void Start()
    {
        instance = this;
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
      
        if (PlayerPrefs.GetInt("WalletConnect") == 0)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (PlayerPrefs.GetString("UserNameAndPassword").IsNotEmpty())
                {
                    break;
                }
            }
           // Debug.LogError("SetName");
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
              

                _onScreenName.text = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
            }
            else
            {
               // Debug.LogError("SetName 2");

                StartCoroutine(IERequestGetUserDetails());
            }
        }
        else
        {
           
          
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
            {
                //Debug.LogError("SetName 1");

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
            }
        }
    }
   
       
    
    #endregion
    }