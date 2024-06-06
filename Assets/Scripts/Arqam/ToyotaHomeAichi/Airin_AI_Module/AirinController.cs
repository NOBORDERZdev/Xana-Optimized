using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AirinController : MonoBehaviour
{

    public UnityEvent<string> AirinAlertAction;
    public UnityEvent AirinDeActivate;

    [SerializeField]
    [Range(0, 5)]
    private float _rotationSpeed = 2.0f;
    [SerializeField]
    private bool _isAirinActivated = false;
    [SerializeField]
    private float _maxDistance = 10.0f;
    private Transform _player;
    private Quaternion _startRot;
    private Animator _animator;
    private Coroutine _distanceCor;
    private Coroutine _rotateCor;
    private enum RotateType { Linear, Smooth }


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _startRot = transform.rotation;
        _player = ReferencesForGamePlay.instance.m_34player.transform;
    }


    private void OnMouseDown()
    {
        if (!_isAirinActivated)
        {
            // Rotate Airin to face the player when clicked
            _rotateCor = StartCoroutine(RotateTowardsPlayer(_player.position, RotateType.Linear, true));
            _isAirinActivated = true;
            ConstantsHolder.xanaConstants.IsShowChatToAll = false;
            AirinAlertAction?.Invoke(XanaChatSystem.instance.UserName);
            _distanceCor = StartCoroutine(CalculateDistance());
        }
        else
        {
            _animator.SetTrigger("hy");
        }
    }

    private void DeactivateAirin()
    {
        _isAirinActivated = false;
        AirinDeActivate.Invoke();
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
            if (direction.magnitude > _maxDistance)
            {
                DeactivateAirin();
                yield break;
            }
            direction.y = 0;
            yield return new WaitForSeconds(0.5f);
            if (_rotateCor == null)
            {
                _rotateCor = StartCoroutine(RotateTowardsPlayer(_player.position, RotateType.Linear, false));
            }
        }
    }
    private IEnumerator RotateTowardsPlayer(Vector3 targetPos, RotateType _rotateType, bool _isGreeting)
    {
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
        if (_isGreeting)
        {
            _animator.SetTrigger("active");
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
