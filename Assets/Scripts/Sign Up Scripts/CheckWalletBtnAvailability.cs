using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWalletBtnAvailability : MonoBehaviour
{
    public GameObject walletBtn;
    public GameObject walletBtnPos;

    private void OnEnable()
    {
        if (walletBtn.activeSelf)
            walletBtnPos.SetActive(true);
        else if (!walletBtn.activeSelf)
            walletBtnPos.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
