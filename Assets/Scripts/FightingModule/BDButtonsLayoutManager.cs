using ControlFreak2;
using System;
using UnityEngine;
using UnityEngine.UI;


public class BDButtonsLayoutManager : MonoBehaviour
{
    public Button saveLayoutBtn;
    public Button closePanelBtn;
    public Button switchToIcons;
    public Button switchToInitials;

    public GameObject currentSelectedObject;

    public RectTransform joyStick, LK, LP, HP, HK, SP, B;

    public Slider sizeSlider;

    public static BDButtonsLayoutManager instance;


    public bool destroyMe = true;

    public Sprite[] btnInitials, btnIcons;
    public Image[] buttonImages;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(ResizeUI);
        switchToIcons.onClick.AddListener(() => SwitchIcons(true));
        switchToInitials.onClick.AddListener(() => SwitchIcons(false));
    }

   
    private void OnDisable()
    {
        sizeSlider.value = 0;
        currentSelectedObject = null;
        sizeSlider.onValueChanged.RemoveAllListeners();
        switchToIcons.onClick.RemoveAllListeners();
        switchToInitials.onClick.RemoveAllListeners();
    }
    private void Start()
    {
        LoadLayout();
        SelectIcons();
        if (destroyMe)
        {
            destroyMe = false;
            gameObject.SetActive(false);
        }
    }


    public void SelectIcons() {

        if (PlayerPrefs.GetInt("icons") == 0)
        {
            SetToInitials();
        }
        else
        {
            SetToIcons();
        }
    }


    private void SetToIcons()
    {
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i].sprite = btnIcons[i];
            BDCanvasButtonsHandler.inst.buttonImgs[i].sprite = btnIcons[i];
            BDCanvasButtonsHandler.inst.buttonImgs2[i].spriteNeutral.sprite = btnIcons[i];
        }
    }


    private void SetToInitials()
    {
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i].sprite = btnInitials[i];
            BDCanvasButtonsHandler.inst.buttonImgs[i].sprite = btnInitials[i];
            BDCanvasButtonsHandler.inst.buttonImgs2[i].spriteNeutral.sprite = btnInitials[i];
        }
    }

    public void SwitchIcons(bool _icons) {
        if (_icons) {
            SetToIcons();
            PlayerPrefs.SetInt("icons", 1);
        }
        else
        {
            SetToInitials();
            PlayerPrefs.SetInt("icons", 0);
        }
    }
    

    #region Layout Placement & Resizing Related Settings
    
    public void SaveLayout()
    {
        PlayerPrefs.SetFloat("joyX", joyStick.anchoredPosition.x);
        PlayerPrefs.SetFloat("joyY", joyStick.anchoredPosition.y);
        PlayerPrefs.SetFloat("joyScaleX", joyStick.transform.localScale.x);
        PlayerPrefs.SetFloat("joyScaleY", joyStick.transform.localScale.y);

        PlayerPrefs.SetFloat("LKX", LK.anchoredPosition.x); 
        PlayerPrefs.SetFloat("LKY", LK.anchoredPosition.y);
        PlayerPrefs.SetFloat("LKScaleX", LK.transform.localScale.x);
        PlayerPrefs.SetFloat("LKScaleY", LK.transform.localScale.y);

        PlayerPrefs.SetFloat("LPX", LP.anchoredPosition.x); 
        PlayerPrefs.SetFloat("LPY", LP.anchoredPosition.y);
        PlayerPrefs.SetFloat("LPScaleX", LP.transform.localScale.x);
        PlayerPrefs.SetFloat("LPScaleY", LP.transform.localScale.y);

        PlayerPrefs.SetFloat("HPX", HP.anchoredPosition.x); 
        PlayerPrefs.SetFloat("HPY", HP.anchoredPosition.y);
        PlayerPrefs.SetFloat("HPScaleX", HP.transform.localScale.x);
        PlayerPrefs.SetFloat("HPScaleY", HP.transform.localScale.y);
        
        PlayerPrefs.SetFloat("HKX", HK.anchoredPosition.x); 
        PlayerPrefs.SetFloat("HKY", HK.anchoredPosition.y);
        PlayerPrefs.SetFloat("HKScaleX", HK.transform.localScale.x);
        PlayerPrefs.SetFloat("HKScaleY", HK.transform.localScale.y);
        
        PlayerPrefs.SetFloat("SPX", SP.anchoredPosition.x); 
        PlayerPrefs.SetFloat("SPY", SP.anchoredPosition.y);
        PlayerPrefs.SetFloat("SPScaleX", SP.transform.localScale.x);
        PlayerPrefs.SetFloat("SPScaleY", SP.transform.localScale.y);
        
        PlayerPrefs.SetFloat("BX", B.anchoredPosition.x); 
        PlayerPrefs.SetFloat("BY", B.anchoredPosition.y);
        PlayerPrefs.SetFloat("BScaleX", B.transform.localScale.x);
        PlayerPrefs.SetFloat("BScaleY", B.transform.localScale.y);



        BDCanvasButtonsHandler.inst.joystick.anchoredPosition = new Vector3(joyStick.anchoredPosition.x, joyStick.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.joystick.transform.localScale = new Vector3(joyStick.transform.localScale.x, joyStick.transform.localScale.y, 0);
   //     GamePlayUIHandler.inst.joystick.gameObject.GetComponent<TouchButton>().SetupNewInits();

        BDCanvasButtonsHandler.inst.LK.anchoredPosition = new Vector3(LK.anchoredPosition.x, LK.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.LK.transform.localScale = new Vector3(LK.transform.localScale.x, LK.transform.localScale.y, 0);
      //  GamePlayUIHandler.inst.LK.gameObject.GetComponent<TouchButton>().SetupNewInits();


        BDCanvasButtonsHandler.inst.LP.anchoredPosition = new Vector3(LP.anchoredPosition.x, LP.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.LP.transform.localScale = new Vector3(LP.transform.localScale.x, LP.transform.localScale.y, 0);
       // GamePlayUIHandler.inst.LP.gameObject.GetComponent<TouchButton>().SetupNewInits();

        BDCanvasButtonsHandler.inst.HP.anchoredPosition = new Vector3(HP.anchoredPosition.x, HP.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.HP.transform.localScale = new Vector3(HP.transform.localScale.x, HP.transform.localScale.y, 0);
       // GamePlayUIHandler.inst.HK.gameObject.GetComponent<TouchButton>().SetupNewInits();

        BDCanvasButtonsHandler.inst.HK.anchoredPosition = new Vector3(HK.anchoredPosition.x, HK.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.HK.transform.localScale = new Vector3(HK.transform.localScale.x, HK.transform.localScale.y, 0);
      //  GamePlayUIHandler.inst.HK.gameObject.GetComponent<TouchButton>().SetupNewInits();

        BDCanvasButtonsHandler.inst.SP.anchoredPosition = new Vector3(SP.anchoredPosition.x, SP.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.SP.transform.localScale = new Vector3(SP.transform.localScale.x, SP.transform.localScale.y, 0);
     //   GamePlayUIHandler.inst.HK.gameObject.GetComponent<TouchButton>().SetupNewInits();

        BDCanvasButtonsHandler.inst.B.anchoredPosition = new Vector3(B.anchoredPosition.x, B.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.B.transform.localScale = new Vector3(B.transform.localScale.x, B.transform.localScale.y, 0);
        //   GamePlayUIHandler.inst.B.gameObject.GetComponent<TouchButton>().SetupNewInits();

        ApplyNewSettings();

        PlayerPrefs.SetInt("ControlsSet", 1);
        PlayerPrefs.Save();

    }
    
    
    void ApplyNewSettings() {
        BDCanvasButtonsHandler.inst.joystick.gameObject.GetComponent<TouchJoystick>().SetupNewInits("JOYSTICK");
        BDCanvasButtonsHandler.inst.LK.gameObject.GetComponent<TouchButton>().SetupNewInits("LK");
        BDCanvasButtonsHandler.inst.LP.gameObject.GetComponent<TouchButton>().SetupNewInits("LP");
        BDCanvasButtonsHandler.inst.HK.gameObject.GetComponent<TouchButton>().SetupNewInits("HK");
        BDCanvasButtonsHandler.inst.HP.gameObject.GetComponent<TouchButton>().SetupNewInits("HP");
        BDCanvasButtonsHandler.inst.SP.gameObject.GetComponent<TouchButton>().SetupNewInits("SP");
        BDCanvasButtonsHandler.inst.B.gameObject.GetComponent<TouchButton>().SetupNewInits("BLOCK");
    }
    
    
    public void LoadLayout()
    {

        joyStick.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("joyX", 149f), PlayerPrefs.GetFloat("joyY", 75.90f), 0); //(149.30, 76.38)
        joyStick.transform.localScale = new Vector3(PlayerPrefs.GetFloat("joyScaleX", 1), PlayerPrefs.GetFloat("joyScaleY", 1), 0);

        LK.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("LKX", -116.40f), PlayerPrefs.GetFloat("LKY", 144.90f), 0); //LK : (950.07, 145.80)
        LK.transform.localScale = new Vector3(PlayerPrefs.GetFloat("LKScaleX", 1), PlayerPrefs.GetFloat("LKScaleY", 1), 0);

        LP.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("LPX", -231.70f), PlayerPrefs.GetFloat("LPY", 144.90f), 0); //LP : (834.77, 145.80)
        LP.transform.localScale = new Vector3(PlayerPrefs.GetFloat("LPScaleX", 1), PlayerPrefs.GetFloat("LPScaleY", 1), 0);

        HP.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("HPX", -174.10f), PlayerPrefs.GetFloat("HPY", 54.90f), 0); //(891.47, 55.80)
        HP.transform.localScale = new Vector3(PlayerPrefs.GetFloat("HPScaleX", 1), PlayerPrefs.GetFloat("HPScaleY", 1), 0);


        HK.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("HKX", -58.90f), PlayerPrefs.GetFloat("HKY", 54.90f), 0);//(1006.07, 55.73)
        HK.transform.localScale = new Vector3(PlayerPrefs.GetFloat("HKScaleX", 1), PlayerPrefs.GetFloat("HKScaleY", 1), 0);

        SP.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("SPX", -48.30f), PlayerPrefs.GetFloat("SPY", 225.30f), 0);//(-49.80, -255.70)
        SP.transform.localScale = new Vector3(PlayerPrefs.GetFloat("SPScaleX", 1), PlayerPrefs.GetFloat("SPScaleY", 1), 0);

        B.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("BX", -173.20f), PlayerPrefs.GetFloat("BY", 225.40f), 0);//(893.47, 225.80)
        B.transform.localScale = new Vector3(PlayerPrefs.GetFloat("BScaleX", 1), PlayerPrefs.GetFloat("BScaleY", 1), 0);

       
        BDCanvasButtonsHandler.inst.joystick.anchoredPosition = new Vector3(joyStick.anchoredPosition.x, joyStick.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.joystick.transform.localScale = new Vector3(joyStick.transform.localScale.x, joyStick.transform.localScale.y, 0);

        BDCanvasButtonsHandler.inst.LK.anchoredPosition = new Vector3(LK.anchoredPosition.x, LK.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.LK.transform.localScale = new Vector3(LK.transform.localScale.x, LK.transform.localScale.y, 0);


        BDCanvasButtonsHandler.inst.LP.anchoredPosition = new Vector3(LP.anchoredPosition.x, LP.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.LP.transform.localScale = new Vector3(LP.transform.localScale.x, LP.transform.localScale.y, 0);

        BDCanvasButtonsHandler.inst.HP.anchoredPosition = new Vector3(HP.anchoredPosition.x, HP.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.HP.transform.localScale = new Vector3(HP.transform.localScale.x, HP.transform.localScale.y, 0);

        BDCanvasButtonsHandler.inst.HK.anchoredPosition = new Vector3(HK.anchoredPosition.x, HK.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.HK.transform.localScale = new Vector3(HK.transform.localScale.x, HK.transform.localScale.y, 0);

        BDCanvasButtonsHandler.inst.SP.anchoredPosition = new Vector3(SP.anchoredPosition.x, SP.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.SP.transform.localScale = new Vector3(SP.transform.localScale.x, SP.transform.localScale.y, 0);

        BDCanvasButtonsHandler.inst.B.anchoredPosition = new Vector3(B.anchoredPosition.x, B.anchoredPosition.y, 0);
        BDCanvasButtonsHandler.inst.B.transform.localScale = new Vector3(B.transform.localScale.x, B.transform.localScale.y, 0);
    }

    
    public void ResetLayout()
    {
        
        PlayerPrefs.DeleteKey("joyX");
        PlayerPrefs.DeleteKey("joyScaleX");
        PlayerPrefs.DeleteKey("joyY");
        PlayerPrefs.DeleteKey("joyScaleY");
        PlayerPrefs.DeleteKey("LKX");
        PlayerPrefs.DeleteKey("LKScaleX");
        PlayerPrefs.DeleteKey("LKY");
        PlayerPrefs.DeleteKey("LKScaleY");
        PlayerPrefs.DeleteKey("LPX");
        PlayerPrefs.DeleteKey("LPScaleX");
        PlayerPrefs.DeleteKey("LPY");
        PlayerPrefs.DeleteKey("LPScaleY");
        PlayerPrefs.DeleteKey("HPX");
        PlayerPrefs.DeleteKey("HPScaleX");
        PlayerPrefs.DeleteKey("HPY");
        PlayerPrefs.DeleteKey("HPScaleY");
        PlayerPrefs.DeleteKey("HKX");
        PlayerPrefs.DeleteKey("HKScaleX");
        PlayerPrefs.DeleteKey("HKY");
        PlayerPrefs.DeleteKey("HKScaleY");  
        PlayerPrefs.DeleteKey("SPX");
        PlayerPrefs.DeleteKey("SPScaleX");
        PlayerPrefs.DeleteKey("SPY");
        PlayerPrefs.DeleteKey("SPScaleY");
        PlayerPrefs.DeleteKey("BX");
        PlayerPrefs.DeleteKey("BScaleX");
        PlayerPrefs.DeleteKey("BY");
        PlayerPrefs.DeleteKey("BScaleY");
        PlayerPrefs.Save();


        joyStick.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("joyX", 149f), PlayerPrefs.GetFloat("joyY", 75.90f), 0); //(149.30, 76.38)
        joyStick.transform.localScale = new Vector3(PlayerPrefs.GetFloat("joyScaleX", 1), PlayerPrefs.GetFloat("joyScaleY", 1), 0);

        LK.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("LKX", -116.40f), PlayerPrefs.GetFloat("LKY", 144.90f), 0); //LK : (950.07, 145.80)
        LK.transform.localScale = new Vector3(PlayerPrefs.GetFloat("LKScaleX", 1), PlayerPrefs.GetFloat("LKScaleY", 1), 0);

        LP.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("LPX", -231.70f), PlayerPrefs.GetFloat("LPY", 144.90f), 0); //LP : (834.77, 145.80)
        LP.transform.localScale = new Vector3(PlayerPrefs.GetFloat("LPScaleX", 1), PlayerPrefs.GetFloat("LPScaleY", 1), 0);

        HP.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("HPX", -174.10f), PlayerPrefs.GetFloat("HPY", 54.90f), 0); //(891.47, 55.80)
        HP.transform.localScale = new Vector3(PlayerPrefs.GetFloat("HPScaleX", 1), PlayerPrefs.GetFloat("HPScaleY", 1), 0);


        HK.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("HKX", -58.90f), PlayerPrefs.GetFloat("HKY", 54.90f), 0);//(1006.07, 55.73)
        HK.transform.localScale = new Vector3(PlayerPrefs.GetFloat("HKScaleX", 1), PlayerPrefs.GetFloat("HKScaleY", 1), 0);

        SP.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("SPX", -48.30f), PlayerPrefs.GetFloat("SPY", 225.30f), 0);//(-49.80, -255.70)
        SP.transform.localScale = new Vector3(PlayerPrefs.GetFloat("SPScaleX", 1), PlayerPrefs.GetFloat("SPScaleY", 1), 0);

        B.anchoredPosition = new Vector3(PlayerPrefs.GetFloat("BX", -173.20f), PlayerPrefs.GetFloat("BY", 225.40f), 0);//(893.47, 225.80)
        B.transform.localScale = new Vector3(PlayerPrefs.GetFloat("BScaleX", 1), PlayerPrefs.GetFloat("BScaleY", 1), 0);

        SetToInitials();
        PlayerPrefs.SetInt("icons", 0);
        PlayerPrefs.SetInt("ControlsSet", 0);
      //  ApplyNewSettings();
    }


    public void ResizeUI(float size)
    {
        if (currentSelectedObject != null)
        {
            currentSelectedObject.transform.localScale = new Vector3(1f + size, 1f + size, 0);
        }
    }
    #endregion

}
