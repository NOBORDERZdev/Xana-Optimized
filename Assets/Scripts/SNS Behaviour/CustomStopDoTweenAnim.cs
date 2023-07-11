using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomStopDoTweenAnim : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<DOTweenAnimation>().DOPlay();
    }

    private void OnDisable()
    {
        this.GetComponent<DOTweenAnimation>().DOPause();
        this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}