using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using MoralisUnity.Kits.AuthenticationKit;
public class UserNFTlistClass : MonoBehaviour
{
    public static UserNFTlistClass instance;
     public static event Action<string> AllDataFetchedfromServer;

    /*
        public OwnedNFTContainer _OwnedNFTDataObj;   
     public string localAPI = "";
    public int NFTpageNumb = 1;
    public int NFTpageSize = 20;
    public string BaseURLNFTList = "https://deep-index.moralis.io/api/v2/";
    public string BSCChainList = "/nft?chain=bsc";
    public string EthChainList = "/nft?chain=eth";
    public string PolygonChainList = "/nft?chain=polygon";
    //   public string API = "https://deep-index.moralis.io/api/v2/0xF86A9608EeF996B20cC79F95a7d2A0D50936Eb33/nft?chain=bsc";
     // https://deep-index.moralis.io/api/v2/{pblic_address}/nft/0x91D7742f1e44bA6CDA5DE2a1590858d20876226C?chain=bsc&format=decimal
    // https://deep-index.moralis.io/api/v2/{pblic_address}/nft/0xc8015EC9bf67d66A7194542Fc72fC0430d0D9b2c?chain=eth&format=decimal
    // https://deep-index.moralis.io/api/v2/{pblic_address}/nft/0x5813939c463bF035614A0b6921f7404E471934F0?chain=polygon&format=decimal
    public string BscTokenAddress = "/nft/0x91D7742f1e44bA6CDA5DE2a1590858d20876226C?chain=bsc&format=decimal";
     public string EthTokenAddress = "/nft/0xc8015EC9bf67d66A7194542Fc72fC0430d0D9b2c?chain=eth&format=decimal";
    public string PolygonTokenAddress = "/nft/0x5813939c463bF035614A0b6921f7404E471934F0?chain=polygon&format=decimal";
    private UserNFTlistClass.MoralisAPIBaseClass BSCChainUltramanObj;
    private UserNFTlistClass.MoralisAPIBaseClass BSCChainAlphaObj;
    private UserNFTlistClass.MoralisAPIBaseClass ETHChainUltramanObj;
    private UserNFTlistClass.MoralisAPIBaseClass PolygonChainUltramanObj;
     // Astroboy Work
    public string EthTokenAddress_Astroboy = "/nft/0xD972F71a1F307354334EEBefa80bb6B236Ffb6a4?chain=eth&format=decimal";
    public string BscTokenAddress_Astroboy = "/nft/0x47f2d74B5180eA8c1566d1573D7B3F2b7EB1A170?chain=bsc&format=decimal";
    public string BscTokenAddressForALPHAPASS = "/nft/0xE737d5A35A41fFd6072503BCA9C3013632287305?chain=bsc&format=decimal";
    private bool OnceUltraman;
    private bool OnceAstroboy;
    private string BaseURLTestNet_Astroboy = "https://api-test.xana.net/item/";
    private string BaseURLMainNet_Astroboy = "https://app-api.xana.net/item/";

    private string AstroboyGettingStringAPI = "get-nft-item";
    private UserNFTlistClass.MoralisAPIBaseClass BSCChainAstroboyObj;
 // "https://prod-backend.xanalia.com/nfts/nft-by-address-user?pageIndex=1&pageSize=30&address=0x45e74fE6b2fA114604DaE462E20fFa755353223e&categoryFilter=2&userId=0";


    public string OwnednftAPIMainNet=  "https://prod-backend.xanalia.com/nfts/nft-by-address-user?pageIndex={0}&pageSize={1}&address=";
    public string OwnednftAPITestNet = "https://backend.xanalia.com/nfts/nft-by-address-user?pageIndex={0}&pageSize={1}&address=";

    //Test SpecifiedCase
     //https://prod-backend.xanalia.com/nfts/nft-by-address-user-tcg?address=0xec943d84e658f3972a35e62558702c2d7c74290d&pageIndex=1&pageSize=30&name=breaking down

    public string OwnedSpecifiednftAPIMainNet = "https://prod-backend.xanalia.com/nfts/nft-by-address-user-tcg?address=";
    public string OwnedSpecifiednftAPITestNet = "https://backend.xanalia.com/nfts/nft-by-address-user-tcg?address=";
    private string SpecifiedNFTPostFix = "&pageIndex=1&pageSize=30&name=breaking down";
 
    private string PostFix = "&categoryFilter=2&sortFilter=2";
    public bool TestSpecificCase;

    public string TestSpecificAddress;

    //public userRoleScript userRoleObj;
    */
    private void Awake()   
    {
        instance = this;
        //OnceUltraman = false;
        //OnceAstroboy = false;
        //Debug.Log("NFTList class Name: " + gameObject.name);
    }
    public async Task CallOwnedNFTListAPIAsync()
    {
        await Task.Delay(1500);
        AuthenticationKit.Instance.LoaderObj.SetActive(false);
        print("All data Fetched here");
        AllDataFetchedfromServer?.Invoke("Web3");
    }  

