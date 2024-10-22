using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using XanaNFT;

public class NFTFromServer : MonoBehaviour
{
    public enum ExhibitComponentType { Constant, Dynamic };
    public ExhibitComponentType _ExhibitComponentType;
    [Serializable]
    public class ExhibitSize
    {
        public Vector3 potrait;
        public Vector3 landscape;
        public Vector3 square;
    }
    public ExhibitSize _ExhibitSize;

    // Start is called before the first frame update
    public List<GameObject> spawnPoints;
    public Transform picsSpawnPoints;
    public bool updateVar;
    public List<MetaData> metaData;
    public List<CreatorDetails> creatorDetails;
    public List<TokenDetails> tokenDetails;
    public string nftLink;
    public string creatorLink;
    public static int allObjects;
    public int RoomCount;
    public bool isDynamicMuseum;
    public DynamicMuseumManager dynamicManager;
    public GameObject spotLightSprite;
    //string s3Path = "https://s3.ap-southeast-1.amazonaws.com/assets.xana.net/XanaDynamicMuseum";
    public int testnetMussuemId;
    public int mainnetMussuemId;
    private string dynamicMusuemApi = "/item/get-museum-all-assets/";
    private string dynamicEventFeedApi = "/userCustomEvent/get-events-all-assets-of-user/";
    public string eventid;
    string MussuemLink;
    public GameObject XanaSummitNFTHandler;
    //public List<Datum> TestList;
    void Start()
    {
        if (dynamicManager == null)
        {
            dynamicManager = FindObjectOfType<DynamicMuseumManager>();
        }   

        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            eventid = XanaEventDetails.eventDetails.id.ToString();
            MussuemLink = dynamicEventFeedApi + XanaEventDetails.eventDetails.museumId + "/" + XanaEventDetails.eventDetails.id;
            Debug.Log("Event Lunching");
            Debug.Log("MussuemLink"+ MussuemLink);
        }
        else 
        {
            MussuemLink = dynamicMusuemApi + ConstantsHolder.xanaConstants.MuseumID;
            Debug.Log("Openning Mussuem");
        }

       

        //picsSpawnPoints = transform.GetChild(0);
        int i = 0;
        while(picsSpawnPoints.childCount > i)
        {
            spawnPoints.Add( picsSpawnPoints.GetChild(i).gameObject);
            picsSpawnPoints.GetChild(i).gameObject.SetActive(false);
            i++;
        }
        RoomCount = 4;
        if (!ConstantsHolder.isFromXANASummit)
        {
            Invoke(nameof(GetNFTDataDetails), 1f);
            //MussuemLink = _singleDomeMusuemApi + ConstantsHolder.xanaConstants.DomeID;
        }
        else
        {
            //GameObject _summitNFTHandlerRef = Instantiate(XanaSummitNFTHandler, Vector3.zero, Quaternion.identity);
            // _summitNFTHandlerRef.GetComponentInChildren<SummitDomeNFTDataController>().NFTDataFetchScrptRef = this;
            XanaSummitNFTHandler.GetComponentInChildren<SummitDomeNFTDataController>().NFTDataFetchScrptRef = this;

            //if (!this.GetComponent<SummitDomeNFTDataController>())
            //{
            //    this.AddComponent<SummitDomeNFTDataController>();
            //    this.GetComponent<SummitDomeNFTDataController>().NFTDataFetchScrptRef = this;
            //}
        }

