using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
//using Cysharp.Threading.Tasks;

public class CryptouserData : MonoBehaviour
{
    public static CryptouserData instance;
    public bool CryptoLogin;
    public string UserAddress;
    public string UserBalance;
    //public UserNFTlistClass.MoralisAPIBaseClass NFTListObjBSC;
    //public UserNFTlistClass.MoralisAPIBaseClass NFTListObjETH;
    //public UserNFTlistClass.MoralisAPIBaseClass NFTListObjPolygon;
  //  public UserNFTlistClass.Root NFTlistdata;

    public bool UltramanPass;
    public bool AstroboyPass;
    public bool AlphaPass;
    public bool Testnet;
    public string ConditionStringFromServer;

 
    private  void Awake()
    {
        if (instance == null)
        {
            instance = this;   
            
            //if (APIBasepointManager.instance != null)
            //{
            //    if (APIBasepointManager.instance.IsXanaLive) // Mainnet
            //    {
                        
            //    }
            //    else  // testnet
            //    {
                   
            //    }
            //}

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }   
    }   
   
    public void AfterLogout()
    {
        //NFTListObjBSC = new UserNFTlistClass.MoralisAPIBaseClass();
        //NFTListObjETH = new UserNFTlistClass.MoralisAPIBaseClass();
        //NFTListObjPolygon = new UserNFTlistClass.MoralisAPIBaseClass();
        //NFTListObjBSC = null;
        //NFTListObjETH = null;
        //NFTListObjPolygon = null;
        Debug.LogError("aaaaa");
        UltramanPass = false;
        AstroboyPass = false;
        AlphaPass = false;
        ConditionStringFromServer = "";
    }

}
