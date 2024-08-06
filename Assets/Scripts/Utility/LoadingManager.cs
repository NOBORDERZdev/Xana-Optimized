using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text percentageText;
    public static bool photonChange =true;
    public GameObject loading_;
     private bool Once;
    private float time;
    public float TotalTimer;
    private bool StartSavingwaitingBool;
    /// <summary>
    /// Help Screen Arrays for 2 scenarios.
    /// If loading percentage is less than 50 only display helpScreenOne items
    /// else loading percentage is greater or equal to 50 display helpScreenTwo items
    /// </summary>
    [Header("Loading Help Screens UI")]
    public GameObject[] helpScreensOne;
    public GameObject[] helpScreensTwo;

    private void Awake()
    {
        loading_.SetActive(false);
        if (Instance == null)
        {
            Instance = this;            
            DontDestroyOnLoad(this);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
      
    }

    private void Start()
    {
        ChangeHelpScreenUI(true);
        StartSavingwaitingBool = false;
        Once = true;
     }
     private void Update()
    {       
        if (PhotonNetwork.LevelLoadingProgress > 0 && PhotonNetwork.LevelLoadingProgress < 1 && photonChange)
        {
            //Debug.LogError(PhotonNetwork.LevelLoadingProgress);
            //percentageText.text = (fill.fillAmount * 100).ToString("F0") + "%";
            FillLoading(PhotonNetwork.LevelLoadingProgress);
        }

        if (fill.fillAmount < 0.5f)
        {
            ChangeHelpScreenUI(true);
        }
        else
        {
            ChangeHelpScreenUI(false);
        }
        percentageText.text = (fill.fillAmount * 100).ToString("F0") + "%";
         if (!Once && StartSavingwaitingBool)
        {
            time += Time.deltaTime;
            fill.fillAmount =( time / TotalTimer) ;
             if (time >= TotalTimer)
            {
                StartSavingwaitingBool = false;
                Once = true;
            }
        }
  


    }

    public IEnumerator LoadAsncScene(string sceneName)
    {
        {
             AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                Debug.Log("progress value===" + asyncLoad.progress);
                FillLoading(asyncLoad.progress);
                yield return null;
                 if (asyncLoad.progress >= 0.9f)
                {
                    FillLoading(asyncLoad.progress);
                    break;
                }
            }
             asyncLoad.allowSceneActivation = true;
            StartCoroutine(FLoading(asyncLoad.progress));
           
        }
    }

    public IEnumerator FLoading(float value)
    {
        if (value > 0.99f)
        {               
            Invoke("HideLoading", 5f);
        }
        else
        {
            fill.fillAmount = value;          
            yield return null;
            StartCoroutine(FLoading(value + 0.01f));
        }
    }

    public void FillLoading(float value)
    {
        fill.fillAmount = value;
    }

    public void ShowLoading()
    {       
        gameObject.SetActive(true);
    }
    public void LeaveMultiplayer()
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadingManager.Instance.LoadAsncScene("Home"));
    }
    public void HideLoading()
    {
        percentageText.text = (0).ToString("F0") + "%";
        fill.fillAmount = 0;
        photonChange = true;
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Home")
        {
            time = 0;
             StartSavingwaitingBool = true;
            Once = false;
           // Debug.LogError("mainscene");
            GameManager.Instance.ComeFromWorld();
            StartCoroutine(OffLoader());
        } 
        else
        {
            gameObject.SetActive(false);
        }
     }
    IEnumerator OffLoader()
    {
        print("Whatsup");

       
         yield return new WaitForSeconds(TotalTimer);
        loading_.SetActive(true);
        if (!ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.orientationchanged)
        {
            //Debug.LogError("~~~~~ Waqas_ LoadingManager ~~~~~~~~~~~");
            //Screen.orientation = ScreenOrientation.Portrait;
        }
        yield return new WaitForSeconds(0.8f);
        loading_.SetActive(false);

        gameObject.SetActive(false);
     }
 
    /// <summary>
    /// Switch between HelpDialogs
    /// </summary>
    /// <param name="isFirst"> 
    /// if isFirst is TRUE then helpScreenOne items will be displayed
    /// and helpScreenTwo items will be hidden </param>
    public void ChangeHelpScreenUI(bool isFirst)
    {
        if (isFirst)
        {
            foreach (GameObject _helpDialog in helpScreensOne)
            {
                _helpDialog.SetActive(true);
            }
            foreach (GameObject _helpDialog in helpScreensTwo)
            {
                _helpDialog.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject _helpDialog in helpScreensOne)
            {
                _helpDialog.SetActive(false);
            }
            foreach (GameObject _helpDialog in helpScreensTwo)
            {
                _helpDialog.SetActive(true);
            }
        }

    }

   

}
