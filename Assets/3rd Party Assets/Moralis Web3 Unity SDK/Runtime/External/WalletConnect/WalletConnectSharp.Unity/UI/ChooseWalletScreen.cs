using Assets.Scripts;
using Assets.Scripts.WalletConnectSharp.Unity.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

namespace WalletConnectSharp.Unity.UI
{
    public class ChooseWalletScreen : MonoBehaviour
    {
        public WalletConnect WalletConnect;
        public Transform buttonGridTransform;
        public Text loadingText;
        public Text statusText;
        
        [SerializeField]
        public WalletSelectItem[] wallets;

        private bool walletButtonsCreated = false;
        private RectTransform TextRectTransform;
        private RectTransform ParentImage2RectTransform;
        private Image ParentImage;
        private Image ParentImage2;
        private Image ParentImage3;
        private RectTransform ChildTransform;
        private RectTransform ChildTransform2_Coinhub;  
         private void Start()
        {
            TextRectTransform = buttonGridTransform.GetChild(0).GetChild(0).transform.GetComponent<RectTransform>();
            ParentImage2RectTransform = buttonGridTransform.GetChild(0).GetChild(1).transform.GetComponent<RectTransform>();
            ChildTransform = buttonGridTransform.GetChild(0).GetChild(1).GetChild(0).transform.GetComponent<RectTransform>();
            ChildTransform2_Coinhub = buttonGridTransform.GetChild(0).GetChild(1).GetChild(1).transform.GetComponent<RectTransform>();    
            ParentImage = buttonGridTransform.GetChild(0).GetComponent<Image>();   
            ParentImage2 = buttonGridTransform.GetChild(0).GetChild(1).GetComponent<Image>();              
            ParentImage3 = buttonGridTransform.GetChild(0).GetChild(2).GetComponent<Image>();                    
#if UNITY_IOS
            // Set wallet filter to those wallets selected by the developer.
            IEnumerable<string> walletFilter = from w in wallets
                                               where w.Selected == true
                                               select w.Name;
            // For iOS Set wallet filter to speed up wallet button display.
            if (walletFilter.Count() > 0)
            {
                WalletConnect.AllowedWalletNames = walletFilter.ToList();
            }
            else
            {
                Debug.Log("No wallets selected for filter.");
            }
#endif
            //StartCoroutine(BuildWalletButtons());
            //BuildWalletButtons();
        }

        private void Update()
        {
            if (!walletButtonsCreated && WalletConnect.SupportedWallets != null && WalletConnect.SupportedWallets.Count > 1)
            {
                walletButtonsCreated = true;
                StartCoroutine(BuildWalletButtons());
            }
        }

