using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeLimitUI : MonoBehaviour
{
    public float time;
    public string Purpose;
    public TextMeshProUGUI TimerLimitText;
    void Start()
    {
        StartCoroutine(IETimeLimit());
    }

    void Update()
    {

    }

    public IEnumerator IETimeLimit()
    {
        while (time > 0)
        {
            TimeSpan m_SecondsInToTimeFormate = TimeSpan.FromSeconds(time);
            time = (Mathf.Clamp(time, 0, Mathf.Infinity));
            TimerLimitText.text = Purpose + ": " + m_SecondsInToTimeFormate.ToString(@"mm\:ss");
            time--;
            yield return new WaitForSeconds(1);
        }
        Destroy(gameObject);
    }
}
