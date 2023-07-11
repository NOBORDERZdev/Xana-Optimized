using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Cinemachine;

public class CanvusHandler : MonoBehaviour
{
    public static CanvusHandler canvusHandlerInstance = new CanvusHandler();

    public GameObject minimapObj_UI;
        
    public CinemachineClearShot _VcamClear;

    public bool is_Trigger;

    public GameObject info_Panel,_characterCustomPanel;
    public string display_InfoString;
  //  public TMP_Text text_display;
    public GameObject AskCommand_text_display;
    public GameObject button_Obj;

    public GameObject Player;

    public Toggle boolButton;

    public GameObject andoridCanvus;

    public bool _IsCat, _IsBunny, _IsBear;

    public enum CustomCharacter { cat,bunny,bear };
    // public int _isC

    public CustomCharacter customCharacterSelected,tempCustomCharacter;

    public GameObject[] CustomAvatars;//, BearAvatar, CatAvatat;

    private void Awake()
    {
        if (canvusHandlerInstance == null)
            canvusHandlerInstance = this;
        else
            Destroy(this);
       
      
    }

    private void Update()
    {
       

    }

    public void Display()
    {
        if ((is_Trigger))
        {
            andoridCanvus.SetActive(false);

            Debug.Log("Display");
            //AskCommand_text_display.SetActive(false);
            //button_Obj.SetActive(false);
            ////  text_display.text = display_InfoString;
            //info_Panel.SetActive(true);
        }
        else
        {
            andoridCanvus.SetActive(true);
            Debug.Log("Input.GetKeyDown(KeyCode.E) ");
          //  text_display.text = null;
            info_Panel.SetActive(false);
        }
    }

    public void offallVcam()
    {
        
        foreach (var item in _VcamClear.ChildCameras)
        {
            item.gameObject.SetActive(false);
        }
    }


    public void is_AdvantureCheck()
    {
         //Player.GetComponent<PlayerControllerCustom>().is_Advanture= boolButton.isOn;
    }

    public void BacktoMainApp()
    {

    }

    public GameObject chatPanel;

    public bool check;
    public void ShowChatPanel()
    {
        if (!check)
        {
            chatPanel.SetActive(true);
            check = true;
            chatPanel.GetComponent<Animator>().SetBool("chatpanel", true);
            
        }
        else
          if (check)
        {
            
            check = false;
            chatPanel.GetComponent<Animator>().SetBool("chatpanel", false);
            Invoke("OffChatPanel", 0.5f);
        }

    }
    void OffChatPanel()
    {
        chatPanel.SetActive(false);
    }

    public bool isGyro = false;
    public Toggle gyroToggle;
    public void GyroButton()
    {
        Debug.Log("Pressed");
        if(!isGyro)
        {
          //  isGyro = gyroToggle.isOn;
            if(UnityEngine.InputSystem.Gyroscope.current.name==null)
            {
                isGyro = false;
                gyroToggle.isOn = isGyro;
               
                return;
            }
            else
            {
                isGyro = true;
                gyroToggle.isOn = isGyro;
                InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
                // InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
                return;
            }
           
        }
       
    }

    void Offchatpanel()
    {
        chatPanel.SetActive(false);
    }

    //public bool DummyActive
    public void CharacterCustomButton(bool pressed)
    {
        if (pressed)
        {
            Gamemanager._InstanceGM.moveDeummy = false;
            Gamemanager._InstanceGM.moveDeummy = false;
            //_characterCustomPanel.SetActive(true);
        }
        else
        {
            Gamemanager._InstanceGM.moveDeummy = true;
            Gamemanager._InstanceGM.moveDeummy = true;
            //_characterCustomPanel.SetActive(false);
        }
    }
  

    public void CharacterSelection(int n)
    {
        if(n==0)
        {
            tempCustomCharacter= CustomCharacter.bunny;
           
             _IsBunny = true;
            _IsCat = false;
            _IsBear = false;
            Select(0, _IsBunny);


        }
        else
             if (n == 1)
        {
            tempCustomCharacter = CustomCharacter.cat;//customCharacterSelected 
            _IsBunny = false;
            _IsCat = true;
            _IsBear = false;
            Select(1, _IsCat);
        }
        else
             if (n == 2)
        {
            tempCustomCharacter = CustomCharacter.bear;
            _IsBunny = false;
            _IsCat = false;
            _IsBear = true;
            Select(2, _IsBear);
        }
    }

    public void Select(int index,bool check)
    {
        foreach (GameObject n in CustomAvatars)
        {
            n.SetActive(false);
        }
        CustomAvatars[index].SetActive(check);
    }

    public void ChangeButtonCommit()
    {
        customCharacterSelected = tempCustomCharacter;

        CharacterCustomButton(false);
    }



}
