using PMY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomA_Nft_Interactbility : MonoBehaviour
{
    [Tooltip("Retrieve the index of the NFT for enabling buttons interactivity upon clicking.")]
    public int nftIndex;
    [Space(5)]
    public Button[] btns;


    private void OnEnable()
    {
        PMY_Nft_Manager.Instance.exitClickedAction += NftExitBtnClicked;
    }

    public void OffBtnInteractibility()
    {
        btns = GetComponentsInChildren<Button>();
        foreach (Button btn in btns)
            btn.interactable = false;
    }

    private void OnDisable()
    {
        PMY_Nft_Manager.Instance.exitClickedAction -= NftExitBtnClicked;
    }

    private void NftExitBtnClicked(int num)
    {
        if (num == nftIndex)
        {
            foreach (Button btn in btns)
                btn.interactable = true;

            PMY_Nft_Manager.Instance.exitClickedAction -= NftExitBtnClicked;
        }
    }

}
