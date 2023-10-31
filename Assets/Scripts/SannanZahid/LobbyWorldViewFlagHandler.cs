using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyWorldViewFlagHandler : MonoBehaviour
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
