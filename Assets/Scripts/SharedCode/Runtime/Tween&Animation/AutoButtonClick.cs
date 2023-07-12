using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AutoButtonClick : MonoBehaviour {

    Button btn;
    public float wait;

    private void Awake()
    {
        if (btn == null) btn = GetComponent<Button>();
    }
    void OnEnable()
    {
        StartCoroutine(cr());
    }
    IEnumerator cr()
    {
        yield return new WaitForSeconds(wait);
        btn.onClick.Invoke();
    }


}
