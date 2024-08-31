using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMaptoCenter : MonoBehaviour
{
    public float moveDuration = 1.0f;
    public List<GameObject> MapHighlightObjs;

    public GameObject mainScreen; // Reference to the parent (Main Screen)
    public GameObject childObject; // Reference to the child object containing the Grandchild
    private GameObject grandChildPing; // Reference to the Grandchild named "Ping"

    void Start()
    {
        for (int i = 0; i < MapHighlightObjs.Count; i++)
        {
            int index = i;
            MapHighlightObjs[i].GetComponent<Button>().onClick.AddListener(() => ItemClicked(index));
        }
    }


    public void ItemClicked(int ind)
    {
        Debug.Log("Item Clicked: " + ind);
        grandChildPing = MapHighlightObjs[ind];
        StartCoroutine(MoveChildToCenterOfMainScreen());
        EnableSelectedImage(ind);
    }

    IEnumerator MoveChildToCenterOfMainScreen()
    {
        Debug.Log("Moving Child to Center of Main Screen");
        Vector3 startPosition = childObject.transform.position;
        Vector3 targetPosition = mainScreen.transform.position - grandChildPing.transform.position + startPosition;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            childObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set
        childObject.transform.position = targetPosition;
    }
    //void MoveChildToCenterOfMainScreen()
    //{
    //    Debug.Log("Moving Child to Center of Main Screen");
    //    // Calculate the offset needed to bring the Grandchild "Ping" to the center of the Main Screen
    //    Vector3 offset = mainScreen.transform.position - grandChildPing.transform.position;

    //    // Apply the offset to the Child Object to center "Ping" in the Main Screen
    //    childObject.transform.position += offset;
    //}
    void EnableSelectedImage(int _SelectedImage)
    {
        foreach (GameObject obj in MapHighlightObjs)
        {
            obj.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
        }

        MapHighlightObjs[_SelectedImage].GetComponent<Image>().color = new Color(1, 1, 1, 1f);
    }
}