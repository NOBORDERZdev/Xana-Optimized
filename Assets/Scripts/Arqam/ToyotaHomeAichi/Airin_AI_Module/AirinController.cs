using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

public class AirinController : MonoBehaviour
{
    public enum RotateType { Linear, Smooth }
    [Range(0,5)]
    public float RotationSpeed = 2.0f;
    [Space(2)]
    public bool IsAirinActivated = false;
    public UnityEvent<string> AirinAlertAction;

    [SerializeField]
    private RotateType _rotateType;
    private Transform player;

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
        player = ReferencesForGamePlay.instance.m_34player.transform;
    }

    private void OnMouseDown()
    {
        // Rotate Airin to face the player when clicked
        StartCoroutine(RotateTowardsPlayer());
        if (!IsAirinActivated)
        {
            IsAirinActivated = true;
            ConstantsHolder.xanaConstants.isShowChatToAll = false;
            AirinAlertAction?.Invoke(XanaChatSystem.instance.UserName);
        }
    }

    private IEnumerator RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
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


}
