using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementSetter : MonoBehaviour
{
    public void PlayerMovementBehaviourPlayer()
    {
        GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(false);
    }
    public void PlayerMovementBehaviourStop()
    {
        GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
    }
}