       // Invoke(nameof(UpdateFrames), 2f);
    }

    public void GetNFTDataDetails() {
        if (!isDynamicMuseum) // is met gallery
        {
            StartCoroutine(GetNftData());
        }
        else // is dynamic museum
        {
            StartCoroutine(GetNFTDatAForDynamicMuseum());
        }
    }

    XanaNftDetails nftDetails;
    S3NftDetail s3NftDetail;
    IEnumerator GetNftData()
    {
        while(Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }
        yield return null;
        using (UnityWebRequest request = UnityWebRequest.Get("https://api.xanalia.com/xana-museum/nfts"))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
           
            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    nftDetails = GetAllData(request.downloadHandler.text);
                    yield return new WaitForEndOfFrame();
                    if (nftDetails.success == true)
                    {
                        int i = 0;
                        foreach (Datum d in nftDetails.data)
                        {
                            if (d.tokenDetails.metaData.thumbnft != null) 
                            {
                                allObjects++;
                            }
                        }
                       
                        //if (isDynamicMuseum)
                        //{
                        //    nftDetails.data = UpdateRoomStatue(nftDetails.data);
                        //}

                        //foreach (Datum d in nftDetails.data)
                        //{
                        //    if (d.tokenDetails.creatorDetails.username != null)
                        //    {
                        //        print("my user name " + d.tokenDetails.creatorDetails.username);
                        //    }
                        //}
                        foreach (Datum d in nftDetails.data)
                        {
                            if(i>=spawnPoints.Count)
                            break;
                            Debug.Log("sUCESS");
                            metaData.Add(d.tokenDetails.metaData);
                            //print(d.tokenDetails.metaData.thumbnft);
                            //print(d.tokenDetails.metaData.thumbnft.EndsWith(".mp4"));
                            spawnPoints[i].SetActive(true);
                            spawnPoints[i].AddComponent<MetaDataInPrefab>().metaData = d.tokenDetails.metaData;
                            //spawnPoints[i].GetComponent<MetaDataInPrefab>().isDynamicMuseum = isDynamicMuseum;
                           spawnPoints[i].GetComponent<MetaDataInPrefab>().creatorDetails = d.creatorDetails;
                            spawnPoints[i].GetComponent<MetaDataInPrefab>().nftLink = d.nftLink;
                            spawnPoints[i].GetComponent<MetaDataInPrefab>().creatorLink = d.creatorLink;
                            spawnPoints[i].GetComponent<MetaDataInPrefab>().tokenDetails = d.tokenDetails;
                            //CreatorDetails cd = d.tokenDetails.creatorDetails;
                            // print("usman testing "+ d.tokenDetails.metaData.name);
                            spawnPoints[i].GetComponent<MetaDataInPrefab>().StartNow();
                            i++;
                          // print("my user name nothing" + d.creatorDetails.username);
                            // create prefabs of meta data recieved
                        }
                    }
                }
                else { Debug.Log("eRROR HERE"); }
            }
            else
            {
                if (request.isNetworkError)
                {
                    yield return StartCoroutine(GetNftData());
                    print("Network Error");
                }
                else
                {
                    if (request.error != null)
                    {
                        yield return StartCoroutine(GetNftData());
                        if (nftDetails.success == false)
                        {
                            print("Hey success false " + nftDetails);
                        }
                    }
                }
            }
            request.Dispose();
        }
    }
    public static void RemoveOne()
    {
        allObjects--;
        if (allObjects == 0)
        {
            print("Scene Is Ready To Display");
        }
    }

    public XanaNftDetails GetAllData(string m_JsonData)
    {
        XanaNftDetails JsonDataObj = new XanaNftDetails();
        JsonDataObj = JsonUtility.FromJson<XanaNftDetails>(m_JsonData);
        return JsonDataObj;
    }

    public S3NftDetail GetAllData_S3(string jsonData)
    {
        S3NftDetail S3NftDetailObj  = new S3NftDetail();
        S3NftDetailObj = JsonUtility.FromJson<S3NftDetail>(jsonData);
        return S3NftDetailObj;
    }

   

    [SerializeField]
    List<S3NftDetail> AllS3Nfts = new List<S3NftDetail>();
    IEnumerator GetNFTDatAForDynamicMuseum()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }
        // Making list for Data
        for (int i = 0; i < 65; i++)
        {
            AllS3Nfts.Add(new S3NftDetail());
        }

        //print("~~~~~~~~~ " + ConstantsGod.API_BASEURL + dynamicMusuemApi);
        //print("TOKEN : "+ ConstantsGod.AUTH_TOKEN);
       
        using (UnityWebRequest request = UnityWebRequest.Get(ConstantsGod.API_BASEURL + MussuemLink))
        {
           
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            //request.SetRequestHeader("Authorization", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MzIyNTAsImlhdCI6MTY4NzUxNDQxMywiZXhwIjoxNjg3Njg3MjEzfQ.FudsBYovB4KQYuR6EMmw0Mh8qAX0ZwaPHsoUyJmfuPk");
          
            yield return request.SendWebRequest();

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error== null)
                {
                    print("~!~!~! " + request.downloadHandler.text);
                    DynamicData apiData = new DynamicData();
                   
                    apiData = JsonUtility.FromJson<DynamicData>(request.downloadHandler.text);
                   

                    if (apiData.data.Count ==0)
                    {
                       Debug.Log(" NO DATA GET FROM API ");
                    }
                    else
                    {
                        for (int i = 0; i < apiData.data.Count; i++)
                        {
                            //print("!~!~! "+ getdata.data.rows[i);
                            if (!string.IsNullOrEmpty(apiData.data[i].id.ToString()))
                            {
                                S3NftDetail data = apiData.data[i];
                                if (apiData.data[i].index-1<AllS3Nfts.Count)
                                {
                                    int listIndex = data.index-1;
                                    AllS3Nfts[listIndex].id = data.id;
                                    AllS3Nfts[listIndex].asset_link = data.asset_link;
                                    AllS3Nfts[listIndex].index = data.index-1;
                                    AllS3Nfts[listIndex].check = data.check;
                                    AllS3Nfts[listIndex].authorName = data.authorName; //sss
                                    AllS3Nfts[listIndex].description = data.description; // sss
                                    AllS3Nfts[listIndex].title = data.title; // sss
                                    AllS3Nfts[listIndex].ratio = data.ratio;
                                    AllS3Nfts[listIndex].thumbnail = data.thumbnail;
                                    AllS3Nfts[listIndex].merdia_type = data.merdia_type;

                                }
                            }
                        }
                      
                    }// data count check if end
                } // error check if end
            }
            else
            {
               Debug.Log(request.error);
            }
            request.Dispose();
            yield return StartCoroutine(UdpdatData(AllS3Nfts));
        }
        #region oldCode
        //for (int index = 1; index <= 48; index++)  // getting 48 NFT's jsons
        //{
        //    print("for call");
        //    using (UnityWebRequest request = UnityWebRequest.Get(ConstantsGod.API_BASEURL+dynamicMusuemApi+"v2/1/100") )
        //    {
        //        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        //        yield return request.SendWebRequest();

        //        if (!request.isHttpError && !request.isNetworkError)
        //        {
        //            if (request.error == null)
        //            {
        //                print("request ~~~ "+ request.downloadHandler.text);
        //                s3NftDetail = GetAllData_S3(request.downloadHandler.text);
        //                yield return new WaitForEndOfFrame();
        //                AllS3Nfts.Add(s3NftDetail);
        //            }
        //        }
        //        else
        //        {
        //            if (request.isNetworkError && request.isHttpError)
        //            {
        //                status = false;
        //                yield return StartCoroutine(GetNFTDatAForDynamicMuseum());
        //                print("Network Error");
        //            }
        //            //else
        //            //{
        //            //    if (request.error != null)
        //            //    {
        //            //        yield return StartCoroutine(GetNftData());
        //            //        if (s3NftDetail.success == false)
        //            //        {
        //            //            print("Hey success false " + s3NftDetail);
        //            //        }
        //            //    }
        //            //}
        //        }
        //        request.Dispose();
        //    }
        //}

        //yield return StartCoroutine(UdpdatData(AllS3Nfts));
        #endregion
    }


    IEnumerator UdpdatData(List<S3NftDetail> allS3Nfts) 
    {
        allS3Nfts = UpdateRoomStatus(allS3Nfts);
        int i = 0;
        foreach (S3NftDetail d in allS3Nfts)
        {
            if (d == null)
                break;
            if (i >= spawnPoints.Count)
                break;
            spawnPoints[i].SetActive(true);
            spawnPoints[i].AddComponent<DynamicGalleryData>().detail = d;
            spawnPoints[i].GetComponent<DynamicGalleryData>().videoPlayer = ShowNFTDetails.instance.VideoObject.GetComponent<VideoPlayer>();
            spawnPoints[i].GetComponent<DynamicGalleryData>().videoPlayerWithStats = ShowNFTDetails.instance.VideoObjectWithDes.GetComponent<VideoPlayer>();
            DynamicGalleryData data = spawnPoints[i].GetComponent<DynamicGalleryData>();
            spawnPoints[i].AddComponent<Fram_Image>();

            data.NewFrame = spawnPoints[i].GetComponent<Fram_Image>();

            data.SpotLightObj = spotLightSprite;
            data.isDynamicMuseum = isDynamicMuseum;
            data.framePrefab = dynamicManager.frame;
           
            int tempIndex;
            string ratio;
            if (d.ratio != "" && d.ratio != null)
            {
                ratio = d.ratio;
                if (ratio == "1:1") // is square
                {
                    tempIndex = 0;
                }
                else if (ratio == "9:16") // is potraite
                {
                    tempIndex = 1;

                }
                else // is landsacpe
                {
                    tempIndex = 2;
                }
            }
            else
            {
                tempIndex = 0;
            }
            data.FrameLocalPos = dynamicManager.exhibatsSizes[tempIndex].FrameLocalPos;
            data.FrameLocalScale = dynamicManager.exhibatsSizes[tempIndex].FrameLocalScale;
            data.SpotLightPos = dynamicManager.exhibatsSizes[tempIndex].SpotLightPos;
            data.ColliderSize = dynamicManager.exhibatsSizes[tempIndex].ColliderSize;
            data.ColiderPos = dynamicManager.exhibatsSizes[tempIndex].ColiderPos;
            data.spotLightPrefabPos = dynamicManager.exhibatsSizes[tempIndex].SpotLightPrefabPos;
          
            //data.FrameLocalPos = dynamicManager.FrameLocalPos;
            //data.FrameLocalScale = dynamicManager.FrameLocalScale;
            //data.SpotLightPos = dynamicManager.SpotLightPos;
            //data.ColliderSize = dynamicManager.ColliderSize;
            data.spotLightPrefab = dynamicManager.spotLightPrefab;
            data.FrameMat = dynamicManager.FrameMaterial;
            data.potraiteSize = dynamicManager.potraiteSize;
            data.landscapeSize = dynamicManager.landscapeSize;
           
            // if user is joining through event then video square size should be this 
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                data.squareSize = dynamicManager.VideosquarSize;
                
            }
            else 
            {
                data.squareSize = dynamicManager.squarSize;
            }

            spawnPoints[i].GetComponent<DynamicGalleryData>().StartNow(); 
            i++;
        }
        yield return null;

    }


    //public void UpdateFrames()
    //{

    //    //spawnPoints[i].GetComponent<Fram_Image>().render = data.spriteObject.GetComponent<SpriteRenderer>();
    //    //spawnPoints[i].GetComponent<Fram_Image>().CreateCubeFrame();
    //}

    /// <summary>
    /// update room according to data fetch from server
    /// </summary>
    /// <param name="dataList"> sever response data </param>
    List<S3NftDetail> UpdateRoomStatus(List<S3NftDetail> allS3Nfts) {
        int index = 0;
        int endIndex = 0;
        int nftCount = 0;
        //int groupFirstIndex=0;
        for (int roomNo = 0; roomNo < RoomCount; roomNo++)
        {
            endIndex = endIndex + 16;
            for (int no = index; no <= endIndex; no++)
            {
                if (allS3Nfts[no].check==true/*allS3Nfts[no].Title != "" && allS3Nfts[no].NFTImageLink != ""*/)
                {
                    nftCount++;
                }
            }
            if (nftCount <= 0)
            {
                dynamicManager.rooms[roomNo].IsInUse = false;
            }
            else
            {
                dynamicManager.rooms[roomNo].IsInUse = true;
            }
            index = endIndex;
            nftCount = 0;
        }
        List<S3NftDetail>  temp = new List<S3NftDetail>(allS3Nfts);
        allS3Nfts= SwapData(temp);
        CloseAndOpenRoom();

        #region oldcode
        //Update rooms
        //if (!dynamicManager.rooms[3].IsInUse) // last room is empty
        //{
        //    dynamicManager.CloseRoom(3);
        //}
        //else  // last room is not empty
        //{
        //    if (!dynamicManager.rooms[2].IsInUse) // third room is empty
        //    {
        //        // copy data of room three to second room
        //        //CopyDataToSecondRoom(allS3Nfts, 32, 47);
        //        CopyDataToSecondRoom(allS3Nfts, 48, 63,32,47);
        //        dynamicManager.CloseRoom(3);
        //        dynamicManager.rooms[3].IsInUse = false;
        //        dynamicManager.rooms[2].IsInUse = true;

        //        //dynamicManager.OpenRoom(2);

        //    }
        //}

        //if (!dynamicManager.rooms[2].IsInUse) // last room is empty
        //{
        //    dynamicManager.CloseRoom(2);
        //}
        //else  // last room is not empty
        //{
        //    if (!dynamicManager.rooms[1].IsInUse) // second room is empty
        //    {
        //        // copy data of room three to second room
        //        CopyDataToSecondRoom(allS3Nfts, 32, 47,16,31);
        //       // dynamicManager.rooms[1].IsInUse = true;
        //        dynamicManager.CloseRoom(2);
        //        dynamicManager.rooms[2].IsInUse = false;
        //        dynamicManager.rooms[1].IsInUse = true;
        //    }
        //}



        //if (!dynamicManager.rooms[2].IsInUse && (!dynamicManager.rooms[1].IsInUse) )
        //{
        //    dynamicManager.CloseRoom(1);
        //}
        #endregion
        return allS3Nfts;
    }

    /// <summary>
    /// Close and open the room according to the room use status
    /// </summary>
    void CloseAndOpenRoom() {
        //foreach (var room in dynamicManager.rooms)
        //{
        //    if (room.IsInUse)
        //    {
        //        room.Obj.SetActive(true);
        //        if (room.Door)
        //        {
        //            room.Door.SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        room.Obj.SetActive(false);
        //        if (room.Door)
        //            room.Door.SetActive(true);
        //    }
        //}

        for (int i = 1; i < RoomCount; i++)
        {
            if (dynamicManager.rooms[i].IsInUse)
            {
                dynamicManager.rooms[i].Obj.SetActive(true);
                if (dynamicManager.rooms[i].Door)
                {
                    dynamicManager.rooms[i].Door.SetActive(false);
                }
            }
            else
            {
                dynamicManager.rooms[i].Obj.SetActive(false);
                if (dynamicManager.rooms[i].Door)
                    dynamicManager.rooms[i].Door.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Swap data in rooms 
    /// </summary>
    /// <param name="allS3Nfts"> nft list from the server</param>
    /// <returns></returns>
    List<S3NftDetail> SwapData(List<S3NftDetail> allS3Nfts) {
        int endIndex;
        int groupFirstIndex;
        endIndex = 64;
        groupFirstIndex = endIndex - 16;
        for (int r = RoomCount-1 ; r >= 1; r--)
        {
            if (dynamicManager.rooms[r].IsInUse) // check current room is in use ?
            {
                if (!dynamicManager.rooms[r - 1].IsInUse) // check previous room is in use?
                {
                    for (int i = endIndex; i > groupFirstIndex; i--)
                    {
                        allS3Nfts[i - 16] = allS3Nfts[i];
                        allS3Nfts[i] = null;
                    }
                    dynamicManager.rooms[r].IsInUse = false;
                    dynamicManager.rooms[r - 1].IsInUse = true;
                    return SwapData(allS3Nfts);
                }
            }
            endIndex -= 16;
            groupFirstIndex -= 16;
        }
        return allS3Nfts;
    }

    //List<S3NftDetail> swippedTemp;

    //List<S3NftDetail> CopyDataToSecondRoom(List<S3NftDetail> dataList, int start, int end, int newRoomMin, int newRoomMax) {
    //   swippedTemp = new List<S3NftDetail>();
    //    for (int i = start; i <= end; i++) // copy data in temp that need to copy
    //    {
    //        swippedTemp.Add(dataList[i]);
    //    }
    //    int copy = 0;
    //    for (int i = newRoomMin; i <= newRoomMax; i++) // copy data in second room
    //    {
    //        dataList[i] = swippedTemp[copy];
    //        copy++;
    //    }
    //    for (int i = start; i < end; i++)// removing from orignal list
    //    {
    //        dataList[i]= null;
    //    }
    //    return dataList;
    //}
   
}
//public enum NftExhibtRatio { 
//    square,
//    potraite,
//    landscape
//}