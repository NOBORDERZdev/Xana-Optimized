using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

enum GameLanguage
{
    English,
    Japanese
}

public class UIController : MonoBehaviour
{
    //UI Preset
    [SerializeField] private GameObject pointerUI;
    [SerializeField] private Transform inGameUI;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<Sprite> teleportImages;
    private List<Sprite> teleportImages_lang;
    [SerializeField] private List<Sprite> teleportBackgroundImages;
    [SerializeField] List<Sprite> infoImages;
    private List<Sprite> infos;
    [SerializeField] private List<Sprite> photozoneImages;
    [SerializeField] private List<Sprite> telescopeImages;
    [SerializeField] List<Sprite> buttonImages;
    [SerializeField] List<Sprite> skyImages;
    // UI Configs
    [SerializeField]
    private Transform informationUI;
    [SerializeField]
    private Transform photoZoneUI;
    [SerializeField]
    private Transform telescopeUI;
    [SerializeField]
    private Transform skyImageUI;
    [SerializeField]
    private Transform teleportationUI;
    [SerializeField] private Transform pointUI;
    private Dictionary<string, Transform> teleportPoints;
    private Dictionary<string, GameObject> inGameUIs;
    private Dictionary<string, Transform> spawnPoints;
    //Values
    private int infoNum = 0;
    private int skyNum = 0;
    //Object Configs
    private GameObject Player
    {
        get
        {
            return ReferencesForGamePlay.instance.MainPlayerParent;
        }
    }
    private GameLanguage gameLanguage;
    private string lang = "";

    private void Awake()
    {
        Enable_DisableObjects.Instance.DisableScreenRotaionButton();
        ReferencesForGamePlay.instance.minimap.SetActive(false);

        teleportPoints = new Dictionary<string, Transform>();
        foreach (Transform child in teleportPoint)
        {
            teleportPoints.Add(child.name, child);
        }

        inGameUIs = new Dictionary<string, GameObject>();
        foreach (Transform child in inGameUI)
        {
            inGameUIs.Add(child.name, child.gameObject);
        }

        spawnPoints = new Dictionary<string, Transform>();
        foreach (Transform child in spawnPoint)
        {
            spawnPoints.Add(child.name, child);
        }

        if (Application.systemLanguage == SystemLanguage.English) gameLanguage = GameLanguage.English;
        else if (Application.systemLanguage == SystemLanguage.Japanese) gameLanguage = GameLanguage.Japanese;
        else gameLanguage = GameLanguage.Japanese;
    }

    void Start()
    {
        teleportImages_lang = new List<Sprite>();
        infos = new List<Sprite>();

        if (gameLanguage == GameLanguage.English)
        {
            lang = "Eng";
        }
        else
        {
            lang = "Jpn";
        }

        for (int i = 0; i < teleportImages.Count; i++)
        {
            if (teleportImages[i].name.Contains(lang))
            {
                teleportImages_lang.Add(teleportImages[i]);
            }
        }

        for (int i = 0; i < infoImages.Count; i++)
        {
            if (infoImages[i].name.Contains(lang))
            {
                infos.Add(infoImages[i]);
            }
        }
        teleportationUI.GetComponent<Image>().sprite = teleportBackgroundImages[lang == "Eng" ? 0 : 1];
        informationUI.GetComponent<Image>().sprite = infos[0];
        photoZoneUI.GetComponent<Image>().sprite = photozoneImages[lang == "Eng" ? 0 : 1];
        telescopeUI.GetComponent<Image>().sprite = telescopeImages[lang == "Eng" ? 0 : 1];

        informationUI.GetComponentInChildren<Button>().onClick.AddListener(onInfoNextClick);
        photoZoneUI.GetComponentInChildren<Button>().onClick.AddListener(onPhotoClick);
        telescopeUI.GetComponentInChildren<Button>().onClick.AddListener(onTelescopeClick);
        teleportationUI.GetChild(0).GetComponent<Button>().onClick.AddListener(closeTeleportation);

        Button[] pointerButton = pointerUI.GetComponentsInChildren<Button>();
        for (int i = 0; i < pointerButton.Length; i++)
        {
            int index = i;
            pointerButton[i].onClick.AddListener(() => openPointerUI(pointerButton[index]));
            pointerButton[i].gameObject.SetActive(false);
        }

        Button[] skyBtnArrays = skyImageUI.GetComponentsInChildren<Button>();
        for (int i = 0; i < skyBtnArrays.Length; i++)
        {
            int index = i;
            skyBtnArrays[i].onClick.AddListener(() => openSkyUI(skyBtnArrays[index]));
        }

        Button[] teleportImgButtons = teleportationUI.GetChild(1).GetComponentsInChildren<Button>();
        for (int i = 0; i < teleportImgButtons.Length; i++)
        {
            int index = i;
            teleportImgButtons[i].onClick.AddListener(() => teleportPlayerToSpawnPoint(teleportImgButtons[index].gameObject));
        }

        //Player = GameObject.FindGameObjectWithTag("PhotonLocalPlayer");
    }