    /*
    public async Task CallNFTListAPIAsync()
    {
      string localAPI = BaseURLNFTList + CryptouserData.instance.UserAddress + BSCChainList;   
        //     string localAPI = BaseURLNFTList + "0x9da03695C29D8896a764974105B686b02BF52637" + BSCChainList;             
        UnityWebRequest request;    
        request = await MoralisGettingList(localAPI);
        if (request.downloadHandler.text.Contains("Invalid key"))
        {
           Debug.Log("hey Invalid NFT list");
        }   
        else
        {
            CryptouserData.instance.NFTListObjBSC = new MoralisAPIBaseClass();
            CryptouserData.instance.NFTListObjBSC = MoralisAPIBaseClass.CreateFromJSON(request.downloadHandler.text);
            print("Total Number of NFTs are : " + CryptouserData.instance.NFTListObjBSC.total);
        }
        string localAPI2 = BaseURLNFTList + CryptouserData.instance.UserAddress + EthChainList;
        // string localAPI2 = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + EthChainList;
        UnityWebRequest request2;
        request2 = await MoralisGettingList(localAPI2);
        if (request2.downloadHandler.text.Contains("Invalid key"))
        {
           Debug.Log("hey Invalid NFT list");
        }
        else
        {
            CryptouserData.instance.NFTListObjETH = new MoralisAPIBaseClass();
            CryptouserData.instance.NFTListObjETH = MoralisAPIBaseClass.CreateFromJSON(request2.downloadHandler.text);
            print("Total Number of NFTs are : " + CryptouserData.instance.NFTListObjETH.total);
        }

        string localAPI3 = BaseURLNFTList + CryptouserData.instance.UserAddress + PolygonChainList;
        //  string localAPI3 = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + PolygonChainList;
        UnityWebRequest request3;
        request3 = await MoralisGettingList(localAPI3);
        if (request3.downloadHandler.text.Contains("Invalid key"))
        {
           Debug.Log("hey Invalid NFT list");
        }
        else
        {
            CryptouserData.instance.NFTListObjPolygon = new MoralisAPIBaseClass();
            CryptouserData.instance.NFTListObjPolygon = MoralisAPIBaseClass.CreateFromJSON(request3.downloadHandler.text);
            print("Total Number of NFTs in polygon chain is = : " + CryptouserData.instance.NFTListObjPolygon.total);
        }

        print("Eth " + CryptouserData.instance.NFTListObjETH.total);
        if (CryptouserData.instance.NFTListObjETH.total > 0)
        {
            print("In eth total NFTs");
            string localAPIETHChain = BaseURLNFTList + CryptouserData.instance.UserAddress + EthTokenAddress;
            //     string localAPIETHChain = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + EthTokenAddress;
            UnityWebRequest requestChain;
            requestChain = await MoralisGettingList(localAPIETHChain);
            if (requestChain.downloadHandler.text.Contains("Invalid key"))
            {
               Debug.Log("hey Invalid NFT list");
            }
            else
            {
                ETHChainUltramanObj = new MoralisAPIBaseClass();
                ETHChainUltramanObj = MoralisAPIBaseClass.CreateFromJSON(requestChain.downloadHandler.text);
                print("Total Number of NFTs In Etherium chain when filter : " + ETHChainUltramanObj.total);

                if (ETHChainUltramanObj.total > 0)
                {
                    print("ultraman is true here in ETH");
                    CryptouserData.instance.UltramanPass = true;
                    OnceUltraman = true;
                     //SceneManager.LoadScene("SplashScene");
                    //return;
                }
            }
        }

        if (CryptouserData.instance.NFTListObjPolygon.total > 0 && !OnceUltraman)
        {
            string localAPIPolygonChain = BaseURLNFTList + CryptouserData.instance.UserAddress + PolygonTokenAddress;
            // string localAPIPolygonChain = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + PolygonTokenAddress;
            UnityWebRequest requestChain;
            requestChain = await MoralisGettingList(localAPIPolygonChain);
            if (requestChain.downloadHandler.text.Contains("Invalid key"))
            {
               Debug.Log("hey Invalid NFT list");
            }
            else
            {
                PolygonChainUltramanObj = new MoralisAPIBaseClass();
                PolygonChainUltramanObj = MoralisAPIBaseClass.CreateFromJSON(requestChain.downloadHandler.text);
                print("Total Number of NFTs are : " + PolygonChainUltramanObj.total);
                if (PolygonChainUltramanObj.total > 0)
                {
                    print("ultraman is true here22 in polygon ");

                    CryptouserData.instance.UltramanPass = true;
                    OnceUltraman = true;

                    //SceneManager.LoadScene("SplashScene");
                    //return;
                }
            }
        }
        if (CryptouserData.instance.NFTListObjBSC.total > 0 && !OnceUltraman)
        {
            print("ultraman here in BSC  00 ");    
                
            string localAPIBSCChain = BaseURLNFTList + CryptouserData.instance.UserAddress + BscTokenAddress;
            //    string localAPIBSCChain = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + BscTokenAddress;
            UnityWebRequest requestChain;
            requestChain = await MoralisGettingList(localAPIBSCChain);
            if (requestChain.downloadHandler.text.Contains("Invalid key"))
            {
               Debug.Log("hey Invalid NFT list");
            }
            else
            {
                BSCChainUltramanObj = new MoralisAPIBaseClass();
                BSCChainUltramanObj = MoralisAPIBaseClass.CreateFromJSON(requestChain.downloadHandler.text);
                print("Total Number of NFTs are : " + BSCChainUltramanObj.total);
                if (BSCChainUltramanObj.total > 0)
                {         
                    //if (BSCChainUltramanObj.result.Count > 0)
                    //{
                    //    for (int i = 0; i < BSCChainUltramanObj.result.Count; i++)
                    //    {
                    //        string upperCaseName = BSCChainUltramanObj.result[i].metadata.ToString().ToUpper();
                    //        if (upperCaseName.Contains("ULTRAMAN"))
                    //        {
                                print("ultraman is true here33 in BSC ");        
                                 CryptouserData.instance.UltramanPass = true;   
                                OnceUltraman = true;        
                                //SceneManager.LoadScene("SplashScene");
                                //return;
                           // }   
                        //}
                    //}
                }
            }
        }


        //  Astroboy // 

        if (CryptouserData.instance.NFTListObjETH.total > 0)
        {
            string localAPIETHChain = BaseURLNFTList + CryptouserData.instance.UserAddress + EthTokenAddress_Astroboy;
            //   string localAPIETHChain = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + EthTokenAddress_Astroboy;  
            UnityWebRequest requestChain;
            requestChain = await MoralisGettingList(localAPIETHChain);
            if (requestChain.downloadHandler.text.Contains("Invalid key"))
            {
               Debug.Log("hey Invalid NFT list");
            }
            else
            {   
                MoralisAPIBaseClass ETHChainAstroboy = new MoralisAPIBaseClass();
                ETHChainAstroboy = MoralisAPIBaseClass.CreateFromJSON(requestChain.downloadHandler.text);
                print("Astroboy in ETH NFTs are :  : " + ETHChainAstroboy.total);
                print(requestChain.downloadHandler.text);
                if (ETHChainAstroboy.total > 0)
                {
                    CryptouserData.instance.AstroboyPass = true;
                    OnceAstroboy = true;
                    //SceneManager.LoadScene("SplashScene");
                    //return;
                }   
            }
        }

        // BSC

        if (CryptouserData.instance.NFTListObjBSC.total > 0 && !OnceAstroboy)
        {
            string localAPIBSCChain = BaseURLNFTList + CryptouserData.instance.UserAddress + BscTokenAddress_Astroboy;
            //  string localAPIBSCChain = BaseURLNFTList + "0x4dad2d694615df9003dcf09df662ecf049a1e51b" + BscTokenAddress_Astroboy;

            UnityWebRequest requestChain;
            requestChain = await MoralisGettingList(localAPIBSCChain);
            if (requestChain.downloadHandler.text.Contains("Invalid key"))
            {
               Debug.Log("hey Invalid NFT list");
            }
            else
            {
                MoralisAPIBaseClass BSCChainAstroboyObj1 = new MoralisAPIBaseClass();
                BSCChainAstroboyObj1 = MoralisAPIBaseClass.CreateFromJSON(requestChain.downloadHandler.text);
                print("Astroboy in BSC NFTs are : " + BSCChainAstroboyObj1.total);
                print(requestChain.downloadHandler.text);

                if (BSCChainAstroboyObj1.total > 0)
                {
                    CryptouserData.instance.AstroboyPass = true;
                    OnceAstroboy = true;
                    //SceneManager.LoadScene("SplashScene");
                    //return;   
                }
            }
        }


        // AlphaPass in BSC network
        if (CryptouserData.instance.NFTListObjBSC.total > 0)
        {
            string localAPIBSCChain = BaseURLNFTList + CryptouserData.instance.UserAddress + BscTokenAddressForALPHAPASS;
            //  string localAPIBSCChain = BaseURLNFTList + "0x50374a69c5a9e6e8024f7bba10ba0f9a64a65151" + BscTokenAddress;
            UnityWebRequest requestChain;
            requestChain = await MoralisGettingList(localAPIBSCChain);
            if (requestChain.downloadHandler.text.Contains("Invalid key"))
            {
               Debug.Log("hey Invalid NFT list");
            }
            else
            {
                BSCChainAlphaObj = new MoralisAPIBaseClass();
                BSCChainAlphaObj = MoralisAPIBaseClass.CreateFromJSON(requestChain.downloadHandler.text);
                print("Total Number of NFTs ALPHAPASS NFT's in List are : " + BSCChainAlphaObj.total);
                if (BSCChainAlphaObj.total > 0)
                {
                    if (BSCChainAlphaObj.result.Count > 0)
                    {
                        for (int i = 0; i < BSCChainAlphaObj.result.Count; i++)
                        {
                            // string upperCaseName = BSCChainUltramanObj.result[i].metadata.ToUpper();
                            if (BSCChainAlphaObj.result[i].name.Contains("Alpha pass"))
                            {
                                print("Contained Alpha Pass");
                                CryptouserData.instance.AlphaPass = true;
                                //  SceneManager.LoadScene("Home");
                                //  return;
                            }
                            else
                            {
                                // Toast.Show("You Need Alpha Pass to Login");
                                //     SceneManager.LoadScene("Morali");
                            }
                        }
                    }
                }
                else
                {
                    //  Toast.Show("You Need Alpha Pass to Login");
                    //  SceneManager.LoadScene("Morali");
                }

            }
        }



        string APIGetString;
        if (CryptouserData.instance.Testnet)
        {

            APIGetString = BaseURLTestNet_Astroboy + AstroboyGettingStringAPI;
        }
        else
        {
            APIGetString = BaseURLMainNet_Astroboy + AstroboyGettingStringAPI;
        }
        UnityWebRequest requestContition;
        requestContition = await GettingContitionFromServer(APIGetString);
        print(requestContition.downloadHandler.text);

        AstroboyRoot AstroboyAPIString = new AstroboyRoot();
        AstroboyAPIString = AstroboyRoot.CreateFromJSON(requestContition.downloadHandler.text);
        if (AstroboyAPIString.success)
        {
            CryptouserData.instance.ConditionStringFromServer = AstroboyAPIString.data.name;
        }

        print(" result is  CryptouserData.instance.NFTListObjBSC : " + CryptouserData.instance.NFTListObjBSC.result.Count);    
        if (CryptouserData.instance.NFTListObjBSC.result.Count> 0)
        {
            print("1st appearance "+ CryptouserData.instance.NFTListObjBSC.result);   
               
            for(int i =0; i <CryptouserData.instance.NFTListObjBSC.result.Count; i++)
            {
                print("2nd appearance : "+ CryptouserData.instance.NFTListObjBSC.result[i].metadata);  

                if (CryptouserData.instance.NFTListObjBSC.result[i].metadata != "")
                {
                    print(CryptouserData.instance.NFTListObjBSC.result[i].metadata);
                    Rootmeta metedataObj = new Rootmeta();
                    metedataObj = Rootmeta.CreateFromJSON(CryptouserData.instance.NFTListObjBSC.result[i].metadata);
                    print("Total Number of NFTs ALPHAPASS NFT's in List are : " + metedataObj.image);
                 }   
             }                  
            //for (int i = 0; i < CryptouserData.instance.NFTListObjBSC.total; i++)      
            //{    
            //    string link = BSCChainUltramanObj.result[i].token_uri.ToUpper();
            //    print("link  " + link);
            //}
        }     
           

        //  ******* //   
        AllDataFetchedfromServer?.Invoke();
        //   SceneManager.LoadScene("Main");  
    }

    */
    /*
   public async Task CallOwnedNFTListAPIAsync()
  {

      localAPI = "";
      Debug.Log("NFTList class Name: " + gameObject.name);
      if (CryptouserData.instance.Testnet)
      {
          if (TestSpecificCase)
          {
            //  localAPI = string.Format(OwnednftAPIMainNet, NFTpageNumb, NFTpageSize) + TestSpecificAddress + PostFix;
              localAPI = OwnedSpecifiednftAPIMainNet + TestSpecificAddress + SpecifiedNFTPostFix;
          }
          else
          {
              //  localAPI = string.Format(OwnednftAPITestNet, NFTpageNumb, NFTpageSize) + CryptouserData.instance.UserAddress + PostFix;
              localAPI = OwnedSpecifiednftAPITestNet + CryptouserData.instance.UserAddress + SpecifiedNFTPostFix;
           }  
          //   localAPI = OwnednftAPIMainNet + "0xb625ba9f8296f5ebac4e06edd0862777a548a7f7" + OwnednftAPIMainNet2;
          //    localAPI = OwnednftAPITestNet + "0x138E3Dd54e5C3CB174F88232ad3fBba81331Db4b" + OwnednftAPIMainNet2;
      }  
      else
      {
          if (TestSpecificCase)
          {  
              //localAPI = string.Format(OwnednftAPIMainNet, NFTpageNumb, NFTpageSize) + TestSpecificAddress + PostFix;
              localAPI = OwnedSpecifiednftAPIMainNet + TestSpecificAddress + SpecifiedNFTPostFix;
           }
          else
          {
              //  localAPI = string.Format(OwnednftAPIMainNet, NFTpageNumb, NFTpageSize) + CryptouserData.instance.UserAddress + PostFix;
              localAPI = OwnedSpecifiednftAPIMainNet + CryptouserData.instance.UserAddress + SpecifiedNFTPostFix;

          }
      }
      //     string localAPI = BaseURLNFTList + "0x9da03695C29D8896a764974105B686b02BF52637" + BSCChainList;             
      Debug.Log("NFTAPI :" + localAPI);
      UnityWebRequest request;
      request = await GettingOwnedNFTS(localAPI);

      */

