using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PMY {
    public class RoomA_Logic : MonoBehaviour
    {
        public int nftIndex;
        public UnityEvent onExitAction;

        private void Start()
        {
            PMY_Nft_Manager.Instance.exitClickedAction += ExitBtnClicked;
        }
        private void OnDisable()
        {
            PMY_Nft_Manager.Instance.exitClickedAction -= ExitBtnClicked;
        }

        private void ExitBtnClicked(int num)
        {
            if (num == nftIndex)
                onExitAction.Invoke();
        }

    }
}
