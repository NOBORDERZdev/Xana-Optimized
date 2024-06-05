using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;

public class AirinController : MonoBehaviour
{
    public enum RotateType { Linear, Smooth }
    public RotateType rotateType;
    [Range(0,5)]
    public float rotationSpeed = 2.0f;
    [Space(2)]
    public bool isAirinActivated = false;
    public UnityEvent<string> airinAction;

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

    void OnMouseDown()
    {
        // Rotate Airin to face the player when clicked
        StartCoroutine(RotateTowardsPlayer());
        if (!isAirinActivated)
        {
            isAirinActivated = true;
            ConstantsHolder.xanaConstants.isShowChatToAll = false;
            airinAction?.Invoke(XanaChatSystem.instance.UserName);
        }
    }

    IEnumerator RotateTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (rotateType.Equals(RotateType.Linear))
            transform.rotation = targetRotation;
        else if (rotateType.Equals(RotateType.Smooth))
        {
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                // Wait for the next frame
                yield return null;
            }
            transform.rotation = targetRotation;
        }
    }


}