    /*
    CryptouserData.instance.NFTlistdata = new Root();
    if (request.downloadHandler.text.Contains("Invalid key"))
   {
      Debug.Log("hey Invalid NFT list");
   }        
   else
   {  
        // CryptouserData.instance.NFTlistdata = Root.CreateFromJSON(request.downloadHandler.text);
         CryptouserData.instance.NFTlistdata = Root.CreateFromJSON(request.downloadHandler.text);  
        print("Total Number of NFTs are : " + CryptouserData.instance.NFTlistdata.count);    
       if (CryptouserData.instance.NFTlistdata.count >0)
       { 
           for (int i=0; i < CryptouserData.instance.NFTlistdata.list.Count; i++ )
           {  
               //print(CryptouserData.instance.NFTlistdata.list[i].name);
               //print(CryptouserData.instance.NFTlistdata.list[i].description);  
               string NFTname = CryptouserData.instance.NFTlistdata.list[i].name.ToLower();
               if (NFTname.Contains("XANA x BreakingDown"))
               {
                   print("BreakingDown");
               }      
              if (NFTname.Contains("alpha pass"))
               {
                   print("Contained Alpha Pass");   
                   CryptouserData.instance.AlphaPass = true;
                }   
               if (NFTname.Contains("astroboy"))
               {
                   print("Astroboy");  
                   CryptouserData.instance.AstroboyPass = true;
                }
               if (NFTname.Contains("ultraman"))
               {         
                   print("Contained Ultraman");

                   CryptouserData.instance.UltramanPass = true;
                }                   
            }
        }
    }

   */

