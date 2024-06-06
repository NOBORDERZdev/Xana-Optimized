using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class AirinController : MonoBehaviour
{
 
    public UnityEvent<string> AirinAlertAction;

    [SerializeField]
    [Range(0, 5)]
    private float _rotationSpeed = 2.0f;
    [SerializeField]
    private bool _isAirinActivated = false;
    [SerializeField]
    private float maxDistance = 10.0f;
    private Transform _player;
    private Quaternion _startRot;
    private AnimatorController _animController;
    private Coroutine _distanceCor;
    private Coroutine _rotateCor;
    private enum RotateType { Linear, Smooth }


    private void Start()
    {
        _animController = GetComponent<AnimatorController>();
        BuilderEventManager.AfterPlayerInstantiated += GetActivePlayer;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= GetActivePlayer;
    }


    private void GetActivePlayer()
    {
        _startRot = transform.rotation;
        _player = ReferencesForGamePlay.instance.m_34player.transform;
    }

    private void OnMouseDown()
    {
        if (!_isAirinActivated)
        {
            // Rotate Airin to face the player when clicked
            _rotateCor = StartCoroutine(RotateTowardsPlayer(_player.position, RotateType.Smooth));
            _isAirinActivated = true;
            ConstantsHolder.xanaConstants.IsShowChatToAll = false;
            AirinAlertAction?.Invoke(XanaChatSystem.instance.UserName);
            _distanceCor = StartCoroutine(CalculateDistance());
        }
    }

    private void DeactivateAirin()
    {
        _isAirinActivated = false;
        //_playerViewId = 0;
        ConstantsHolder.xanaConstants.IsShowChatToAll = true;
        if (_distanceCor != null)
        {
            StopCoroutine(_distanceCor);
        }
        if (_rotateCor != null)
        {
            StopCoroutine(_rotateCor);
        }
        StartCoroutine(RotateToOriginalPosition());
    }

    private IEnumerator CalculateDistance()
    {
        while (_isAirinActivated)
        {

            Vector3 direction = _player.position - transform.position;
            if (direction.magnitude > maxDistance)
            {
                DeactivateAirin();
                yield break;
            }
            direction.y = 0;
            yield return new WaitForSeconds(0.5f);
            if (_rotateCor == null)
            {
                _rotateCor = StartCoroutine(RotateTowardsPlayer(_player.position, RotateType.Linear));
            }
        }
    }
    private IEnumerator RotateTowardsPlayer(Vector3 targetPos, RotateType _rotateType)
    {
        Debug.LogError("call: " + _rotateType);
        Vector3 direction = targetPos - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (_rotateType.Equals(RotateType.Linear))
        {
            transform.rotation = targetRotation;
        }
        else if (_rotateType.Equals(RotateType.Smooth))
        {
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
                // Wait for the next frame
                yield return null;
            }
            transform.rotation = targetRotation;
        }
        _rotateCor = null;
    }
    private IEnumerator RotateToOriginalPosition()
    {
        // Rotate back to the original position smoothly
        while (Quaternion.Angle(transform.rotation, _startRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _startRot, Time.deltaTime * _rotationSpeed);
            yield return null;
        }
        // Ensure the final rotation is exactly the original rotation
        transform.rotation = _startRot;
    }

}
