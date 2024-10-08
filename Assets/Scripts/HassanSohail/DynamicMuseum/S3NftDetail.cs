using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S3NftDetail
{
    public int id;
    public string museumId;
    public int index;
    public string asset_link;
    public bool check;
    public string[] authorName;
    public string[] description;
    public string[] title;
    public string ratio;
    public string thumbnail;
    public string merdia_type;
    public Time createdAt;
    public Time updatedAt;

    public S3NftDetail() { 
    }
    public S3NftDetail(int _id, string _museumId, int _index, string _asset_link, bool _check , string[] _title, string[] _authorName, string[] _description, string[] _creatorname) {
        id = _id;
        museumId = _museumId;
        index = _index;
        asset_link = _asset_link;
        check = _check;
        authorName = _creatorname;
        description = _description;
        title = _title;
        
        
    }

}


//[System.Serializable]
//public class DynamicApiData {

//    public bool success;
//    public DynamicData data;
//    public string msg;
//}

[System.Serializable]
public class DynamicData {
    //public int count;
    public bool success;
    public List<S3NftDetail> data;
}

[System.Serializable]
public class DomeNFTData
{
    public int id;
    public int dome_id;
    public int index;
    public string name;
    public string description;
    public string thumbnail;
    public int type;
    public string videoType;
    public string videoUrl;
    public bool isYoutubeUrl;
    public string proportionType;
    public DateTime createdAt;
    public DateTime updatedAt;
    public bool check;
}

[System.Serializable]
public class DomeNFTDataArray
{
    public int width;
    public List<DomeNFTData> getcontentbyDomeId;
}

