using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCircleBtnHighlightHandler : MonoBehaviour
{
    public static Action<int> ActivateHighlightsByIndex;
    public List<Transform> ActionBtnHeighlightObjects = new List<Transform>();

    private void OnEnable()
    {
        ActivateHighlightsByIndex += ActivateHighlights;
        ActionManager.DisableFavoriteCircleHighlighter += DisableHighlights;
    }
    private void OnDisable()
    {
        ActivateHighlightsByIndex -= ActivateHighlights;
        ActionManager.DisableFavoriteCircleHighlighter -= DisableHighlights;
        DisableHighlights();
    }


    public void ActivateHighlights(int index)
    {
        Debug.LogError("---->   ActivateHighlights" + index);
        DisableHighlights();
        ActionBtnHeighlightObjects[index].GetComponent<ActionFavouriteCircleBtn>().ActivateHeighlight(true);
    }

    private void DisableHighlights()
    {
        foreach (Transform item in ActionBtnHeighlightObjects)
        {
            item.GetComponent<ActionFavouriteCircleBtn>().ActivateHeighlight(false);
        }
    }
}
