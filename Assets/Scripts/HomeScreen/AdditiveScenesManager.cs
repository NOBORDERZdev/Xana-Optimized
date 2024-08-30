using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveScenesManager : MonoBehaviour
{
    public float sceneDelay;
    public string sceneTest;
    public string sceneTest2;
    public string sceneTest3;
    public string sceneTest4;
    public GameObject SNSmodule;
    public GameObject SNSMessage;
    public BottomTabManager homeBottomTab;
    public static System.Action OnAllSceneLoaded;

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
        if (BodyCustomizer.Instance)
        {
            BodyCustomizer.Instance.BodyCustomCallFromStore();
        }
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

        //if (XanaConstants.xanaConstants.metaverseType != XanaConstants.MetaverseType.PMY)
        //{
            GameManager.Instance.mainCharacter.GetComponent<AvatarController>().IntializeAvatar();
        //}

        if (XanaConstants.xanaConstants.isBackfromSns)
        {
            homeBottomTab.OnClickFeedButton();
            XanaConstants.xanaConstants.isBackfromSns=false;
        }
        LoadingHandler.Instance.HideLoading();

        OnAllSceneLoaded?.Invoke();
       // LoadingHandler.Instance.HideLoading(ScreenOrientation.Portrait, XanaConstants.xanaConstants.isBackFromWorld);
    }
}
