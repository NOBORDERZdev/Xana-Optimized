using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesBlinking : MonoBehaviour
{
    public static EyesBlinking instance;
    public SkinnedMeshRenderer blendHolder;
    public List<float> EyeBlendShapeValues = new List<float>();

    public float blinkingRate;

    public bool isEyeClose = false;

    public float waitTime;
    private float counter = 0;


    public float blinkingSpeed;
    public bool isBlinking = true;

    public bool isCoroutineRunning = true;

    private void Awake()
    {
        instance = this;
        StartCoroutine(BlinkingStartRoutine());
    }
    public void StoreBlendShapeValues()
    {
        EyeBlendShapeValues.Clear();
        isBlinking = false;
        blendHolder.SetBlendShapeWeight(26, 0);
        blendHolder.SetBlendShapeWeight(32, 0);

        EyeBlendShapeValues.Clear();
        for (int i = 23; i <= 35; i++)
        {
            EyeBlendShapeValues.Add(blendHolder.GetBlendShapeWeight(i));
        }
        isBlinking = true;
    }

    public IEnumerator BlinkingStartRoutine()
    {
        if (isBlinking && EyeBlendShapeValues.Count != 0)
        {
            while (isEyeClose)
            {
                for (int i = 23; i <= 35 && i != 26 && i != 32; i++)
                {
                    blendHolder.SetBlendShapeWeight(i, Mathf.MoveTowards(blendHolder.GetBlendShapeWeight(i), 0, Time.deltaTime * blinkingRate));
                }
                blendHolder.SetBlendShapeWeight(26, Mathf.MoveTowards(blendHolder.GetBlendShapeWeight(26), 100, Time.deltaTime * blinkingRate));
                blendHolder.SetBlendShapeWeight(32, Mathf.MoveTowards(blendHolder.GetBlendShapeWeight(32), 100, Time.deltaTime * blinkingRate));

                if (counter < blinkingSpeed)
                    counter += Time.deltaTime;

                if (blendHolder.GetBlendShapeWeight(26) == 100)
                {
                    isEyeClose = false;
                    counter = 0;
                }
            }
            yield return new WaitForSeconds(blinkingSpeed);
            while (!isEyeClose)
            {
                for (int i = 23; i <= 35; i++)
                {
                    blendHolder.SetBlendShapeWeight(i, Mathf.MoveTowards(blendHolder.GetBlendShapeWeight(i), EyeBlendShapeValues[i - 23], Time.deltaTime * blinkingRate));
                }
                if (counter < blinkingSpeed)
                    counter += Time.deltaTime;

                if (blendHolder.GetBlendShapeWeight(26) == 0)
                {
                    isEyeClose = true;
                    counter = 0;
                }
            }
            isCoroutineRunning = true;
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(BlinkingStartRoutine());
        }
        isCoroutineRunning = false;
    }
}