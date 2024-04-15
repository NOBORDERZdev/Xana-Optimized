using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public class WalletConnectDataClasses : MonoBehaviour
{
  /// *************************************************   CONNECTION REQUEST  STARTED   *************************************************************************///
  
 
    [System.Serializable]
    public class first11
    {  
        public string type = "connect";
        public first22 data;
        public first11 getData(string myAppID)
        {
             first11 Obj1 = new first11();
            first22 Obj2 = new first22();
            Obj1.data = Obj2.getData(myAppID);
            return Obj1;
        }
    }
    [System.Serializable]
    public class first22
    {
        public string type = "app";
        public data2 data;

        public first22 getData(string myAppID)
        {
            first22 Obj1 = new first22();
            data2 Obj2 = new data2();
            Obj1.type = type;
            Obj1.data = Obj2.GetConnectValue(myAppID);
            return Obj1;
        }
    }
    [System.Serializable]
  public  class data2
    {
        public string appId = "11";
        public string name = "XANA";
        public string description = "hello i come from xana";
        public string icon = "https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/Xana+new+Logo/Union_9.png";
        public string url = "https://https://angeluim-metaverse.s3.ap-southeast-1.amazonaws.com/unitydata/Xana+new+Logo/Xana.png";
        public data2 GetConnectValue(string myAppID)
        {
            data2 Obj = new data2();
            Obj.appId = myAppID;
            Obj.name = name;
            Obj.description = description;
            Obj.icon = icon;
            Obj.url = url;
            return Obj;
        }
    }
    /////////////////////////////////////////////////////  CONNECTION REQUEST  ENDS  ///////////////////////////////////////////////////////////////////////////


    /// *************************************************   DISCONNECTION REQUEST STARTS HERE   *************************************************************************///

 
    [System.Serializable]
    public class Disconnect1
    {
        public string type = "disconnect";
        public Disconnect2 data;
        public Disconnect1 _Disconnect(string _AppID)
        {
            Disconnect1 myObj = new Disconnect1();
            Disconnect2 obj2 = new Disconnect2();
            myObj.type = type;
            myObj.data = obj2._Disconnect2(_AppID);
            return myObj;
        }
    }

    [System.Serializable]
    public class Disconnect2
    {
        public string type = "app_disconnect";
        public Disconnect3 data;
        public Disconnect2 _Disconnect2(string _AppID)
        {
            Disconnect2 myObj = new Disconnect2();
            Disconnect3 myObj2 = new Disconnect3();
            myObj.type = type;
            myObj.data = myObj2._Disconnect3(_AppID);
            return myObj;
        }
    }
    [System.Serializable]
  public  class Disconnect3
    {
        public string appId = "";
        public Disconnect3 _Disconnect3(string _AppID)
        {
            Disconnect3 myObj = new Disconnect3();
            myObj.appId = _AppID.ToString();
            return myObj;
        }
    }

    /////////////////////////////////////////////////////  DISCONNECTION REQUEST  ENDS HERE ///////////////////////////////////////////////////////////////////////////



    /// *************************************************   GENERATE MSG REQUEST STARTS HERE   *************************************************************************///
 
    [System.Serializable]
   public class GenerateMsgClass
    {
        public string type = "ask";
        public msgClassData data = null;  
        public GenerateMsgClass msgClassFtn(String msg, string myAppID)
        {
            GenerateMsgClass obj1 = new GenerateMsgClass();
            msgClassData Obj2 = new msgClassData();
            obj1.type = "ask";
            obj1.data = Obj2.getData2(msg, myAppID);
            return obj1;
        }
    }
    [System.Serializable]
   public class msgClassData
    {
        public string msg = "msg here";
        public string appId = "app id here";
        public msgClassData getData2(String msg1, string myAppID)
        {
            msgClassData Obj1 = new msgClassData();
            Obj1.msg = msg1;
            Obj1.appId = myAppID;
            return Obj1;
        }
    }
    /////////////////////////////////////////////////////  GENERATE MSG REQUEST ENDS HERE  ///////////////////////////////////////////////////////////////////////////

 
[Serializable]
    public class GeneralClassFields
    {
        public string status;
        public string type;
        public string data;
    }
 

    [Serializable]
   public class CommonObjectFields
    {
        public string type;
        public string data;
    }
      
    [System.Serializable]
   public class SuccessClass
    {
        public string type;
        public string data;
    }

     [System.Serializable]
    public class ConnectedClass
    {
        public string type;
        public ReadResponse2 data;
    }
    [System.Serializable]
   public class ReadResponse
    {
        public string type;
        public ReadResponse2 data;
        public ReadResponse returnResponse(string getText)
        {
            ReadResponse obj = new ReadResponse();
            obj = JsonUtility.FromJson<ReadResponse>(getText);
            return obj;
        }
    }
 
    [System.Serializable]
    public class ReadResponse2
    {
        public string msg;
        public string walletId;
        public string address;
        public ReadResponse returnResponse(string getText)
        {
            ReadResponse obj = new ReadResponse();
            obj = JsonUtility.FromJson<ReadResponse>(getText);
            return obj;
        }
    }
    ///////////////////////////////////////////////////////////   END MESSAGE  RESPONSE HERE   ////////////////////////////////////////////////////////////////





    /// *************************************************   VerifySignature STARTS HERE   *************************************************************************///

  //  {"status": "success", "type":"verifysig", "data":{"sig":"0x95d46404b73019f164754090f3f76d6f3b420a4ef8a694311662524dd1eb71a6182f3573d041fc4370dd80f0a0dfccfb8ce347faf3319575fc5510509b4632b81b",
  //  "pubKey":"0xfd8da888a6063219e30f53fd0a32014a54a807ba", "nonce":"6Cxvr9s4bKV1w3fD"}}



[System.Serializable]
    public class VerifySignatureClass
    {
         public string status;
        public string type;
        public dataSignatureClass data;
    }
    [System.Serializable]
    public class dataSignatureClass
    {
        public string sig;
        public string pubKey;
        public string nonce;  
        public string sigXanalia;  
        public string nonceXanalia;  
     }  
    ///////////////////////////////////////////////////////////   EverifySignature ENDS HERE   ////////////////////////////////////////////////////////////////

    /// *************************************************   ERROR STARTS HERE   *************************************************************************///

    [System.Serializable]
    public class ErrorClass
    {
        public string type;
        public string data;
    }
    ///////////////////////////////////////////////////////////   ERROR ENDS HERE   ////////////////////////////////////////////////////////////////

    /// *************************************************   GENERATE NOUNCE STARTS HERE   *************************************************************************///

    [System.Serializable]
    public class NounceClass
    {
        public string walletAddress;

        public NounceClass NounceClassFtn( string _pubAddress)
        {
            NounceClass obj1 = new NounceClass();
             obj1.walletAddress = _pubAddress;
             return obj1;
        }
    }
    //  { "success":true,"data":{ "nonce":"yYF2PEzXQ42Ac5FA"},"msg":"Nonce get successfully"}

 [Serializable]
    public class NounceMsg1
    {
         public bool success;
         public NounceMsg2 data;
        public string msg;
     }
   
     [Serializable]
    public class NounceMsg2
    {
          public string nonce;
     }



    ///////////////////////////////////////////////////////////    GENERATE NOUNCE ENDS HERE   ////////////////////////////////////////////////////////////////

 

    /// *************************************************   Verify signed msg STARTS HERE   *************************************************************************///
    [System.Serializable]
    public class VerifySignedMsgClass
    {
        public string nonce;
        public string signature;
        public VerifySignedMsgClass VerifySignedClassFtn(string _nonce , string _Signature)
        {
            VerifySignedMsgClass obj1 = new VerifySignedMsgClass();
            obj1.nonce = _nonce;
            obj1.signature = _Signature;
            return obj1;
        }
     }  

    [Serializable]
    public class VerifyReadSignedMsgFromServer
    {
        public bool success;
        public VerifyReadSignedMsgFromServer2 data;
        public string msg;
    }
       
    //{ "success":true,
    //    "data":       
    //   { "token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MTEwOSwiaWF0IjoxNjM4ODcyNTM5LCJleHAiOjE2MzkwNDUzMzl9.4BH7YmDXAj8BD_J7zIOz13mDgEMID-Ev9yKCw3sE2gk",
    //     "encryptedId":"U2FsdGVkX19OilIWtOIOsFmJsDTmeQ611iDOoheZ064=",
    //  "user":{ "id":1109,"name":"","email":"","phoneNumber":"","coins":"0.00"} },
    //  "msg":"User connected successfully"}
 
    [Serializable]
    public class VerifyReadSignedMsgFromServer2
    {
        public string token;
        public string encryptedId;
        public UserDataDetail user;
    }  
    public class UserDataDetail
    {
        public int id;
        public string name;
        public string email;
        public string phoneNumber;
        public string coins;
      }
    public ClassWithToken TokenClassHere(string getTxt)
    {
        ClassWithToken obj = new ClassWithToken();
         return obj;
    }


    [System.Serializable]
    public class ClassWithToken
    {
        public bool success;
        public JustToken data;
        public string msg;
        public static ClassWithToken CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ClassWithToken>(jsonString);
        }
    }

    [System.Serializable]
    public class JustToken
    {
        public string token;
        public string encryptedId;
        public UserData user;

        public static JustToken CreateFromJSON(string jsonString)
        {
            print("Person " + jsonString);
            return JsonUtility.FromJson<JustToken>(jsonString);
        }
    }

    [System.Serializable]
    public class UserData
    {
        public int id;
        public string name;
        public string email;
        public string phoneNumber;
        public string coins;
    }

    ///////////////////////////////////////////////////////////   Verify signed msg ENDS HERE   ////////////////////////////////////////////////////////////////
  
    
    
    ///////////////////////////////////////////////////////////  Start App Connect Class   ////////////////////////////////////////////////////////////////

     [System.Serializable]
    public class AppConnectClass
    {
        public string status;
        public string type;
        public AppConnectDataClass data;
    }
    [System.Serializable]
    public class AppConnectDataClass
    {
        public string msg;
    }
 
    ///////////////////////////////////////////////////////////  END App Connect Class   ////////////////////////////////////////////////////////////////
   
    
    
    
    /////////////////////////////////////////////////////////// Start Connection Approved Class   ////////////////////////////////////////////////////////////////
   
    [System.Serializable]
    public class ConnectionApprovedClass
    {
        public string status;
        public string type;
        public ConnectionApprovedDataClass data;
    }
    //   "data":{ "msg":"app approved to connect", "walletId":"0x2c7536E3605D9C16a7a3D7b1898e529396a65c23", "address":"0x2c7536E3605D9C16a7a3D7b1898e529396a65c23"} }
     [System.Serializable]
    public class ConnectionApprovedDataClass
    {
        public string msg;
        public string walletId;
        public string address;
    }
    /////////////////////////////////////////////////////////// END Connection Approved Class   ////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////// Start Disconnect Class   ////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class DisconnectDataClass
    {
        public string msg;
        public string walletId;
        public string address;
    }  
    /////////////////////////////////////////////////////////// END Disconnect Class   ////////////////////////////////////////////////////////////////



    ///////////////////////////////////////////////////////////  Start App Rejected Class   ////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class AppRejectedClass
    {
        public string status;
        public string type;
        public AppRejectedDataClass data;
        public AppRejectedClass returnResponse(string getText)
        {
            AppRejectedClass obj = new AppRejectedClass();
            obj = JsonUtility.FromJson<AppRejectedClass>(getText);
            return obj;
        }


    }
    [System.Serializable]
    public class AppRejectedDataClass
    {
        public string msg;  
        public string walletId;
    }

    ///////////////////////////////////////////////////////////  END App Rejected Class   ////////////////////////////////////////////////////////////////

    /// *************************************************   GENERATE NOUNCE STARTS HERE   *************************************************************************///

    [System.Serializable]
    public class NounceClassForXanalia
    {
        public string publicAddress;

        public NounceClassForXanalia NounceClassFtnForXanalia(string _pubAddress)
        {
            NounceClassForXanalia obj1 = new NounceClassForXanalia();
            obj1.publicAddress = _pubAddress;
            return obj1;
        }
    }

    /// *************************************************   Read Xanalia NOUNCE  *************************************************************************///

    [Serializable]
    public class NounceMsgXanalia
    {
        public bool success;
        public string data;
     }

    /// *************************************************   Read JWT xanalia After Verification  *************************************************************************///


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
 
 
    [Serializable]
    public class Links
    {
        public string facebook;
        public string website;
        public string discord;
        public string twitter;
        public string instagram;
        public string youtube;
        public VerifyReadSignedMsgFromServerXanalia returnResponse(string getText)
        {
            VerifyReadSignedMsgFromServerXanalia obj = new VerifyReadSignedMsgFromServerXanalia();
            obj = JsonUtility.FromJson<VerifyReadSignedMsgFromServerXanalia>(getText);
            return obj;
        }
    }

    [Serializable]
    public class UserDataDetailFromXanalia
    {
        public string _id;
        public string username;
        public string role;
        public int following;
        public int followers;
        public long createdAt;
        public string profile_image;
        public string about;
        public Links links;
        public string en_about;
        public string ja_about;
        public string ko_about;
        public string zh_about;
        public int transalte_again;
        public string title;
        public string userNftRole;  
        public VerifyReadSignedMsgFromServerXanalia returnResponse(string getText)
        {
            VerifyReadSignedMsgFromServerXanalia obj = new VerifyReadSignedMsgFromServerXanalia();
            obj = JsonUtility.FromJson<VerifyReadSignedMsgFromServerXanalia>(getText);
            return obj;
        }
    }

    [Serializable]
    public class VerifyReadSignedMsgFromServer2FromXanalia
     {
        public UserDataDetailFromXanalia user;
        public string token;
        public string message;
        public VerifyReadSignedMsgFromServerXanalia returnResponse(string getText)
        {
            VerifyReadSignedMsgFromServerXanalia obj = new VerifyReadSignedMsgFromServerXanalia();
              obj = JsonUtility.FromJson<VerifyReadSignedMsgFromServerXanalia>(getText);
            return obj;
        }  
    }

    [Serializable]
    public class VerifyReadSignedMsgFromServerXanalia
    {
        public bool success;
        public VerifyReadSignedMsgFromServer2FromXanalia data;  
        public VerifyReadSignedMsgFromServerXanalia returnResponse(string getText)
        {
            VerifyReadSignedMsgFromServerXanalia obj = new VerifyReadSignedMsgFromServerXanalia();
            obj = JsonUtility.FromJson<VerifyReadSignedMsgFromServerXanalia>(getText);
            return obj;
        }
    }
  


    /*
    [Serializable]
    public class VerifyReadSignedMsgFromServerXanalia
    {
        public bool success;
        public VerifyReadSignedMsgFromServer2FromXanalia data;
     }  

 
[Serializable]
    public class VerifyReadSignedMsgFromServer2FromXanalia
    {
         public UserDataDetailFromXanalia user;
         public string token;
         public string message;
        public VerifyReadSignedMsgFromServer2FromXanalia returnResponse(string getText)
        {
            VerifyReadSignedMsgFromServer2FromXanalia obj = new VerifyReadSignedMsgFromServer2FromXanalia();
            obj = JsonUtility.FromJson<VerifyReadSignedMsgFromServer2FromXanalia>(getText);
            return obj;
        }     
    }  
   

    [Serializable]
    public class UserDataDetailFromXanalia
    {
        public string _id;
        public string username;
        public string role;
        public int following;
        public int followers;
        public DateTime createdAt;

        public VerifyReadSignedMsgFromServer2FromXanalia returnResponse(string getText)
        {
            VerifyReadSignedMsgFromServer2FromXanalia obj = new VerifyReadSignedMsgFromServer2FromXanalia();
            obj = JsonUtility.FromJson<VerifyReadSignedMsgFromServer2FromXanalia>(getText);
            return obj;
        }
    }
        */
    /*
    {limit: 25,
    loggedIn: "0x7ebe14ab1e82f9d230d8235c5ca7d3b77d92b07d",
    networkType: "testnet",
    nftType: "mycollection",
    owner: "0x7ebe14ab1e82f9d230d8235c5ca7d3b77d92b07d",
    page: 1}
    */
    /// *************************************************   NFT list Converting to JSON *************************************************************************///

    [Serializable]
    public class NFTList
    {
        public int limit;
        public string loggedIn;
        public string networkType;
        public string nftType;
        public string owner;
        public int page;
        public NFTList AssignNFTList(int _limit, string _loggedIn, string _networkType, string _nftType, string _owner, int _page)
        {
            NFTList obj1 = new NFTList();
            obj1.limit = _limit;
            obj1.loggedIn = _loggedIn;
            obj1.networkType = _networkType;
            obj1.nftType = _nftType;
            obj1.owner = _owner;
            obj1.page = _page;
            return obj1;
        }
    }

    /// *************************************************   NFT list Reader  *************************************************************************///

    //   { networkType: "mainnet", nftType: "mycollection", status: "my_collection", page: 1, limit: 40,…}
    /*

        limit: 40
        loggedIn: "60f0287d436b32d50cece467"
        networkType: "mainnet"
        nftType: "mycollection"
        page: 1
        status: "my_collection"
     */




    [Serializable]
    public class NFTListMainNet
    {
        public int limit;
        //public string loggedIn;
        public string networkType;
        public string nftType;
        public int page;
        //public string status;
        //public string sort;
        public NFTListMainNet AssignNFTList(int _limit, string _networkType, string _nftType, int _page)
        {
            NFTListMainNet obj1 = new NFTListMainNet();
            obj1.limit = _limit;
            //obj1.loggedIn = _loggedIn;
            obj1.networkType = _networkType;
            obj1.nftType = _nftType;
             obj1.page = _page;
            //obj1.status = _status;
            //obj1.sort = _sort;
            return obj1;
        }
    }

    /// *************************************************   NFT list Reader  *************************************************************************///

    [Serializable]
    public class NonCryptoNFTRole
    {
        public string _id;
        public string username;
        public string email;
        public string role;
        public int following;
        public int followers;
        public long createdAt;
        public string referralCode;
        public string signUpRef;
        public string[] userNftRole;
        public string[] userNftRoleArr;
        public long createdFrom;
        public object emailVerificationToken;
        public bool emailVerified;
        //public bool dataShared;
        //public string userNftRole;
        public static NonCryptoNFTRole CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<NonCryptoNFTRole>(jsonString);
        }  
    }  
    [Serializable]
    public class RootNonCryptoNFTRole
    {
        public bool success;
        public NonCryptoNFTRole data;
        public static RootNonCryptoNFTRole CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<RootNonCryptoNFTRole>(jsonString);
        }
    }  

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class _1
    {
        public string _hex;
    }     
     [Serializable]
    public class TokenId
    {
        public string _hex;
    }  
    [Serializable]
    public class ReturnValues
    {
        public string _0;    
        public _1 _1;      
        public string _2;
        public string _3;  
        public string collection;
        public TokenId tokenId;  
        public string minter;
        public string tokenURI;
        public string to;
    }    
    [Serializable]
    public class Raw
    {
        public string data;
        public List<string> topics;
    }
    [Serializable]
    public class Properties
    {
        public string type;
    }
    [Serializable]
    public class MetaData
    {
        public string name;
        public string description;
        public string image;
        public Properties properties;
        public int totalSupply;
        public string externalLink;  
        public string thumbnft;
    }
    [Serializable]
    public class SellNFT
    {
        public string _id;
        public string transactionHash;
        public string seller;  
        public string tokenId;
        public string price;
        public string priceConversion;
        public bool buy;
        public object sellDateTime;
        public string baseCurrency;
        public string allowedCurrencies;
        public string mainChain;
        public string collection;
        public string owner;
        public string buyCalculated;
        public string buyCurrencyType;
        public object buyDateTime;
        public string buyTxHash;
        public string buybaseCurrency;
    }  
    [Serializable]
    public class Datum
    {
        public string _id;
        public string address;
        public int blockNumber;
        public string transactionHash;
        public int transactionIndex;
        public string blockHash;
        public int logIndex;
        public bool removed;
        public string id;
        public ReturnValues returnValues;
        public string @event;
        public string signature;
        public Raw raw;
        public MetaData metaData;
        public string tokenId;
        public int timestamp;
        public string newtokenId;
        public string price;
        public object sortPrice;
        public string sortPrice2;
        public int rating;
        public int approval;
        public int like;
        public string thumbnailUrl;
        public List<object> likeObj;
        public int like_count;
        public SellNFT sellNFT;
    }
    [Serializable]
    public class Root
    {
        public bool success;
        public List<Datum> data;
        public int count;
    }        
}  

   