        private IEnumerator BuildWalletButtons()
        {
            yield return WalletConnect.FetchWalletList();   

            Debug.Log("Building wallet buttons.");
            GameObject buttonPrefabParent = new GameObject("buttonPrefab1");
            buttonPrefabParent.AddComponent<Image>();
            //buttonPrefabParent.AddComponent<Mask>();
            buttonPrefabParent.GetComponent<Image>().sprite = ParentImage.sprite;
            //buttonPrefabParent.GetComponent<Image>().color = ParentImage.color;

            GameObject PrefabParent2 = new GameObject("Prefab2");
            PrefabParent2.AddComponent<Image>();
            PrefabParent2.AddComponent<Mask>();
            PrefabParent2.GetComponent<Image>().sprite = ParentImage2.sprite;
            GameObject PrefabParent3 = new GameObject("Prefab2");
            PrefabParent3.AddComponent<Image>();
            PrefabParent3.AddComponent<Mask>();
            PrefabParent3.GetComponent<Image>().sprite = ParentImage3.sprite;
 


            GameObject buttonPrefab = new GameObject("buttonPrefab");
            buttonPrefab.AddComponent<Image>();
            buttonPrefab.AddComponent<Button>();
            GameObject TextPrefab = new GameObject("Textprefab");
            TextPrefab.AddComponent<Text>();
            foreach (var walletId in WalletConnect.SupportedWallets.Keys)
            {
                var walletData = WalletConnect.SupportedWallets[walletId];
                var walletObjBtnParent = Instantiate(buttonPrefabParent, buttonGridTransform);
                var walletParent2 = new GameObject();
                 if (walletData.name =="SafePal")
                {    
                     print (walletData.name);
                     walletParent2 = Instantiate(PrefabParent3, walletObjBtnParent.transform);
                 }
                 else
                {
                     walletParent2 = Instantiate(PrefabParent2, walletObjBtnParent.transform);
                }     
                walletParent2.GetComponent<RectTransform>().anchorMin = ParentImage2RectTransform.anchorMin;
                walletParent2.GetComponent<RectTransform>().anchorMax = ParentImage2RectTransform.anchorMax;
                walletParent2.GetComponent<RectTransform>().anchoredPosition = ParentImage2RectTransform.anchoredPosition;
                walletParent2.GetComponent<RectTransform>().sizeDelta = ParentImage2RectTransform.sizeDelta;


                var walletObj = Instantiate(buttonPrefab, walletParent2.transform);  
                var walletImage = walletObj.GetComponent<Image>();
                var walletButton = walletObj.GetComponent<Button>();

                var wallettextpre = Instantiate(TextPrefab, walletObjBtnParent.transform);
                wallettextpre.GetComponent<Text>().text = walletData.name;
                wallettextpre.GetComponent<Text>().font = statusText.font;
                if (walletData.name == "Coinhub")
                {
                    walletObj.GetComponent<RectTransform>().anchorMin = ChildTransform2_Coinhub.anchorMin;
                    walletObj.GetComponent<RectTransform>().anchorMax = ChildTransform2_Coinhub.anchorMax;
                    walletObj.GetComponent<RectTransform>().anchoredPosition = ChildTransform2_Coinhub.anchoredPosition;
                    walletObj.GetComponent<RectTransform>().sizeDelta = ChildTransform2_Coinhub.sizeDelta;
                 }   
                else   
                {
                    walletObj.GetComponent<RectTransform>().anchorMin = ChildTransform.anchorMin;
                    walletObj.GetComponent<RectTransform>().anchorMax = ChildTransform.anchorMax;
                    walletObj.GetComponent<RectTransform>().anchoredPosition = ChildTransform.anchoredPosition;
                    walletObj.GetComponent<RectTransform>().sizeDelta = ChildTransform.sizeDelta;
 
                }

                wallettextpre.GetComponent<RectTransform>().anchorMin = TextRectTransform.anchorMin;
                wallettextpre.GetComponent<RectTransform>().anchorMax = TextRectTransform.anchorMax;
                wallettextpre.GetComponent<RectTransform>().anchoredPosition = TextRectTransform.anchoredPosition;
                wallettextpre.GetComponent<RectTransform>().sizeDelta = TextRectTransform.sizeDelta;

                wallettextpre.GetComponent<Text>().color = statusText.color;
                wallettextpre.GetComponent<Text>().alignment = statusText.alignment;
                wallettextpre.GetComponent<Text>().resizeTextForBestFit = false;
                wallettextpre.GetComponent<Text>().fontSize = (statusText.fontSize-3);        
                wallettextpre.GetComponent<Text>().fontStyle = statusText.fontStyle;      

                walletImage.sprite = walletData.medimumIcon;
                walletButton.onClick.AddListener(delegate
                {
                    // Check if WalletConnect is ready for the user prompt to make sure the user wont get stuck
                    if (WalletConnect.Session.ReadyForUserPrompt)
                    {
                        WalletConnect.OpenDeepLink(walletData);
                        // hide wallets after 
                        gameObject.SetActive(false);
                        // show status text
                        statusText.gameObject.SetActive(true);
                    }
                });
            }

            Destroy(loadingText.gameObject);
        }

        public static List<WalletSelectItem> GetWalletNameList()
        {
            List<WalletSelectItem> result = SupportedWalletList.SupportedWalletNames();

            return result;
        }
    }
}