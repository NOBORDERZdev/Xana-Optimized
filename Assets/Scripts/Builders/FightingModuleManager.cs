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
    public bool isEnvLoaded;
    public string addressableSceneName;
    public string environmentLabel;
    public GameObject ClothAvatar;
    public GameObject CharacterLight;
    public Vector3 CharacterLightPosition;
    public float LightIntesity = .6f;
    public Animator avatarAnimator;
    public RuntimeAnimatorController fightingModuleAnimator;
    public RuntimeAnimatorController XanaAvatarAnimator;
    public GameObject myAvatar;
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
    void Start()
    {

    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.L))
        {
            myAvatar = Instantiate(ClothAvatar, transform);
            myAvatar.GetComponent<FootStaticIK>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            myAvatar.GetComponent<Animator>().runtimeAnimatorController = fightingModuleAnimator;
        }*/
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
            /*if (UserName.Contains("Guest") || UserName.Contains("ゲスト"))
            {
                if (GameManager.currentLanguage == "ja")
                {
                    UserName = "ゲスト" + UserName.Substring(UserName.Length - 4);
                }
                else if (GameManager.currentLanguage == "en")
                {
                    UserName = "Guest" + UserName.Substring(UserName.Length - 4);
                }
            }*/
        }
        Debug.LogError("UserName: " + UserName);
        ClothAvatar = GameManager.Instance.mainCharacter;
        avatarAnimator = ClothAvatar.GetComponent<Animator>();
        if (myAvatar != null)
        {
            Destroy(myAvatar);
        }
        myAvatar = Instantiate(ClothAvatar, transform);
        myAvatar.GetComponent<FootStaticIK>().enabled = false;
        myAvatar.name = UserName;
        GameObject L = Instantiate(CharacterLight, myAvatar.transform);
        for (int i = 0; i < L.transform.childCount; i++)
        {
            if (L.transform.GetChild(i).GetComponent<Light>())
            {
                L.transform.GetChild(i).GetComponent<Light>().intensity = LightIntesity;
            }
        }
        L.transform.position = CharacterLightPosition;
        myAvatar.GetComponent<Animator>().runtimeAnimatorController = fightingModuleAnimator;
        myAvatar.gameObject.SetActive(false);
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene("Demo_Fighter3D - Type 2");
    }

    public void LoadGamePlayScene()
    {
        StartCoroutine(IELoadGamePlayscene());
    }

    public IEnumerator IELoadGamePlayscene()
    {
        if (!isEnvLoaded)
        {
            if (environmentLabel.Contains(" : "))
            {
                string name = environmentLabel.Replace(" : ", string.Empty);
                environmentLabel = name;
            }
            //yield return StartCoroutine(DownloadEnvoirnmentDependanceies(environmentLabel));
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(environmentLabel, LoadSceneMode.Single, false);
            LoadingHandler.Instance.UpdateLoadingStatusText("Loading World...");
            LoadingHandler.Instance.UpdateLoadingSlider(.6f, true);
            yield return handle;
            addressableSceneName = environmentLabel;
            //...

            //One way to handle manual scene activation.
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                yield return handle.Result.ActivateAsync();
                isEnvLoaded = true;
            }
            else // error occur 
            {
                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();

                //HomeBtn.onClick.Invoke();
            }
        }
        else
        {
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();

            //RespawnPlayer();
        }
    }
}
