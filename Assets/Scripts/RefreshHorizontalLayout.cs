using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class RefreshHorizontalLayout : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(StartEnum());
    }

    IEnumerator StartEnum()
    {
        GetComponent<HorizontalLayoutGroup>().enabled = false;
        yield return new WaitForSeconds(.1f);
        GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
