using UnityEngine;
using UnityEngine.InputSystem;

public class VR_UImanager : MonoBehaviour
{
    public static VR_UImanager instance;
    public GameObject footer_UI;
    public GameObject spaceScreen_UI;
    public InputActionReference button_B;
    public InputActionReference button_A;
    private bool isKeyboardCanvasActive = false;
    private bool isWorldPopupActive = false;
    public GameObject world_PopUP;
    public GameObject keyboardCanvas;

    private const string FooterUIKey = "FooterUIEnabled";
    private const string SpaceScreenUIKey = "SpaceScreenUIEnabled";
    public GameObject splashScreen;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bool isFooterUIEnabled = PlayerPrefs.GetInt(FooterUIKey, 0) == 1;
        bool isSpaceScreenUIEnabled = PlayerPrefs.GetInt(SpaceScreenUIKey, 0) == 1;

        footer_UI.SetActive(isFooterUIEnabled);
        spaceScreen_UI.SetActive(isSpaceScreenUIEnabled);

        
    }

    public void Start()
    {
        keyboardCanvas = KeyboardManager.instance.gameObject;
        if (keyboardCanvas == null)
        {
            Debug.LogError("KeyboardCanvas GameObject not found. Make sure it is named correctly.");
        }
    }

    public void EnableUIElements()
    {
        footer_UI.SetActive(true);
        spaceScreen_UI.SetActive(true);

        SaveUIState();
    }

    private void SaveUIState()
    {
        PlayerPrefs.SetInt(FooterUIKey, footer_UI.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt(SpaceScreenUIKey, spaceScreen_UI.activeSelf ? 1 : 0);
        PlayerPrefs.Save();
    }



    private void OnEnable()
    {
        button_B.action.Enable();
        button_A.action.Enable();
    }

    private void OnDisable()
    {
        button_B.action.Disable();
        button_A.action.Disable();
    }

    void Update()
    {
        if (button_B.action.triggered)
        {
            ToggleCanvas();
        }
        else if(button_A.action.triggered)
        {
            PopUp_Canvas();
        }
    }

    private void ToggleCanvas()
    {
        if (keyboardCanvas != null)
        {
            isKeyboardCanvasActive = !isKeyboardCanvasActive;
            keyboardCanvas.SetActive(isKeyboardCanvasActive);
        }
        else
        {
            Debug.LogWarning("keyboardCanvas is not assigned.");
        }
    }

    public void PopUp_Canvas()
    {
        if (world_PopUP != null)
        {
            world_PopUP.SetActive(false);
            isWorldPopupActive = false;
        }
    }
}
