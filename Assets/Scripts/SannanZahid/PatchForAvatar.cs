using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForAvatar : MonoBehaviour
{

    void Update()
    {
        GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(true);
    }
    private void OnDisable()
    {
        GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().UpdateState(false);
    }
}
