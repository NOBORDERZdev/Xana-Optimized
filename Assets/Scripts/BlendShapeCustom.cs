using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlendShapeCustom : MonoBehaviour
{


    public Button SaveStoreBtn;
    public GameObject GreyRibbonImage, WhiteRibbonImage;
    public Button next, previous;


    public List<ReduUnduCLass> red1;
    public int count, listcount;
    SkinnedMeshRenderer characterHead;
    // Start is called before the first frame update
    void OnEnable()
    {

        red1 = new List<ReduUnduCLass>();
        count = 0;
        chk = false;
        SaveStoreBtn.GetComponent<Image>().color = Color.white;
        GreyRibbonImage.gameObject.SetActive(true);
        WhiteRibbonImage.gameObject.SetActive(false);
        characterHead = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>();
    }

    public void ResettoLastSavedMorphsOnly()
    {

        //  if (File.Exists(GameManager.Instance.GetStringFolderPath()) && File.ReadAllText(GameManager.Instance.GetStringFolderPath()) != "")
        //   {
        SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
        _CharacterData = _CharacterData.CreateFromJSON(File.ReadAllText(GameManager.Instance.GetStringFolderPath()));

        //  for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
        //  {
        ///      characterHead.SetBlendShapeWeight(i, _CharacterData.FaceBlendsShapes[i]);
        //  }

        // }
        // print(BlendShapeImporter.Instance.SelectedMorph);
        for (int i = 0; i < characterHead.sharedMesh.blendShapeCount; i++)
        {
            if (characterHead.sharedMesh.GetBlendShapeName(i).Contains(BlendShapeImporter.Instance.SelectedPart) && i < _CharacterData.FaceBlendsShapes.Length)
                characterHead.SetBlendShapeWeight(i, _CharacterData.FaceBlendsShapes[i]);
        }
        BlendShapeImporter.Instance.SliderY.gameObject.SetActive(false);
        BlendShapeImporter.Instance.SliderX.gameObject.SetActive(false);
        BlendShapeImporter.Instance.SetAllColors(false);
    }
    public void savingIndex()
    {
        // enable save button
        SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
        GreyRibbonImage.gameObject.SetActive(false);
        WhiteRibbonImage.gameObject.SetActive(true);
        //

        //// add  multiple clicks to list  get the index plus the value of index 
        SkinnedMeshRenderer characterHead = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>();


        // new work
        ReduUnduCLass redunud = new ReduUnduCLass();

        redunud.indx = new float[GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount];
        for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            redunud.indx[i] = characterHead.GetBlendShapeWeight(i);
        red1.Add(redunud);
        chk = false;
        // new work
    }
    bool chk;



    public void globalCheck(bool dir)
    {
        if (!chk)
        {

            count = red1.Count;
            if (red1.Count != 0)
                chk = true;
            else
                return;
        }


        SkinnedMeshRenderer characterHead = GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>();

        if (dir)
        {
            if (count < red1.Count)
            {

                for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
                    characterHead.SetBlendShapeWeight(i, red1[count].indx[i]);//   
                count++;
            }
        }
        else
        {

            if (count > 0)
                count--;

            for (int i = 0; i < GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
                characterHead.SetBlendShapeWeight(i, red1[count].indx[i]);//                                                                     
        }
        BlendShapeImporter.Instance.SliderY.gameObject.SetActive(false);
        BlendShapeImporter.Instance.SliderX.gameObject.SetActive(false);
        BlendShapeImporter.Instance.SetAllColors(false);
    }


    public void NextIndex_List()
    {


    }

    // Update is called once per frame
    public void SaveButtonOn()
    {
        // savingIndex();
        //    SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
        //   GreyRibbonImage.gameObject.SetActive(false);
        //   WhiteRibbonImage.gameObject.SetActive(true);
    }
    public void SaveButtonOff()
    {
        SaveStoreBtn.GetComponent<Image>().color = Color.white;
        GreyRibbonImage.SetActive(true);
        WhiteRibbonImage.SetActive(false);
    }

    public class ReduUnduCLass
    {
        public float[] indx;// = new float[GameManager.Instance.m_ChHead.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount];
    }
}
