using AdvancedInputFieldSamples;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Web3Web2Handler : MonoBehaviour
{
    public static event Action<string> AllDataFetchedfromServer;
    //   public static event Action<string> AllWeb2NFTFetched;

    //   public int OwnedNFTPageNumb = 1;
    // public int OwnedNFTPageSize = 1000;
    public bool XanaliaBool;
    private string MainNetOwnednftAPI = "https://prod-backend.xanalia.com/nfts/nft-by-address-user?pageIndex={0}&pageSize={1}&address=";
    private string TestNetOwnednftAPI = "https://backend.xanalia.com/nfts/nft-by-address-user?pageIndex={0}&pageSize={1}&address=";
    private string Postfix = "&categoryFilter=2&sortFilter=2";

    //Test SpecifiedCase
    //https://prod-backend.xanalia.com/nfts/nft-by-address-user-tcg?address=0xec943d84e658f3972a35e62558702c2d7c74290d&pageIndex=1&pageSize=30&name=breaking down

    //Old API
    // https://prod-backend.xanalia.com/nfts/nft-by-address-user?pageIndex=1&pageSize=30&address=0x43Dc54e78EA2F1A038e84c0e003871a87D4D80C1&categoryFilter=2&userId=0

    //private string OwnedSpecifiednftAPIMainNet = "https://prod-backend.xanalia.com/nfts/nft-by-address-user-tcg?address=";
    private string OwnedSpecifiednftAPIMainNet = "https://prod-backend.xanalia.com/nfts/nft-by-address-user-tcg-v2?address=";
    private string OwnedSpecifiednftAPITestNet = "https://backend.xanalia.com/nfts/nft-by-address-user-tcg-v2?address=";

    //private string SpecifiedNFTPostFix = "&pageIndex=1&pageSize=1000&name=breaking down";
    private string SpecifiedNFTPostFix = "&pageIndex=1&pageSize=1000&name=";
    // https://backend.xanalia.com/nfts/nft-by-address-user-tcg?address=0x138e3dd54e5c3cb174f88232ad3fbba81331db4b&pageIndex=1&pageSize=1000&name=breaking down
    //private string OwnedSpecifiednftAPIMainNet = "https://prod-backend.xanalia.com/nfts/nft-by-address-user?pageIndex=1&pageSize=300&address=";
    // private string OwnedSpecifiednftAPITestNet = "https://backend.xanalia.com/nfts/nft-by-address-user?pageIndex=1&pageSize=300&address=";  
    // private string SpecifiedNFTPostFix = "&categoryFilter=2&userId=0";
    public OwnedNFTContainer _OwnedNFTDataObj;
    public string publicID;
    //public UserNFTlistClass.Root NFTlistdata;
    public bool TestSpecificCase;
    public string TestSpecificAdrress;

    [SerializeField]
    private RequestData requestData;

    [SerializeField] bool useXanaliaMainnetApiOnTestnet;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("web3apiforweb2: " + gameObject.name);

    }



    public void GetWeb2UserData(string _publicID, Action callback = null)
    {
        print("Functionweb2");
        if (TestSpecificCase)
        {
            publicID = TestSpecificAdrress;
        }
        else
        {
            publicID = _publicID;
        }
        CallOwnedNFTListAPIAsync(callback);
    }

    public async Task CallOwnedNFTListAPIAsync(Action callback)
    {

        string localAPI = "";



        if (!APIBasepointManager.instance.IsXanaLive)
        {
            //  localAPI = string.Format(TestNetOwnednftAPI, OwnedNFTPageNumb, OwnedNFTPageSize) + publicID + Postfix;
            if (TestSpecificCase)
            {
                // XANALLIA APIS ARE NOT IMPLEMENT ON TESTNET THATS WHY USING MAINNET APIS.
                if (useXanaliaMainnetApiOnTestnet)
                {
                     localAPI = OwnedSpecifiednftAPIMainNet + TestSpecificAdrress + SpecifiedNFTPostFix;
                }
                else
                {
                    localAPI = OwnedSpecifiednftAPITestNet + TestSpecificAdrress + SpecifiedNFTPostFix;
                }

            }
            else
            {
                // XANALLIA APIS ARE NOT IMPLEMENT ON TESTNET THATS WHY USING MAINNET APIS.
                if (useXanaliaMainnetApiOnTestnet)
                {
                     localAPI = OwnedSpecifiednftAPIMainNet + publicID + SpecifiedNFTPostFix;
                }
                else
                {
                  localAPI = OwnedSpecifiednftAPITestNet + publicID + SpecifiedNFTPostFix;
                }
            }
        }
        else
        {
            if (TestSpecificCase)
            {
                localAPI = OwnedSpecifiednftAPIMainNet + TestSpecificAdrress + SpecifiedNFTPostFix;

            }
            else
            {
                localAPI = OwnedSpecifiednftAPIMainNet + publicID + SpecifiedNFTPostFix;

            }
        }
        UnityWebRequest request;
        Debug.Log("localAPI for Getting Owned NFT: " + localAPI);
        request = await GettingOwnedNFTS(localAPI);
        //    NFTlistdata = new UserNFTlistClass.Root();
        _OwnedNFTDataObj.NewRootInstance();
        if (request.downloadHandler.text.Contains("Invalid key"))
        {
            Debug.Log("<color = red> hey Invalid NFT list </color>");
        }
        else
        {
            //   NFTlistdata = UserNFTlistClass.Root.CreateFromJSON(request.downloadHandler.text);
            Debug.Log("NFT DATA from API: " + request.downloadHandler.text);
            _OwnedNFTDataObj.CreateJsonFromRoot(request.downloadHandler.text);

            /*for (int i = 0; i < NFTlistdata.list.Count; i++)
            {
                if (!allNFTlistdata.list.Contains(NFTlistdata.list[i]))
                {
                    allNFTlistdata.list.Add(NFTlistdata.list[i]);
                }
            }*/
            callback?.Invoke();
            Debug.Log("Total Number of NFTs are : " + _OwnedNFTDataObj.NFTlistdata.count);
            if (_OwnedNFTDataObj.NFTlistdata.count > 0)
            {
                for (int i = 0; i < _OwnedNFTDataObj.NFTlistdata.list.Count; i++)
                {
                    string NFTname = _OwnedNFTDataObj.NFTlistdata.list[i].name.ToLower();
                    if (NFTname.Contains("XANA x BreakingDown"))
                    {
                        Debug.LogError("BreakingDown");
                    }
                    if (NFTname.Contains("deemo"))
                    {
                        ConstantsHolder.xanaConstants.IsDeemoNFT = true;
                    }
                    if (NFTname.Contains("astroboy"))
                    {
                        Debug.LogError("Astroboy");
                        //  CryptouserData.instance.AstroboyPass = true;
                    }
                    if (NFTname.Contains("ultraman"))
                    {
                        Debug.LogError("Contained Ultraman");
                        //  CryptouserData.instance.UltramanPass = true;
                    }
                }
            }
        }
        await Task.Delay(500);
        Debug.Log("AllDataFetchedfromServer web2");
        AllDataFetchedfromServer?.Invoke("Web2");
    }



    public async Task<UnityWebRequest> GettingOwnedNFTS(string url)
    {


        requestData.address = publicID;

        // Serialize the data object into a JSON string
        string jsonData = JsonConvert.SerializeObject(requestData);
        // Convert the JSON data to a byte array
        Debug.Log("jsonData: " + jsonData);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Set up the UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Post(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set the content type header
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();
        // Send the request
        print("return " + request.downloadHandler.text);
        return request;



    }


    //private string MainNetOneNFTOwnerShip = "https://prod-backend.xanalia.com/nfts/user-nft-status?userAddress=&nftId=";
    //private string TesnetOneNFTOwnerShip = "https://backend.xanalia.com/nfts/user-nft-status?userAddress=&nftId=";

    private string PrefixMainNetOneNFTOwnerShip = "https://prod-backend.xanalia.com/nfts/user-nft-status?userAddress=";
    private string PrefixTesnetOneNFTOwnerShip = "https://backend.xanalia.com/nfts/user-nft-status?userAddress=";

    private string PostfixOneNFTOwnerShip = "&nftId=";

    // https://backend.xanalia.com/nfts/user-nft-status?userAddress=0x138e3dd54e5c3cb174f88232ad3fbba81331db4b&nftId=20083

    public async Task<bool> CheckSpecificNFTAndReturnAsync(string _nftid)
    {
        print("NFT22 " + _nftid);
        string localAPI = "";
        if (APIBasepointManager.instance.IsXanaLive)
        {
            //  localAPI = "https://prod-backend.xanalia.com/nfts/user-nft-status?userAddress=0x43Dc54e78EA2F1A038e84c0e003871a87D4D80C1&nftId=118868";
            if (TestSpecificCase)
            {
                print("NFT33 " + _nftid);

                localAPI = PrefixMainNetOneNFTOwnerShip + TestSpecificAdrress + PostfixOneNFTOwnerShip + _nftid;
            }
            else
            {
                print("NFT44 " + _nftid);

                localAPI = PrefixMainNetOneNFTOwnerShip + PlayerPrefs.GetString("publicID") + PostfixOneNFTOwnerShip + _nftid;
            }
        }
        else
        {
            // localAPI = "https://backend.xanalia.com/nfts/user-nft-status?userAddress=0x43Dc54e78EA2F1A038e84c0e003871a87D4D80C1&nftId=118868";

            if (TestSpecificCase)
            {
                print("NFT55 " + _nftid);
                if (useXanaliaMainnetApiOnTestnet)
                {
                   localAPI = PrefixMainNetOneNFTOwnerShip + TestSpecificAdrress + PostfixOneNFTOwnerShip + _nftid;
                }
                else
                {
                    localAPI = PrefixTesnetOneNFTOwnerShip + TestSpecificAdrress + PostfixOneNFTOwnerShip + _nftid;
                }
            }
            else
            {
                if (useXanaliaMainnetApiOnTestnet)
                {
                   localAPI = PrefixMainNetOneNFTOwnerShip + PlayerPrefs.GetString("publicID") + PostfixOneNFTOwnerShip + _nftid;
                }
                else
                {
                    localAPI = PrefixTesnetOneNFTOwnerShip + PlayerPrefs.GetString("publicID") + PostfixOneNFTOwnerShip + _nftid;
                }
                print("NFT66 " + _nftid);
            }
        }
        Debug.Log("localAPI: " + localAPI);
        UnityWebRequest request;
        request = await GettingSpecificOwnedNFTS(localAPI);
        TestSpecificNFT myObject1 = new TestSpecificNFT();
        myObject1 = JsonUtility.FromJson<TestSpecificNFT>(request.downloadHandler.text);
        return myObject1.isOwned;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class TestSpecificNFT
    {
        public bool isOwned;
    }
    public async Task<UnityWebRequest> GettingSpecificOwnedNFTS(string url)
    {
        print(url);
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();
        print("return " + request.downloadHandler.text);
        return request;
    }

}
[Serializable]
public class RequestData
{
    public int pageIndex;
    public int pageSize;
    public string address;
    public string[] name;
}