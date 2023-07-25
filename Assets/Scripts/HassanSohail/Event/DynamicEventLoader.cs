using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DynamicEventLoader : MonoBehaviour
{
    [SerializeField] GameObject loaderPanel;
    [SerializeField] Slider loadingSlider;

    private void Start()
    {
        Int();
    }

    void Int()
    {
        loaderPanel.SetActive(false);
        loadingSlider.value = 0;
    }


    public void UpdateSlider(int value) {
        loadingSlider.DOValue(value, 0.1f, true);
    }
}
