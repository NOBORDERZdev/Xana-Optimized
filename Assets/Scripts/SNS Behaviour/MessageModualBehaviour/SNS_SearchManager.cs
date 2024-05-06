using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdvancedInputFieldPlugin;

public class SNS_SearchManager : MonoBehaviour
{
    // public List<string> allData;
    public List<string> searchHistory;
    public TMP_InputField _input;
    public AdvancedInputField _inputTMP;
    public Transform listContainer;
    public Transform searchContainer;
    public GameObject searchScrollView;
    public GameObject mainScrollView;

    public bool isSearchForSelectFriend, isSearschForConversationList;

    private void OnDisable()
    {
        if (string.IsNullOrEmpty(_inputTMP.RichText))
        {
            mainScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        }
        _inputTMP.Text = "";
        SetupAvtiveScollView(false);
    }

    public void SetupAvtiveScollView(bool isActive)
    {
        searchScrollView.SetActive(isActive);

        isActive = !isActive;
        mainScrollView.SetActive(isActive);
    }      

    //public void OnTextValueChange()
    //{
    //    //string userInput = _input.text;
    //    string userInput = _inputTMP.Text;
    //    if (string.IsNullOrEmpty(userInput))
    //    {
    //        for (int i = 0; i < listContainer.childCount; i++)
    //        {
    //            listContainer.transform.GetChild(i).gameObject.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        searchHistory = new List<string>();
    //        List<string> tmpList = new List<string>();
    //        List<string> tmpList2 = new List<string>();
    //        if (isSearchForSelectFriend)
    //        {
    //            for (int i = 0; i < SNS_APIController.Instance.allFollowingUserList.Count; i++)
    //            {
    //                if (!string.IsNullOrEmpty(SNS_APIController.Instance.allFollowingUserList[i]))
    //                {
    //                    string nameStr = SNS_APIController.Instance.allFollowingUserList[i].ToLower();
    //                    if (nameStr.Contains(userInput.ToLower()))
    //                    {
    //                        tmpList.Add(SNS_APIController.Instance.allFollowingUserList[i]);
    //                    }
    //                }
    //            }

    //            for (int i = 0; i < tmpList.Count; i++)
    //            {
    //                if (tmpList[i].StartsWith(userInput.ToLower()))
    //                {
    //                    searchHistory.Add(tmpList[i]);
    //                }
    //                else
    //                {
    //                    tmpList2.Add(tmpList[i]);
    //                }
    //            }

    //            for (int i = 0; i < tmpList2.Count; i++)
    //            {
    //                searchHistory.Add(tmpList2[i]);
    //            }

    //            for (int i = 0; i < listContainer.childCount; i++)
    //            {
    //                //if (i < searchHistory.Count)
    //                bool isFind = false;
    //                //for (int j = 0; j < searchHistory.Count; j++)
    //                //{
    //                //    if (listContainer.transform.GetChild(i).GetComponent<MessageUserDataScript>().textUserName.text == searchHistory[j])
    //                //    {
    //                //        isFind = true;
    //                //        break;
    //                //    }
    //                //}

    //                if (isFind)
    //                {
    //                    //listContainer.transform.GetChild(i).GetComponent<MessageUserDataScript>().textUserName.text = searchHistory[i];
    //                    listContainer.transform.GetChild(i).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    listContainer.transform.GetChild(i).gameObject.SetActive(false);
    //                }
    //            }
    //        }
    //        else if (isSearschForConversationList)
    //        {
    //            for (int i = 0; i < SNS_APIController.Instance.allConversationList.Count; i++)
    //            {
    //                if (!string.IsNullOrEmpty(SNS_APIController.Instance.allConversationList[i]))
    //                {
    //                    string nameStr = SNS_APIController.Instance.allConversationList[i].ToLower();
    //                    if (nameStr.Contains(userInput.ToLower()))
    //                    {
    //                        tmpList.Add(SNS_APIController.Instance.allConversationList[i]);
    //                    }
    //                }
    //            }

    //            for (int i = 0; i < tmpList.Count; i++)
    //            {
    //                if (tmpList[i].StartsWith(userInput.ToLower()))
    //                {
    //                    searchHistory.Add(tmpList[i]);
    //                }
    //                else
    //                {
    //                    tmpList2.Add(tmpList[i]);
    //                }
    //            }

    //            for (int i = 0; i < tmpList2.Count; i++)
    //            {
    //                searchHistory.Add(tmpList2[i]);
    //            }

