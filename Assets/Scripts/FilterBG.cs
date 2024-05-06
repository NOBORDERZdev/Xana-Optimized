using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class FilterBG : MonoBehaviour
{

    //public PostProcessProfile profile;
    public GameObject contentCategoryPanel;
    public GameObject categoryPrefab;
    public static FilterBG instance;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetAllFilter());
    }
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickFilterItem(byte r, byte g, byte b, byte a)
    {
        //for (int i = 0; i < transform.parent.childCount; i++)
        //{
        //    transform.parent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        //    transform.parent.GetChild(i).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
        //}
        //transform.GetChild(1).gameObject.SetActive(true);
        //transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.blue;


        //profile.GetSetting<ColorGrading>().colorFilter.Override(new Color32(r,g,b,a));
        // LiveVideoRoomManager.Instance.mainVolume.profile = profile;

        if (LiveVideoRoomManager.Instance.filterMainVolume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var color))
        {
            color.colorFilter.overrideState = true;
            color.colorFilter.showAlpha = true;
            color.colorFilter.value = new Color32(r, g, b, a);
        }
    }



    IEnumerator GetAllFilter()
    {
        //Debug.Log("All Anim");
        UnityWebRequest uwr = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.FILTERPROFILE);
        try
        {
            uwr.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        }
        catch (Exception e1)
        {
        }

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {

            Debug.Log("response====" + uwr.downloadHandler.text.ToString());
            FilterProfile bean = JsonUtility.FromJson<FilterProfile>(uwr.downloadHandler.text.ToString());
            if (!bean.Equals("") || !bean.Equals(null))
            {

                for (int i = 0; i < bean.data.filter.Count; i++)
                {
                    //Debug.Log("response====" + bean.data.filter.Count);
                    GameObject categoryObject;
                    categoryObject = Instantiate(categoryPrefab);
                    categoryObject.transform.SetParent(contentCategoryPanel.transform);
                    categoryObject.transform.localPosition = Vector3.zero;
                    categoryObject.transform.localScale = Vector3.one;
                    categoryObject.transform.localRotation = Quaternion.identity;

                    categoryObject.transform.GetComponent<Image>().color = new Color32(byte.Parse(bean.data.filter[i].color.R), byte.Parse(bean.data.filter[i].color.G), byte.Parse(bean.data.filter[i].color.B), 255);
                    categoryObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = bean.data.filter[i].name;
                    categoryObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color32(115, 115, 115, 255);

                    BackGroundItem LBC = categoryObject.GetComponent<BackGroundItem>();

                    if (LBC == null)
                    {
                        LBC = categoryObject.AddComponent<BackGroundItem>();
                    }
                    LBC.Initializ(byte.Parse(bean.data.filter[i].color.R), byte.Parse(bean.data.filter[i].color.G)
                        , byte.Parse(bean.data.filter[i].color.B), byte.Parse(bean.data.filter[i].color.A), this,
                        contentCategoryPanel.gameObject);

                }
            }


        }
    }
    [System.Serializable]
    public class Color
    {
        public string R;
        public string G;
        public string B;
        public string A;
    }
    [System.Serializable]
    public class Filter
    {
        public int id;
        public string name;
        public Color color;
    }
    [System.Serializable]
    public class Data
    {
        public List<Filter> filter;
    }
    [System.Serializable]
    public class FilterProfile
    {
        public bool success;
        public Data data;
        public string msg;
    }

}