    /*
     _OwnedNFTDataObj.NewRootInstance();
    if (request.downloadHandler.text.Contains("Invalid key"))
    {
       Debug.Log("hey Invalid NFT list");
    }  
    else
    {
        // CryptouserData.instance.NFTlistdata = Root.CreateFromJSON(request.downloadHandler.text);
        _OwnedNFTDataObj.CreateJsonFromRoot(request.downloadHandler.text);
         //Root.CreateFromJSON(request.downloadHandler.text);
        print("Total Number of NFTs are : " + CryptouserData.instance.NFTlistdata.count);
        if (_OwnedNFTDataObj.NFTlistdata.count > 0)
        {
            for (int i = 0; i < _OwnedNFTDataObj.NFTlistdata.list.Count; i++)
            {
                //print(CryptouserData.instance.NFTlistdata.list[i].name);
                //print(CryptouserData.instance.NFTlistdata.list[i].description);  
                string NFTname = _OwnedNFTDataObj.NFTlistdata.list[i].name.ToLower();
                if (NFTname.Contains("xana x breakingdown"))
                {
                    print("BreakingDown");
                }
                if (NFTname.Contains("alpha pass"))
                {
                    print("Contained Alpha Pass");
                    CryptouserData.instance.AlphaPass = true;
                }
                if (NFTname.Contains("astroboy"))
                {
                    print("Astroboy");
                    CryptouserData.instance.AstroboyPass = true;
                }
                if (NFTname.Contains("ultraman"))
                {
                    print("Contained Ultraman");
                     CryptouserData.instance.UltramanPass = true;
                }
            }  
        }  
    }
 
    await Task.Delay(1500);
         AuthenticationKit.Instance.LoaderObj.SetActive(false);  
        print("All data Fetched here");  
         AllDataFetchedfromServer?.Invoke("Web3");
      }       */
    /*
     public async Task<UnityWebRequest> MoralisGettingList(string url)
    {
        print(url);
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-API-Key", "7Wjy8amycKpdXUOjGmABd3jfi675o73sdcfDkOah0yguTu5Vk8Es8l5fakBdu4sA");
        await request.SendWebRequest();
         return request;
    }

    public async Task<UnityWebRequest> GettingOwnedNFTS(string url)
    {
        print(url);
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();

        print("return " + request.downloadHandler.text);
        return request;
    }
       

    public async Task<UnityWebRequest> GettingContitionFromServer(string url)
    {
        print(url);
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();

        print("return " + request.downloadHandler.text);
        return request;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class Attribute
    {
        public int id;
        public int nftId;
        public string body;
        public string clothes;
        public string earrings;
        public string eyes;
        public string glasses;
        public string facePaint;
        public string beautyMark;
        public string hairAccessories;
        public string hairs;
        public string plotSize;
        public string xxCoOrds;
        public string yyCoOrds;
        public string rarity;
        public string size;
        public string coOrds;
        public int landNft;
        public int collection_id;
        public object xana_builder_info;
        public string astro_type;
        public string astro_level;
        public string background;
        public string sake_cask;
        public string weapon;
        public string flipper;
        public string white_body;
        public string white_shoulder;
        public string white_flipper;
        public string white_face;
        public string foot;
        public string beak;
        public string belt_back;
        public string belt_front;
        public string rope;
        public string kimono;
        public string eye_cover;
        public string eye_brow;
        public string head;
        public string armor;
        public string cape;
        public string neck;
        public string rod;
        public int penpenz_nft;
        public string Eye_lense;
        public string Skin;
        public string Eye_Shapes;
        public string Lips;
        public string Eyelid;
        public string Face_Tattoo;
        public string Arm_Tattoo;
        public string legs_Tattoo;
        public string Forehead_Tattoo;
        public string Chest_Tattoo;
        public string Mustache;
        public string Pants;
        public string Eyebrows;
        public string Chains;
        public string Full_Costumes;
        public string Gloves;
        public string Shoes;
    }



    [Serializable]
    public class Collection
    {
        public int id;     
        public string name;
        public string address;
        public int collectionType;
        public string avatar;
    }

    [Serializable]
    public class Creator
    {
        public int userId;
        public string description;
        public string avatar;
        public string name;
        public object facebookLink;
        public object instagramLink;
        public object twitterLink;
        public int role;
        public string address;
    }

    [Serializable]
    public class List
    {
        public int nftId;
        public string name;
        public int category;
        public int standardType;
        public string description;
        public int marketNftStatus;
        public object tokenPrice;
        public int userId;
        public int collectionsId;
        public int networkId;
        public int isLock;
        public int noOfCopies;
        public string mediaUrl;
        public string tokenId;
        public int isMigrated;
        public string categoryName;
        public string price;
        public string totalLike;
        public DateTime createdAt;
        public string sprice;
        public int isLike;
        public Network network;
        public Creator creator;
        public Collection collection;
        public Owner owner;
        public SaleData saleData;
        public string tokenPriceIcon;
        public int ownedCopies;
        public Attribute attribute;
    }

    [Serializable]
    public class Network
    {
        public string networkName;
        public int networkId;
        public string avatar;
    }
    [Serializable]
    public class Owner
    {
        public int userId;
        public int nftId;
        public string name;
        public object avatar;
        public int role;
        public string address;
    }
  
    [Serializable]
       public class SaleData
    {
        public object fixPrice; 
        public object auction;
    }

    [Serializable]
    public class Root
    {
        public List<List> list;
        public int count;

        public static Root CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Root>(jsonString);
        }
    }   
     

    



    [Serializable]
    public class AstroboyData
    {
        public int id;
        public string name;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [Serializable]
    public class AstroboyRoot
    {
        public bool success;
        public AstroboyData data;
        public string msg;

        public static AstroboyRoot CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<AstroboyRoot>(jsonString);
        }
     }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class ExtendInfometa
    {
        public string videoUrl;  
    }  
    [Serializable]
    public class Propertiesmeta
    {
        public string type;
    }
    [Serializable]
    public class Rootmeta
    {
        public string name;
        public string description;
        public string image;
        public Propertiesmeta properties;
        public string totalSupply;
        public string externalLink;
        public string thumbnft;
        public string imageOld;
        public string animation_url;
        public ExtendInfometa extendInfo;

        public static Rootmeta CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Rootmeta>(jsonString);
        }

    }
   
 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class MoralisAPIBaseClass2
    {
        public string token_address;
        public string token_id;  
        public string amount;
        public string owner_of;
        public string token_hash;
        public string block_number_minted;
        public string block_number;
        public string contract_type;
        public string name;
        public string symbol;
        public string token_uri;
        public string metadata;
        public DateTime synced_at;
        public DateTime? last_token_uri_sync;
        public DateTime? last_metadata_sync;
    }
    [Serializable]
    public class MoralisAPIBaseClass
    {
        public int total;
        public int page;
        public int page_size;
        public string cursor;
        public List<MoralisAPIBaseClass2> result;
        public string status;

        public static MoralisAPIBaseClass CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MoralisAPIBaseClass>(jsonString);
        }

    }
    */
}
