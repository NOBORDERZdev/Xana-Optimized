using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using static GlobalConstants;
using PMY;
using static PMY.PMY_VideoAndImage;

/// <summary>
/// This class handles basic link color behavior, supports also underline (static only)
/// Does not support strike-through, but can be easily implemented in the same way as the underline
/// </summary>
[DisallowMultipleComponent()]
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProUGUIHyperlinks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum EnvType { JJWorld, PMYWorld }
    public EnvType envType;
    [SerializeField]
    private Color32 hoveredColor = new Color32(0x00, 0x59, 0xFF, 0xFF);
    [SerializeField]
    private Color32 pressedColor = new Color32(0x00, 0x00, 0xB7, 0xFF);
    [SerializeField]
    private Color32 usedColor = new Color32(0xFF, 0x00, 0xFF, 0xFF);
    [SerializeField]
    private Color32 usedHoveredColor = new Color32(0xFD, 0x5E, 0xFD, 0xFF);
    [SerializeField]
    private Color32 usedPressedColor = new Color32(0xCF, 0x00, 0xCF, 0xFF);

    private List<Color32[]> startColors = new List<Color32[]>();
    private TextMeshProUGUI textMeshPro;
    private Dictionary<int, bool> usedLinks = new Dictionary<int, bool>();
    private int hoveredLinkIndex = -1;
    private int pressedLinkIndex = -1;
    private Camera mainCamera;
    private static bool uniqueClick = true;



    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        mainCamera = Camera.main;
        if (textMeshPro.canvas.renderMode == RenderMode.ScreenSpaceOverlay) mainCamera = null;
        else if (textMeshPro.canvas.worldCamera != null) mainCamera = textMeshPro.canvas.worldCamera;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        int linkIndex = GetLinkIndex();
        if (linkIndex != -1) // Was pointer intersecting a link?
        {
            pressedLinkIndex = linkIndex;
            //if (usedLinks.TryGetValue(linkIndex, out bool isUsed) && isUsed) // Has the link been already used?
            //{
            //    // Have we hovered before we pressed? Touch input will first press, then hover
            //    if (pressedLinkIndex != hoveredLinkIndex) startColors = SetLinkColor(linkIndex, usedPressedColor);
            //    else SetLinkColor(linkIndex, usedPressedColor);
            //}
            //else
            //{
            //    // Have we hovered before we pressed? Touch input will first press, then hover
            //    if (pressedLinkIndex != hoveredLinkIndex) startColors = SetLinkColor(linkIndex, pressedColor);
            //    else SetLinkColor(linkIndex, pressedColor);
            //}
            hoveredLinkIndex = pressedLinkIndex; // Changes flow in LateUpdate
        }
        else pressedLinkIndex = -1;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        int linkIndex = GetLinkIndex();
        if (linkIndex != -1 && linkIndex == pressedLinkIndex) // Was pointer intersecting the same link as OnPointerDown?
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            //SetLinkColor(linkIndex, usedHoveredColor);
            startColors.ForEach(c => c[0] = c[1] = c[2] = c[3] = usedColor);
            usedLinks[linkIndex] = true;

            // For Analatics URL Clicked = true;
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, true, false);
            CallFirebaseEventForLinkClicked();
            Application.OpenURL(linkInfo.GetLinkID());
        }
        pressedLinkIndex = -1;



    }

    void CallFirebaseEventForLinkClicked()
    {
        if (envType.Equals(EnvType.JJWorld))
        {
            string eventName = XanaConstants.xanaConstants.EnviornmentName;

            if (XanaConstants.xanaConstants.EnviornmentName.Contains("ZONE-X"))
            {
                // we don't have this museum yet
                ////worldName = "1F_Mainloby_NFTclick";
            }
            else if (XanaConstants.xanaConstants.EnviornmentName.Contains("ZONE X Musuem"))
            {
                eventName = FirebaseTrigger.URL_ZoneX.ToString() + "_" + (JjInfoManager.Instance.clickedNftInd + 1);
            }
            else if (XanaConstants.xanaConstants.EnviornmentName.Contains("FIVE ELEMENTS"))
            {
                eventName = FirebaseTrigger.URL_FiveElements.ToString() + "_" + (JjInfoManager.Instance.clickedNftInd + 1);
            }
            else
            {
                if (JjInfoManager.Instance.roomName.Equals(JJVideoAndImage.MuseumType.AtomMuseum.ToString()))
                    eventName = FirebaseTrigger.URL_AtomRoom.ToString() + JjInfoManager.Instance.clRoomId + "_" + (JjInfoManager.Instance.clickedNftInd + 1);
                else if (JjInfoManager.Instance.roomName.Equals(JJVideoAndImage.MuseumType.RentalSpace.ToString()))
                    eventName = FirebaseTrigger.URL_AtomRental.ToString() + JjInfoManager.Instance.clRoomId + "_" + (JjInfoManager.Instance.clickedNftInd + 1);
            }

            if (JjInfoManager.Instance.clRoomId != 0)
                SendFirebaseEvent(eventName);
        }
        else if (envType.Equals(EnvType.PMYWorld))
        {
            string eventName = XanaConstants.xanaConstants.EnviornmentName;
            switch (eventName)
            {
                case "PMY ACADEMY":
                    eventName = FirebaseTrigger.URL_PMYLobby.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                    break;
                case "PMYGallery":
                    eventName = FirebaseTrigger.URL_Gallery.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                    break;
                case "PMYRoomA":
                    {
                        switch (PMY_Nft_Manager.Instance.PMY_RoomId)
                        {
                            case 8:
                                eventName = FirebaseTrigger.URL_CRoom1.ToString() + "_" +(PMY_Nft_Manager.Instance.clickedNftInd + 1);
                                break;
                            case 9:
                                eventName = FirebaseTrigger.URL_CRoom2.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                                break;
                            case 10:
                                eventName = FirebaseTrigger.URL_CRoom3.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                                break;
                            case 11:
                                eventName = FirebaseTrigger.URL_CRoom4.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                                break;
                            case 12:
                                eventName = FirebaseTrigger.URL_CRoom5.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                                break;
                            case 13:
                                eventName = FirebaseTrigger.URL_CRoom6.ToString() + "_" + (PMY_Nft_Manager.Instance.clickedNftInd + 1);
                                break;
                        }
                        break;
                    }
            }
            SendFirebaseEvent(eventName);
        }
    }

    private int GetLinkIndex()
    {
        return TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, mainCamera);
    }

}