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
    [SerializeField]
    Transform boxerplayerTransform;
    [SerializeField]
    Transform newplayerTransform;
    public Vector3 Offset;
    bool SnapToPosition = false;
    Vector3 LastDisablePosition = default;
    private void OnEnable()
    {
        if(SnapToPosition)
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
        _postHandler.GetLatestPost(_postText);
    }
    void Update()
    {
        transform.position = _playerTransform.position + Offset;//Vector3.MoveTowards(transform.position, _playerTransform.position + Offset, Time.deltaTime);
    }
    public void UpdateText(string txt)
    {
        if(txt != "")
        _postText.text = txt;
    }
}