    //            for (int i = 0; i < listContainer.childCount; i++)
    //            {
    //                bool isFind = false;
                    
    //                for (int j = 0; j < searchHistory.Count; j++)
    //                {
    //                    if (listContainer.transform.GetChild(i).GetComponent<AllConversationData>().textTitle.text == searchHistory[j])
    //                    {
    //                        isFind = true;
    //                        break;
    //                    }
    //                }

    //                //if (i < searchHistory.Count)
    //                if (isFind)
    //                {
    //                    //listContainer.transform.GetChild(i).GetComponent<AllConversationData>().textTitle.text = searchHistory[i];
    //                    listContainer.transform.GetChild(i).gameObject.SetActive(true);
    //                }
    //                else
    //                {
    //                    listContainer.transform.GetChild(i).gameObject.SetActive(false);
    //                }
    //            }
    //        }
    //    }
    //    //  Debug.LogError("userInput" + userInput);
    //}

    #region FindFreiends Search
    //[Header("FindFriends References")]
    //public List<MessageUserDataScript> allMessageUserDataList = new List<MessageUserDataScript>();
    //public List<MessageUserDataScript> searchHistoryAllMessageUserData = new List<MessageUserDataScript>();

    //public void SetUpAllMessageUserData(int pageNum)
    //{
    //    if (pageNum == 1)
    //    {
    //        foreach (Transform item in searchContainer)
    //        {
    //            Destroy(item.gameObject);
    //        }
    //        allMessageUserDataList.Clear();
    //        searchHistoryAllMessageUserData.Clear();
    //    }
    //    for (int i = 0; i < listContainer.childCount; i++)
    //    {
    //        MessageUserDataScript messageUserDataScript = listContainer.GetChild(i).GetComponent<MessageUserDataScript>();
    //        if (!allMessageUserDataList.Contains(messageUserDataScript))
    //        {
    //            allMessageUserDataList.Add(listContainer.GetChild(i).GetComponent<MessageUserDataScript>());
    //            Instantiate(listContainer.GetChild(i).gameObject, searchContainer);
    //        }
    //    }
    //}

    //public void OnTextValueChangeFindFriend()
    //{
    //    //string userInput = _input.text;
    //    string userInput = _inputTMP.Text;
    //    if (string.IsNullOrEmpty(userInput))
    //    {
    //        for (int i = 0; i < listContainer.childCount; i++)
    //        {
    //            listContainer.transform.GetChild(i).gameObject.SetActive(true);
    //        }
    //        SetupAvtiveScollView(false);
    //    }
    //    else
    //    {
    //        SetupAvtiveScollView(true);

    //        searchHistoryAllMessageUserData = new List<MessageUserDataScript>();
    //        List<MessageUserDataScript> tmpList = new List<MessageUserDataScript>();
    //        List<MessageUserDataScript> tmpList2 = new List<MessageUserDataScript>();
    //        for (int i = 0; i < allMessageUserDataList.Count; i++)
    //        {
    //            if (!string.IsNullOrEmpty(allMessageUserDataList[i].textUserName.text))
    //            {
    //                string nameStr = allMessageUserDataList[i].textUserName.text.ToLower();

    //                if (nameStr.Contains(userInput.ToLower()))
    //                {
    //                    tmpList.Add(allMessageUserDataList[i]);
    //                }
    //            }
    //        }

    //        for (int i = 0; i < tmpList.Count; i++)
    //        {
    //            string nameStr = tmpList[i].textUserName.text.ToLower();
    //            if (nameStr.StartsWith(userInput.ToLower()))
    //            {
    //                searchHistoryAllMessageUserData.Add(tmpList[i]);
    //            }
    //            else
    //            {
    //                tmpList2.Add(tmpList[i]);
    //            }
    //        }

    //        for (int i = 0; i < tmpList2.Count; i++)
    //        {
    //            searchHistoryAllMessageUserData.Add(tmpList2[i]);
    //        }

    //        for (int i = 0; i < allMessageUserDataList.Count; i++)
    //        {
    //            if (i < searchHistoryAllMessageUserData.Count)
    //            {
    //                MessageUserDataScript MUD = searchContainer.transform.GetChild(i).GetComponent<MessageUserDataScript>();
    //                MUD.textUserName.text = searchHistoryAllMessageUserData[i].textUserName.text;
    //                MUD.allFollowingRow = searchHistoryAllMessageUserData[i].allFollowingRow;
    //                MUD.profileImage.sprite = searchHistoryAllMessageUserData[i].profileImage.sprite;
    //                MUD.selectionToggle.isOn = searchHistoryAllMessageUserData[i].selectionToggle.isOn;
    //                searchContainer.transform.GetChild(i).gameObject.SetActive(true);
    //            }
    //            else
    //            {
    //                searchContainer.transform.GetChild(i).gameObject.SetActive(false);
    //            }
    //        }
    //    }        
    //}
    #endregion