    private void onInfoNextClick()
    {
        infoNum++;
        if (infoNum > infos.Count - 1)
        {
            infoNum = 0;
            informationUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[buttonImages.FindIndex(sprite => sprite.name == "Next")];
            informationUI.gameObject.SetActive(false);
        }
        else if (infoNum == infos.Count - 1)
        {
            informationUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[buttonImages.FindIndex(sprite => sprite.name == "Close")];
        }
        informationUI.GetComponent<Image>().sprite = infos[Math.Clamp(infoNum, 0, infoImages.Count - 1)];
    }

    private void onPhotoClick()
    {
        photoZoneUI.gameObject.SetActive(false);
    }
    private void renderTeleportUI(string triggerName)
    {
        for (int i = 0; i < teleportImages_lang.Count; i++)
        {
            if (teleportImages_lang[i].name.Split("_")[1] != triggerName.Substring(0, triggerName.IndexOf("_Teleport")))
            {
                Button[] imageButtons = teleportationUI.GetChild(1).GetComponentsInChildren<Button>();
                for (int j = 0; j < imageButtons.Length; j++)
                {
                    if (imageButtons[j].GetComponent<RawImage>().texture == null)
                    {
                        imageButtons[j].GetComponent<RawImage>().texture = teleportImages_lang[i].texture;
                        break;
                    }
                }
            }
        }

    }
    private void openPointerUI(Button interactBtn)
    {
        if (interactBtn.name.Contains("_Teleport"))
        {
            //player.GetComponent<vThirdPersonInput>().enabled = false;
            GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;

            Player.transform.position = teleportPoints[interactBtn.name].position;
            Player.transform.rotation = teleportPoints[interactBtn.name].rotation;
            teleportationUI.gameObject.SetActive(true);
            renderTeleportUI(interactBtn.name);
        }
        else
        {
            string interactBtnName = Regex.Match(interactBtn.name, @"^(\D+)").Groups[1].Value;
            inGameUIs[interactBtnName].SetActive(true);
        }
        interactBtn.gameObject.SetActive(false);
    }
    private void onTelescopeClick()
    {
        telescopeUI.gameObject.SetActive(false);
        skyImageUI.gameObject.SetActive(true);
        GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;

        renderSkyImage(skyNum);
    }
    private void renderSkyImage(int index)
    {
        skyImageUI.GetComponent<Image>().sprite = skyImages[index];
        if (index == 3)
        {
            skyImageUI.GetChild(2).gameObject.SetActive(false);
        }
        else if (index == 0)
        {
            skyImageUI.GetChild(1).gameObject.SetActive(false);
            if (!skyImageUI.GetChild(2).gameObject.activeSelf)
            {
                skyImageUI.GetChild(2).gameObject.SetActive(true);
            }
        }
        else
        {
            if (!skyImageUI.GetChild(1).gameObject.activeSelf)
            {
                skyImageUI.GetChild(1).gameObject.SetActive(true);
            }
            if (!skyImageUI.GetChild(2).gameObject.activeSelf)
            {
                skyImageUI.GetChild(2).gameObject.SetActive(true);
            }
        }
    }
    private void openSkyUI(Button interactBtn)
    {
        if (interactBtn.name == "Close")
        {
            skyImageUI.gameObject.SetActive(false);
            GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = true;

            skyNum = 0;
        }
        else
        {
            if (interactBtn.name == "Next")
            {
                Math.Clamp(skyNum++, 0, 3);
            }
            else if (interactBtn.name == "Prev")
            {
                Math.Clamp(skyNum--, 0, 3);
            }
            renderSkyImage(skyNum);
        }
    }
    private void closeTeleportation()
    {
        teleportationUI.gameObject.SetActive(false);

        Button[] imageButtons = teleportationUI.GetChild(1).GetComponentsInChildren<Button>();
        for (int i = 0; i < imageButtons.Length; i++)
        {
            imageButtons[i].GetComponent<RawImage>().texture = null;
        }
        GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = true;

        //player.GetComponent<vThirdPersonInput>().enabled = true;
    }

    private void teleportPlayerToSpawnPoint(GameObject imgButton)
    {
        string placeName = imgButton.GetComponent<RawImage>().texture.name.Split("_")[1];
        Player.transform.position = spawnPoints[placeName].position;
        Player.transform.rotation = spawnPoints[placeName].rotation;
        Invoke("closeTeleportation", 0.1f);
    }
    public void SetPointUI(string point)
    {
        pointUI.GetChild(0).GetComponent<Text>().text = point;
    }
}
