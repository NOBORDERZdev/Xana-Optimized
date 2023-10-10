using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;

public class AllWorldManage : MonoBehaviour
{
    [Header("Home Page Scrollviews and Component")]
    public List<GameObject> HighlghterList = new List<GameObject>();
    public List<GameObject> HighlghterListText = new List<GameObject>();
    public List<GameObject> ScrollObjectsList = new List<GameObject>();
    public List<TextMeshProUGUI> ScrollerText = new List<TextMeshProUGUI>();
    public GameObject FlexibleReact;

    [Header("World Page Scrollviews and Component")]
    public Button searchButton;
    public TMP_InputField searchBar;
    public List<GameObject> worldPageScrollviews = new List<GameObject>();
    public List<GameObject> WorldPagehighlighters = new List<GameObject>();
    public List<GameObject> WorldPagehighlightersText = new List<GameObject>();
    public List<TextMeshProUGUI> WorldPageText = new List<TextMeshProUGUI>();






    public void XanaHotWorldLoad()
    {
        if (!ScrollObjectsList[0].activeSelf)
        {
            ScrollEnableDisable(0);
            FlexibleReact.GetComponent<FlexibleRectNewWorld>().enabled = true;
            FlexibleReact.GetComponent<FlexibleRectNewCustomWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewGameWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectTestWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld_New>().enabled = false;
            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.Hot, true);
        }
    }


    bool allWorldLoaded = true;
    public void NewWorldLoad()
    {
        if (!ScrollObjectsList[1].activeSelf)
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem("NewBuilderWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }


            ScrollEnableDisable(1);
            FlexibleReact.GetComponent<FlexibleRectNewWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewCustomWorld>().enabled = true;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewGameWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectTestWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld_New>().enabled = false;

            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.AllWorld, true);
            if (allWorldLoaded)
            {
                allWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.AllWorld, (sucess) =>
                 {
                     if (!sucess)
                         SetTextForScroller("No world published yet.", ScrollerText[1]);
                     else
                         ScrollerText[1].gameObject.SetActive(false);
                 });
            }
        }

    }
    bool myWorldLoaded = true;
    public void MyWorldLoad()
    {
        if (!ScrollObjectsList[2].activeSelf)
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem("MyBuilderWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }

            ScrollEnableDisable(2);
            FlexibleReact.GetComponent<FlexibleRectNewWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewCustomWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld>().enabled = true;
            FlexibleReact.GetComponent<FlexibleRectNewGameWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectTestWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld_New>().enabled = false;

            if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            {
                BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.MyWorld, true);
                if (myWorldLoaded)
                {
                    myWorldLoaded = false;
                    BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.MyWorld, (sucess) =>
                     {
                         if (!sucess)
                             SetTextForScroller("You have not created any world yet.", ScrollerText[2]);
                         else
                             ScrollerText[2].gameObject.SetActive(false);
                     });
                }

            }
            else
            {
                SetTextForScroller("You have not created any world yet.", ScrollerText[2]);
            }
        }
    }

    bool gameWorldLoaded = true;
    public void GameWorldLoad()
    {
        if (!ScrollObjectsList[3].activeSelf)
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem("GameWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }


            ScrollEnableDisable(3);
            FlexibleReact.GetComponent<FlexibleRectNewWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewCustomWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewGameWorld>().enabled = true;
            FlexibleReact.GetComponent<FlexibleRectTestWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld_New>().enabled = false;

            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.GameWorld, true);
            if (gameWorldLoaded)
            {
                gameWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.GameWorld, (sucess) =>
                {
                    if (!sucess)
                        SetTextForScroller("No world published yet.", ScrollerText[3]);
                    else
                        ScrollerText[3].gameObject.SetActive(false);
                });
            }
        }
    }

    bool eventWorldLoaded = true;
    public void EventWorldLoad()   //my worlds method name is also same so add new here for event category
    {
        if (!ScrollObjectsList[4].activeSelf)
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem("EventWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }


            ScrollEnableDisable(4);
            FlexibleReact.GetComponent<FlexibleRectNewWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewCustomWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewGameWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectTestWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld_New>().enabled = true;

            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.EventWorld, true);
            if (eventWorldLoaded)
            {
                eventWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.EventWorld, (sucess) =>
                {
                    if (!sucess)
                        SetTextForScroller("No world published yet.", ScrollerText[4]);
                    else
                        ScrollerText[4].gameObject.SetActive(false);
                });
            }
        }
    }


    public void TestWorldLoad()
    {
        if (!ScrollObjectsList[5].activeSelf)
        {
            if (!PremiumUsersDetails.Instance.CheckSpecificItem("TestWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }


            ScrollEnableDisable(5);
            FlexibleReact.GetComponent<FlexibleRectNewWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewCustomWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewGameWorld>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectNewEventWorld_New>().enabled = false;
            FlexibleReact.GetComponent<FlexibleRectTestWorld>().enabled = true;

            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.TestWorld, true);
            if (eventWorldLoaded)
            {
                eventWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.TestWorld, (sucess) =>
                {
                    if (!sucess)
                        SetTextForScroller("No world published yet.", ScrollerText[5]);
                    else
                        ScrollerText[5].gameObject.SetActive(false);
                });
            }
        }
    }




    void SetTextForScroller(string textToChange, TextMeshProUGUI text)
    {
        text.text = textToChange;
        text.gameObject.SetActive(true);
    }


    void ScrollEnableDisable(int index)
    {
        for (int i = 0; i < ScrollObjectsList.Count; i++)
        {
            ScrollObjectsList[i].SetActive(false);
            HighlghterList[i].SetActive(false);
            HighlghterListText[i].SetActive(false);
        }
        HighlghterList[index].SetActive(true);
        ScrollObjectsList[index].SetActive(true);
        HighlghterListText[index].SetActive(true);
    }


    public void WorldHotPage()
    {
        if (!worldPageScrollviews[0].activeSelf)
        {

            WorldScrollEnableDisable(0);
            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.Hot, false);
        }
    }
    public void WorldNewPage()
    {
        if (!worldPageScrollviews[1].activeSelf)
        {

            if (!PremiumUsersDetails.Instance.CheckSpecificItem("NewBuilderWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }
            WorldScrollEnableDisable(1);
            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.AllWorld, false);
            if (allWorldLoaded)
            {
                allWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.AllWorld, (sucess) =>
                {
                    if (!sucess)
                        WorldPageSetTextForScroller("No world published yet.", WorldPageText[1]);
                    else
                        WorldPageText[1].gameObject.SetActive(false);
                });
            }
        }

    }
    public void WorldMyWorldPage()
    {
        if (!worldPageScrollviews[2].activeSelf)
        {

            if (!PremiumUsersDetails.Instance.CheckSpecificItem("MyBuilderWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }

            WorldScrollEnableDisable(2);
            if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            {
                BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.MyWorld, false);
                if (myWorldLoaded)
                {
                    myWorldLoaded = false;
                    BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.MyWorld, (sucess) =>
                    {
                        if (!sucess)
                            WorldPageSetTextForScroller("You have not created any world yet.", WorldPageText[2]);
                        else
                            WorldPageText[2].gameObject.SetActive(false);
                    });
                }
            }
            else
            {
                WorldPageSetTextForScroller("You have not created any world yet.", WorldPageText[2]);
            }
        }
    }



    public void WorldGamePage()
    {
        if (!worldPageScrollviews[3].activeSelf)
        {

            if (!PremiumUsersDetails.Instance.CheckSpecificItem("GameWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }

            WorldScrollEnableDisable(3);
            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.GameWorld, false);
            if (myWorldLoaded)
            {
                myWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.GameWorld, (sucess) =>
                {
                    if (!sucess)
                        WorldPageSetTextForScroller("You have not created any world yet.", WorldPageText[3]);
                    else
                        WorldPageText[3].gameObject.SetActive(false);
                });
            }
        }
    }



    public void WorldEventPage()
    {
        if (!worldPageScrollviews[4].activeSelf)
        {

            if (!PremiumUsersDetails.Instance.CheckSpecificItem("EventWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }

            WorldScrollEnableDisable(4);
            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.EventWorld, false);
            if (myWorldLoaded)
            {
                myWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.EventWorld, (sucess) =>
                {
                    if (!sucess)
                        WorldPageSetTextForScroller("You have not created any world yet.", WorldPageText[4]);
                    else
                        WorldPageText[4].gameObject.SetActive(false);
                });
            }

        }
    }


    public void WorldTESTPage()
    {
        if (!worldPageScrollviews[5].activeSelf)
        {

            if (!PremiumUsersDetails.Instance.CheckSpecificItem("TestWorlds"))
            {
                return;
            }
            else
            {
                print("Horayyy you have Access");
            }

            WorldScrollEnableDisable(5);
            BuilderEventManager.OnWorldTabChange?.Invoke(APIURL.TestWorld, false);
            if (myWorldLoaded)
            {
                myWorldLoaded = false;
                BuilderEventManager.OnBuilderWorldLoad?.Invoke(APIURL.TestWorld, (sucess) =>
                {
                    if (!sucess)
                        WorldPageSetTextForScroller("You have not created any world yet.", WorldPageText[5]);
                    else
                        WorldPageText[5].gameObject.SetActive(false);
                });
            }

        }
    }





    void WorldScrollEnableDisable(int index)
    {
        for (int i = 0; i < worldPageScrollviews.Count; i++)
        {
            worldPageScrollviews[i].SetActive(false);
            WorldPagehighlighters[i].SetActive(false);
            WorldPagehighlightersText[i].SetActive(false);
        }
        worldPageScrollviews[index].SetActive(true);
        WorldPagehighlighters[index].SetActive(true);
        WorldPagehighlightersText[index].SetActive(true);
    }

    void WorldPageSetTextForScroller(string textToChange, TextMeshProUGUI text)
    {
        text.text = textToChange;
        text.gameObject.SetActive(true);
    }
}
