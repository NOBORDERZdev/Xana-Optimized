using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public EmoteManager EmoteManager;
    public ReactionManager ReactionManager;

    public void Start()
    {
        EmoteManager.GetServerData();
        ReactionManager.GetServerData();
    }
    public void OpenEmoteDialogUI()
    {
        EmoteManager.OpenEmoteDialogUI();
    }
    public void OpenReactionDialogUI()
    {
        ReactionManager.OpenReactionDialogUI();
    }
}
