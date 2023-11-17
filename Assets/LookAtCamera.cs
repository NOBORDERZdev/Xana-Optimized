using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UserPostFeature;

public class LookAtCamera : MonoBehaviour
{
    Transform cameraTransform;
    [SerializeField]
    TMPro.TMP_Text PostText;
    [SerializeField]
    UserPostFeature PostHandler;
    private void OnEnable()
    {
        PostHandler.OnUpdatePostText += UpdateText;
    }
    private void OnDisable()
    {
        PostHandler.OnUpdatePostText -= UpdateText;
    }
    void Start()
    {
        PostHandler.GetLatestPost(PostText);
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt(cameraTransform);
    }
    public void UpdateText(string txt)
    {
        PostText.text = txt;
    }
}