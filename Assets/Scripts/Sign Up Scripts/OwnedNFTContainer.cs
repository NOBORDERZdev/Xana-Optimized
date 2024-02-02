using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
//[CreateAssetMenu  (menuName = "OwnedNFTs/Container")]
[CreateAssetMenu(menuName = "OwnedNFT/NFTContrainer")]
public class OwnedNFTContainer : ScriptableObject
{
    public  Root NFTlistdata;
    public List<string> userNftRoleSlist;
    public List<string> NFTsURL;
    public List<string> NFTsName;
    public List<string> NFTsDescriptions;
    public List<int> NFTstype;
    public List<string> CollectionAddress;
    public List<Attribute1> _Attributes;
    public List<int> _NFTIDs;
    public void NewRootInstance()
    {
       // NFTlistdata = new Root();
        NFTlistdata.count = 0;
        NFTlistdata.list.Clear(); 
    } 
    public void CreateJsonFromRoot(string json)
    {
        NFTlistdata =  Root.CreateFromJSON(json);
        FillAllListAsyncWaiting();
    }
    public async Task FillAllListAsyncWaiting()
    {
        userNftRoleSlist.Clear();
        NFTsURL.Clear();
        NFTsName.Clear();
        NFTsDescriptions.Clear();
        NFTstype.Clear();
        CollectionAddress.Clear();
        _NFTIDs.Clear();
        _Attributes.Clear();

        if (NFTlistdata.count > 0)
        {
            for (int i = 0; i < NFTlistdata.list.Count; i++)
            {
                NFTsURL.Add(NFTlistdata.list[i].mediaUrl);
                NFTstype.Add(NFTlistdata.list[i].category);
                NFTsDescriptions.Add(NFTlistdata.list[i].description);
                NFTsName.Add(NFTlistdata.list[i].name);
                CollectionAddress.Add(NFTlistdata.list[i].collection.address);
                _Attributes.Add(NFTlistdata.list[i].attribute);
                _NFTIDs.Add(NFTlistdata.list[i].nftId);
            }
        }

        await Task.Yield(); // This line allows the method to be awaited without blocking
         Debug.Log("total " + (NFTlistdata.count - 1));
    }
 
    public  void FillAllListAsync()
    {
        userNftRoleSlist.Clear();
        NFTsURL.Clear();
        NFTsName.Clear();
        NFTsDescriptions.Clear();
        NFTstype.Clear();
        CollectionAddress.Clear();
        _NFTIDs.Clear();
        _Attributes.Clear(); 
        if(NFTlistdata.count > 0)
        {
            for (int i = 0; i < NFTlistdata.list.Count; i++)
            {
                NFTsURL.Add(NFTlistdata.list[i].mediaUrl);
                NFTstype.Add(NFTlistdata.list[i].category);
                NFTsDescriptions.Add(NFTlistdata.list[i].description);
                NFTsName.Add(NFTlistdata.list[i].name);
                CollectionAddress.Add(NFTlistdata.list[i].collection.address);
                _Attributes.Add(NFTlistdata.list[i].attribute);
                _NFTIDs.Add(NFTlistdata.list[i].nftId);
            }
        }
       
      
        Debug.Log("total " + (NFTlistdata.count - 1));
    }
    public void ClearAllLists()
    {
        // Main List
        NFTlistdata.count = 0;
        NFTlistdata.list.Clear();

        //Side Lists
        userNftRoleSlist.Clear();
        NFTsURL.Clear();
        NFTsName.Clear();
        NFTsDescriptions.Clear();
        NFTstype.Clear();
        CollectionAddress.Clear();
        _NFTIDs.Clear();
        _Attributes.Clear();
    }

}
 

 // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[Serializable]
public class Attribute1
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
    public int stamina;
    public int speed;
    public string profile;
    public int defence;
    public int special_move;
    public int punch;
    public int kick;
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
    public Attribute1 attribute;
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
 
