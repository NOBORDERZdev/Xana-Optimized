using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class FightingModuleManager : MonoBehaviour
{
    public bool isUserHaveAlphaPass;
    public bool isEnvLoaded;
    public string addressableSceneName;
    public string environmentLabel;
    public string player1Icon;
    public string player2Icon;
    public static FightingModuleManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnClickMainMenu()
    {
        StartCoroutine(IEOnClickMainMenu());
    }

    public IEnumerator IEOnClickMainMenu()
    {
        const string UsernamePrefs = "PlayerName";
        string UserName = "";
        if (PlayerPrefs.HasKey(UsernamePrefs))
        {
            UserName = PlayerPrefs.GetString(UsernamePrefs);
        }
        Debug.LogError("UserName: " + UserName);
        Input.multiTouchEnabled = true;
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene("Demo_Fighter3D - Type 2");
    }
}
