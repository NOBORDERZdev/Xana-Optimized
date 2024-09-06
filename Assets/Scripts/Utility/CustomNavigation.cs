using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomNavigation : MonoBehaviour
{
    public static CustomNavigation Instance;
    public List<actiondata> data;
    public GameObject[] parent;
    public GameObject nextbtn;
    public GameObject previous;
    public GameObject reset;
    public int currentindex;
    public bool scriptInvoke;
    
    [Serializable]
    public class actiondata
    {
        public List<GameObject> data;
        public int current;

        public actiondata()
        {
            data = new List<GameObject>();
            current = -1;
            
        }
    }

  

    public void set_currentindex(int pos) { currentindex = pos;

       
            if(data[currentindex].current >= data[currentindex].data.Count-1 && data[currentindex].current <=-1){
                nextbtn.GetComponent<Button>().interactable = false;
                previous.GetComponent<Button>().interactable = false;
            }
            else
           if(data[currentindex].current +1>= data[currentindex].data.Count)
            {
                nextbtn.GetComponent<Button>().interactable = false;
                previous.GetComponent<Button>().interactable = true;
            }
            else
           if(data[currentindex].current <= -1)
            {
                
                nextbtn.GetComponent<Button>().interactable = true;
                previous.GetComponent<Button>().interactable = false;

            }
            else
            {
                nextbtn.GetComponent<Button>().interactable = true;
                previous.GetComponent<Button>().interactable = true;

            }
        
    }
    public void addData(GameObject obje)
    {
        if (scriptInvoke)
        {
            scriptInvoke = false;
          // Debug.Log("script invoke");
            return;
        }
      // Debug.Log(data[currentindex].data.Count);
        if (data[currentindex].data.Count <= data[currentindex].current+1) {
            data[currentindex].data.Add(obje);
            data[currentindex].current = data[currentindex].data.Count-1;
        //   Debug.Log("current" + data[currentindex].current);
        }
        else
        {
           
            data[currentindex].data[data[currentindex].current+1] =obje;
            data[currentindex].current += 1;
           Debug.Log(data[currentindex].current + "  ,  " + data[currentindex].current + 1);
            

        }

        if (data[currentindex].data.Count != 0)
        {
            if (data[currentindex].current >= data[currentindex].data.Count-1 && data[currentindex].current <= -1)
            {
                nextbtn.GetComponent<Button>().interactable = false;
                previous.GetComponent<Button>().interactable = false;
            }else
            if (data[currentindex].current >= data[currentindex].data.Count-1)
            {
                nextbtn.GetComponent<Button>().interactable = false;
                previous.GetComponent<Button>().interactable = true;
            }
            else
            if (data[currentindex].current <= -1)
            {
                nextbtn.GetComponent<Button>().interactable = true;
                previous.GetComponent<Button>().interactable = false;

            }
            else
            {
                nextbtn.GetComponent<Button>().interactable = true;
                previous.GetComponent<Button>().interactable = true;

            }
        }
    }


    private void Awake()
   {
        Instance = this;
        data = new List<actiondata>();
    foreach (GameObject obj in parent)
        {
            
           
                for (int i = 1; i < obj.transform.childCount; i++)
                {
                    
            
                        data.Add(new actiondata());
                    
                }
            
        }
    }

    public void Resetdata()
    {
       // FindObjectOfType<CustomFaceMorph>().ResetMorphing();
         
        //FindObjectOfType<ChangeGear>().UnequipItem("Legs", FindObjectOfType<Equipment>().wornLegs.gameObject.name);
        //FindObjectOfType<ChangeGear>().EquipItem("Legs", "MDpants");

        //FindObjectOfType<ChangeGear>().UnequipItem("Chest", FindObjectOfType<Equipment>().wornChest.gameObject.name);
        //FindObjectOfType<ChangeGear>().EquipItem("Chest", "MDgambeson");
        

    }
    public void Save()
    {
        BodyFaceCustomizer.Instance.save();
        //FindObjectOfType<loadBundle>().save();
     

    }
    public void next()
    {
        if (data[currentindex].current + 1 >= data[currentindex].data.Count-1)
        {
            nextbtn.GetComponent<Button>().interactable = false;
        }

        previous.GetComponent<Button>().interactable = true;
        data[currentindex].current += 1;
       Debug.Log("current next " + data[currentindex].current);
        scriptInvoke = true;
        data[currentindex].data[data[currentindex].current].GetComponent<Button>().onClick.Invoke();
        BodyFaceCustomizer.Instance.setprev();

    }

    public void prev()
    {
        //if (data[currentindex].current - 1 <= -1)
        //{
        //    previous.GetComponent<Button>().interactable=false;
        //}

        //nextbtn.GetComponent<Button>().interactable = true;
        //data[currentindex].current -= 1;
       
        //if (data[currentindex].current == -1)
        //{
        //    if(data[currentindex].data[data[currentindex].current + 1].transform.parent.parent.parent.parent.name== "SkirtsSelection")
        //    {
        //        if (PlayerPrefs.GetString("player pant") != "" && PlayerPrefs.GetString("player shirt") != "MDpants")
        //        {
        //            // DefaultClothDatabase.instance.itemList.Add(new Item(UnityEngine.Random.Range(0, 1000), PlayerPrefs.GetString("player pant"), "", PlayerPrefs.GetString("player pant"), "Legs", TextureandMeshwithDownloaded(PlayerPrefs.GetString("player pant"))));
        //            //   ui.AddOrRemoveClothes("naked_legs", "Legs", PlayerPrefs.GetString("player pant"), 0);
        //             FindObjectOfType<ChangeGear>().UnequipItem("Legs", FindObjectOfType<Equipment>().wornLegs.gameObject.name);
        //            FindObjectOfType<ChangeGear>().EquipItem("Legs", PlayerPrefs.GetString("player pant"));
        //        }
        //        else
        //        {
        //            FindObjectOfType<ChangeGear>().UnequipItem("Legs", FindObjectOfType<Equipment>().wornLegs.gameObject.name);
        //            FindObjectOfType<ChangeGear>().EquipItem("Legs", "MDpants");
        //        }
        //        return;
        //    }
        //    if  (data[currentindex].data[data[currentindex].current + 1].transform.parent.parent.parent.parent.name == "CostumeSelection")
        //    {
        //        if (PlayerPrefs.GetString("player shirt") != "" && PlayerPrefs.GetString("player shirt") != "MDgambeson")
        //        {
        //            // DefaultClothDatabase.instance.itemList.Add(new Item(UnityEngine.Random.Range(0, 1000), PlayerPrefs.GetString("player shirt"), "", PlayerPrefs.GetString("player shirt"), "Chest", TextureandMeshwithDownloaded(PlayerPrefs.GetString("player shirt"))));

        //            // ui.AddOrRemoveClothes("naked_chest", "Chest", PlayerPrefs.GetString("player shirt"), 1);
        //             FindObjectOfType<ChangeGear>().UnequipItem("Chest", FindObjectOfType<Equipment>().wornChest.gameObject.name);
        //            FindObjectOfType<ChangeGear>().EquipItem("Chest", PlayerPrefs.GetString("player shirt"));
        //        }
        //        else
        //        {
        //              FindObjectOfType<ChangeGear>().UnequipItem("Chest", FindObjectOfType<Equipment>().wornChest.gameObject.name);
        //            FindObjectOfType<ChangeGear>().EquipItem("Chest", "MDgambeson");

        //        }
        //        return;
        //    }
        //    if (data[currentindex].data[data[currentindex].current + 1].transform.parent.parent.parent.parent.name == "ShoesSelection")
        //    {
        //        FindObjectOfType<ChangeGear>().UnequipItem("Feet", FindObjectOfType<Equipment>().wornBoots.gameObject.name);
        //        return;
        //    }
        //    //if (data[currentindex].data[data[currentindex].current + 1].transform.parent.parent.parent.parent.name == "HairsSelection")
        //    //{

        //    //    FindObjectOfType<BodyMorphing>().HairSelection(PlayerPrefs.GetInt("player hair",0));
        //    //   Debug.Log("Hairsection");
        //    //}
        //        BodyFaceCustomizer.Instance.apply_prev();
        //  //  data[currentindex].data[data[currentindex].current+1].transform.parent.gameObject.transform.GetChild(0).GetComponent<Button>().onClick.Invoke();
        // //   scriptInvoke = true;
        //    return;
        //}
        //Debug.Log("current previous " + data[currentindex].current);
        //scriptInvoke = true;
        //data[currentindex].data[data[currentindex].current].GetComponent<Button>().onClick.Invoke();
        //BodyFaceCustomizer.Instance.setprev();
    }   
    
}
