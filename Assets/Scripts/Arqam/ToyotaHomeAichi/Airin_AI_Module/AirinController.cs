using UnityEngine;
using UnityEngine.Events;

public class AirinController : MonoBehaviour
{
    // Reference to the player
    public Transform player;
    public bool isAirinActivated = false;
    public UnityEvent<string> airinAction;

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
        RotateTowardsPlayer();
        if (!isAirinActivated)
        {
            isAirinActivated = true;
            ConstantsHolder.xanaConstants.isShowChatToAll = false;
            airinAction?.Invoke(XanaChatSystem.instance.UserName);
        }
    }

    void RotateTowardsPlayer()
    {
        // Calculate the direction from Airin to the player
        Vector3 direction = player.position - transform.position;
        // Set the y component to 0 to keep the rotation only in the horizontal plane
        direction.y = 0;
        // Calculate the rotation needed to face the player
        Quaternion rotation = Quaternion.LookRotation(direction);
        // Apply the rotation to Airin
        transform.rotation = rotation;
    }

}