    #region AllConversation Search.......
    //[Header("AllConversation References")]
    //public List<AllConversationData> allConversationDatasList = new List<AllConversationData>();
    //public List<AllConversationData> searchHistoryAllConversation = new List<AllConversationData>();
    //public IEnumerator SetUpAllConversationData()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    foreach (Transform item in searchContainer)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    allConversationDatasList.Clear();
    //    searchHistoryAllConversation.Clear();
    //    //Debug.LogError("MainContainer child cound:" + listContainer.childCount + "     :searchContainer:" + searchContainer);
    //    for (int i = 0; i < listContainer.childCount; i++)
    //    {
    //        allConversationDatasList.Add(listContainer.GetChild(i).GetComponent<AllConversationData>());
    //        Instantiate(listContainer.GetChild(i).gameObject, searchContainer);
    //    }
    //}

    ////this method are used to Search AllConversation.......
    //public void OnTextValueChangeAllConversation()
    //{
    //    //string userInput = _input.text;
    //    string userInput = _inputTMP.Text;
    //    if (string.IsNullOrEmpty(userInput))
    //    {
    //        for (int i = 0; i < listContainer.childCount; i++)
    //        {
    //            listContainer.transform.GetChild(i).gameObject.SetActive(true);
    //        }
    //        SetupAvtiveScollView(false);
    //    }
    //    else
    //    {
    //        SetupAvtiveScollView(true);

    //        searchHistoryAllConversation = new List<AllConversationData>();
    //        List<AllConversationData> tmpList0000 = new List<AllConversationData>();
    //        List<AllConversationData> tmpList20000 = new List<AllConversationData>();

    //        for (int i = 0; i < allConversationDatasList.Count; i++)
    //        {
    //            if (!string.IsNullOrEmpty(allConversationDatasList[i].textTitle.text))
    //            {
    //                string nameStr = allConversationDatasList[i].textTitle.text.ToLower();
    //                if (nameStr.Contains(userInput.ToLower()))
    //                {
    //                    tmpList0000.Add(allConversationDatasList[i]);
    //                }
    //            }
    //        }

    //        for (int i = 0; i < tmpList0000.Count; i++)
    //        {
    //            string nameStr = tmpList0000[i].textTitle.text.ToLower();
    //            if (nameStr.StartsWith(userInput.ToLower()))
    //            {
    //                searchHistoryAllConversation.Add(tmpList0000[i]);
    //            }
    //            else
    //            {
    //                tmpList20000.Add(tmpList0000[i]);
    //            }
    //        }

    //        for (int i = 0; i < tmpList20000.Count; i++)
    //        {
    //            searchHistoryAllConversation.Add(tmpList20000[i]);
    //        }

    //        for (int i = 0; i < allConversationDatasList.Count; i++)
    //        {
    //            if (i < searchHistoryAllConversation.Count)
    //            {
    //                AllConversationData ACD = searchContainer.transform.GetChild(i).GetComponent<AllConversationData>();
    //                ACD.allChatGetConversationDatum = searchHistoryAllConversation[i].allChatGetConversationDatum;
    //                ACD.LoadFeed();
    //                /*ACD.textTitle.text = searchHistoryAllConversation[i].textTitle.text;
    //                ACD.textLastMessage.text = searchHistoryAllConversation[i].textLastMessage.text;
    //                ACD.textTime.text = searchHistoryAllConversation[i].textTime.text;
    //                ACD.profileImage.sprite = searchHistoryAllConversation[i].profileImage.sprite;
    //                ACD.UnReadCountobject.SetActive(searchHistoryAllConversation[i].UnReadCountobject.activeInHierarchy);
    //                ACD.UnreadMessageText.text = searchHistoryAllConversation[i].UnreadMessageText.text;
    //                ACD.OnlyLoadProfileImage(searchHistoryAllConversation[i].avtarUrl);*/
    //                searchContainer.transform.GetChild(i).gameObject.SetActive(true);
    //            }
    //            else
    //            {
    //                searchContainer.transform.GetChild(i).gameObject.SetActive(false);
    //            }
    //        }
    //    }
    //}
    #endregion
}