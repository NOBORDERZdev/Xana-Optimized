using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AirinController : MonoBehaviour
{
    public enum RotateType { Linear, Smooth }
    public UnityEvent<string> AirinAlertAction;

    [SerializeField]
    private RotateType _rotateType;
    [SerializeField]
    [Range(0, 5)]
    private float RotationSpeed = 2.0f;
    [SerializeField]
    [Space(2)]
    private bool IsAirinActivated = false;
    [SerializeField]
    private float maxDistance = 10.0f;
    private Transform _player;
    private int _playerViewId = 0;
    private Coroutine _coroutine;

    private void Start()
    {
        BuilderEventManager.AfterPlayerInstantiated += GetActivePlayer;
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= GetActivePlayer;
    }



    private void GetActivePlayer()
    {
        _player = ReferencesForGamePlay.instance.m_34player.transform;
    }

    private void OnMouseDown()
    {
        // Rotate Airin to face the player when clicked
        StartCoroutine(RotateTowardsPlayer());
        if (!IsAirinActivated)
        {
            IsAirinActivated = true;
            ConstantsHolder.xanaConstants.IsShowChatToAll = false;
            AirinAlertAction?.Invoke(XanaChatSystem.instance.UserName);
            _playerViewId = ArrowManager.Instance.gameObject.GetComponent<PhotonView>().ViewID;
            _coroutine = StartCoroutine(CalculateDistance());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>().ViewID == _playerViewId)
        {
            DeactivateAirin();
        }
    }

    private void DeactivateAirin()
    {
        IsAirinActivated = false;
        _playerViewId = 0;
        ConstantsHolder.xanaConstants.IsShowChatToAll = true;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    private IEnumerator RotateTowardsPlayer()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (_rotateType.Equals(RotateType.Linear))
            transform.rotation = targetRotation;
        else if (_rotateType.Equals(RotateType.Smooth))
        {
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
                // Wait for the next frame
                yield return null;
            }
            transform.rotation = targetRotation;
        }
    }

    private IEnumerator CalculateDistance()
    {
        while (IsAirinActivated)
        {

            Vector3 direction = _player.position - transform.position;
            if (direction.magnitude > maxDistance)
            {
                DeactivateAirin();
                yield break; 
            }
            direction.y = 0;
            yield return new WaitForSeconds(0.5f);
        }
    }


}
