using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
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



        if (!ServerBaseURlHandler.instance.IsXanaLive)
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
        request = await GettingOwnedNFTS(localAPI);
        _OwnedNFTDataObj.NewRootInstance();
        if (request.downloadHandler.text.Contains("Invalid key"))
        {
            Debug.Log("<color = red> hey Invalid NFT list </color>");
        }
        else
        {
            _OwnedNFTDataObj.CreateJsonFromRoot(request.downloadHandler.text);
            callback?.Invoke();
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
                        XanaConstantsHolder.xanaConstants.IsDeemoNFT = true;
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
        AllDataFetchedfromServer?.Invoke("Web2");
    }



    public async Task<UnityWebRequest> GettingOwnedNFTS(string url)
    {
        requestData.address = publicID;
        string jsonData = JsonConvert.SerializeObject(requestData);
        // Convert the JSON data to a byte array
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest request = UnityWebRequest.Post(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();
        // Send the request
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
        string localAPI = "";
        if (ServerBaseURlHandler.instance.IsXanaLive)
        {
            //  localAPI = "https://prod-backend.xanalia.com/nfts/user-nft-status?userAddress=0x43Dc54e78EA2F1A038e84c0e003871a87D4D80C1&nftId=118868";
            if (TestSpecificCase)
            {
                localAPI = PrefixMainNetOneNFTOwnerShip + TestSpecificAdrress + PostfixOneNFTOwnerShip + _nftid;
            }
            else
            {
                localAPI = PrefixMainNetOneNFTOwnerShip + PlayerPrefs.GetString("publicID") + PostfixOneNFTOwnerShip + _nftid;
            }
        }
        else
        {
            if (TestSpecificCase)
            {
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
            }
        }
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
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();
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