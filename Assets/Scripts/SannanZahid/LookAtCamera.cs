using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
    public Transform _playerTransform;
    [SerializeField]
    Transform boxerplayerTransform;
    [SerializeField]
    public Transform newplayerTransform;
    public Vector3 Offset;
    bool SnapToPosition = false;
    Vector3 LastDisablePosition = default;

    Coroutine routine;
    private void OnEnable()
    {
        if (SnapToPosition)
            transform.position = LastDisablePosition;

        SnapToPosition = false;
        _postHandler.OnUpdatePostText += UpdateText;
        BoxerNFTEventManager.OnNFTequip += (nft) => _playerTransform = boxerplayerTransform.transform;
        BoxerNFTEventManager.OnNFTUnequip += () => _playerTransform = newplayerTransform.transform;
    }
    private void OnDisable()
    {
        LastDisablePosition = transform.position;
        SnapToPosition = true;
        _postHandler.OnUpdatePostText -= UpdateText;
        BoxerNFTEventManager.OnNFTequip -= (nft) => boxerplayerTransform = boxerplayerTransform.transform;
        BoxerNFTEventManager.OnNFTUnequip -= () => _playerTransform = newplayerTransform.transform;

    }
    void Start()
    {
        GetLatestPost();
    }
    public void GetLatestPost()
    {
        _postHandler.GetLatestPost(_postText);
    }
    void Update()
    {
        transform.position = _playerTransform.position + Offset;//Vector3.MoveTowards(transform.position, _playerTransform.position + Offset, Time.deltaTime);
    }
    public void UpdateText(string txt)
    {
        if (txt != "")
        {
            _postText.text = txt;
            _postHandler.InsertNewlines(_postText);
        }
    }
}