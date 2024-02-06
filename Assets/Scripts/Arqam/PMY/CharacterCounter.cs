using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCounter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        //yield return new WaitForSeconds(0.5f);
        int count = GetComponentInChildren<TMP_Text>().text.Length;
        if(count > 20)
            GetComponent<VerticalLayoutGroup>().padding.top = 0;

        ContentSizeFitter contentSizeFitter = GetComponent<ContentSizeFitter>();
        contentSizeFitter.enabled = false;
        yield return new WaitForSeconds(0.1f);
        contentSizeFitter.enabled = true;
    }

}
