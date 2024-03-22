using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyWorldItemFlagHandler : MonoBehaviour
{
    bool ActiveFlag = true;
    public bool ActivityInApp()
    {
        return ActiveFlag;
    }
    public void ActivityFlag(bool flag)
    {
        ActiveFlag = flag;
    }
}
