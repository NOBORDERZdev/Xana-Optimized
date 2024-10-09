using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMLoader : MonoBehaviour
{
    public string bgmUrl;
    private void OnEnable()
    {
        BuilderEventManager.loadBGMDirectly?.Invoke(bgmUrl);
    }

    private void OnDisable()
    {
        BuilderEventManager.StopBGM?.Invoke();
    }
}
