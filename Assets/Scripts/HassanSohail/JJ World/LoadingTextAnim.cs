using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LoadingTextAnim : MonoBehaviour
{
    [SerializeField] TMP_Text LoadingText;
    //[SerializeField] float startPoint;
    [SerializeField] float speed;
    [SerializeField] List<string> texts;
    int i = 0;
    private void Start()
    {
        InvokeRepeating(nameof(changeTxt),speed,1);
    }

    void changeTxt()
    {
        LoadingText.text = texts[i];
        if (i < texts.Count - 1)
        {
            i++;
        }
        else
        {
            i = 0;
        }
    }

}
