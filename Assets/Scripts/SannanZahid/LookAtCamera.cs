using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UserPostFeature;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField]
    Transform _cameraTransform;
    [SerializeField]
    TMPro.TMP_Text _postText;
    [SerializeField]
    UserPostFeature _postHandler;
    [SerializeField]
    Transform _playerTransform;
    public Vector3 Offset;
    private void OnEnable()
    {
        _postHandler.OnUpdatePostText += UpdateText;
    }
    private void OnDisable()
    {
        _postHandler.OnUpdatePostText -= UpdateText;
    }
    void Start()
    {
        _postHandler.GetLatestPost(_postText);
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position + Offset, 0.5f);
        //transform.LookAt(_cameraTransform);
    }
    public void UpdateText(string txt)
    {
        _postText.text = txt;
    }
}