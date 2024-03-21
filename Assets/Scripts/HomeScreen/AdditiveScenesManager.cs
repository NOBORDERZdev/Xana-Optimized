using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveScenesManager : MonoBehaviour
{
    public float sceneDelay;
    private string sceneTest= "LoginSignUp";
    private string sceneTest2= "InventoryScene";
    private string sceneTest3= "SNSFeedModuleScene";
    private string sceneTest4= "SNSMessageModuleScene";
    [HideInInspector]
    public GameObject SNSmodule;
    [HideInInspector]
    public GameObject SNSMessage;
    public HomeFooterTabCanvas homeBottomTab;
    
    private void Start()
    {
        if(!XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            sceneDelay = .5f;
            StartCoroutine(AddDelayStore(sceneDelay / 3));
            StartCoroutine(AddDelay(sceneDelay));
            StartCoroutine(AddDelaySNSFeedModule(sceneDelay));
            StartCoroutine(AddDelaySNSMessageModule(sceneDelay));
        }
    }
    IEnumerator AddDelayStore(float delay)
    {
        yield return new WaitForSeconds(delay);
         AsyncOperation async = SceneManager.LoadSceneAsync(sceneTest2, LoadSceneMode.Additive);
        while(!async.isDone)
        {
            yield return null;
        }
        BodyCustomizer.Instance.BodyCustomCallFromStore();
    }
    IEnumerator AddDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneTest, LoadSceneMode.Additive);
    }

    IEnumerator AddDelaySNSFeedModule(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneTest3, LoadSceneMode.Additive);
    }
    IEnumerator AddDelaySNSMessageModule(float delay)
    {
       
        yield return new WaitForSeconds(delay);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneTest4, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        //GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();
        if (XanaConstants.xanaConstants.isBackfromSns)
        {
            homeBottomTab.OnClickFeedButton();
            XanaConstants.xanaConstants.isBackfromSns=false;
        }
        LoadingHandler.Instance.HideLoading();
       // LoadingHandler.Instance.HideLoading(ScreenOrientation.Portrait, XanaConstants.xanaConstants.isBackFromWorld);
    }
}
