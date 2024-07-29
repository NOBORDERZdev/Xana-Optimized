using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashManager : MonoBehaviour
{
    [SerializeField]
    private float timer;
    private void OnEnable()
    {
        StartCoroutine(SplashSystem());
    }

    public IEnumerator SplashSystem()
    {
        yield return new WaitForSeconds(timer);
        this.gameObject.SetActive(false);
    }
}
