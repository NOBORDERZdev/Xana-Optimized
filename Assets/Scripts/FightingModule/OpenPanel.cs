using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public void OpenLayouPanel() {
        FightingGameManager.activateLayoutPanel?.Invoke();
    }
}
