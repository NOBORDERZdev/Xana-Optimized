using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PMY {
    public class RoomA_Logic : MonoBehaviour
    {
        public int nftIndex;
        public UnityEvent onExitAction;

        private void Awake()
        {
            BuilderEventManager.AfterWorldOffcialWorldsInatantiated += HookEvent;          
        }

        private void HookEvent()
        {          
            PMY_Nft_Manager.Instance.exitClickedAction += ExitBtnClicked;
        }

        private void OnDisable()
        {
            PMY_Nft_Manager.Instance.exitClickedAction -= ExitBtnClicked;
            BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= HookEvent;
        }

        private void ExitBtnClicked(int num)
        {
            if (num == nftIndex)
                onExitAction.Invoke();
        }

    }
}
