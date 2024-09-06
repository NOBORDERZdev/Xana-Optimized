using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace XanaAi
{
    public class AiController : MonoBehaviour
    {
        GameObject wornShirt, wornPant, wornHair, wornShose, wornEyewearable, wornChain, wornGloves;
        [SerializeField] TMP_Text NameTxt;
        List<Texture> masks = new List<Texture>();
        private Stitcher stitcher;
        [SerializeField] WanderingAI wandering;
        [SerializeField]AiSelfie selfie;
        [SerializeField]AiEmote aiEmote;
        [SerializeField] AiReaction aiReaction;
        [SerializeField] AIJump aIJump;
        [SerializeField] AiFreeCam freeCam;
        //[SerializeField] EyesBlinking blinking;
        bool isNewlySpwaned =true;
        Coroutine emoteCoroutine;
        [HideInInspector]
        public Coroutine ActionCoroutine=null;
        [HideInInspector]
        public bool isPerformingAction= false;
        private void Awake()
        {
            stitcher = new Stitcher();
            
        }

        private void Start()
        {
            //blinking.StoreBlendShapeValues();          // enabling blinking
            //blinking.isBlinking = true;
            //StartCoroutine(blinking.BlinkingStartRoutine()); 
        }

        /// <summary>
        /// To Unstich item from the AI body
        /// </summary>
        /// <param name="type"></param>
        public void UnStichItem(string type)
        {
            switch (type)
            {
                case "Chest":
                    Destroy(wornShirt);
                    break;
                case "Legs":
                    Destroy(wornPant);
                    break;
                case "Hair":
                    Destroy(wornHair);
                    break;
                case "Feet":
                    Destroy(wornShose);
                    break;
                case "EyeWearable":
                    Destroy(wornEyewearable);
                    break;
                case "Chain":
                    Destroy(wornChain);
                    break;
                case "Glove":
                    Destroy(wornGloves);
                    break;
            }
        }

        /// <summary>
        /// To stich item on AI rig
        /// </summary>
        /// <param name="item">Cloth to wear</param>
        /// <param name="applyOn">AI that are going to wear the dress</param>
        public void StichItem(int itemId, GameObject item, string type, GameObject applyOn, bool applyHairColor = true)
        {
            CharacterBodyParts tempBodyParts = applyOn.gameObject.GetComponent<CharacterBodyParts>();
            UnStichItem(type);
            if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().texture != null)
            {
                Texture tempTex = item.GetComponent<EffectedParts>().texture;
                masks.Add(tempTex);
                tempBodyParts.ApplyMaskTexture(type, tempTex, this.gameObject);
            }

            if (item.GetComponent<EffectedParts>() && item.GetComponent<EffectedParts>().variation_Texture != null)
            {
                item.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetTexture("_BaseMap", item.GetComponent<EffectedParts>().variation_Texture);
            }

            item = this.stitcher.Stitch(item, applyOn);
            if (type == "Hair")
            {
                StartCoroutine(tempBodyParts.ImplementColors(Color.black /*_CharacterData.HairColor*/, SliderType.HairColor, applyOn));
            }
            item.layer = 11;
            switch (type)
            {
                case "Chest":
                    wornShirt = item;
                    break;
                case "Legs":
                    wornPant = item;
                    break;
                case "Hair":
                    wornHair = item;
                    break;
                case "Feet":
                    wornShose = item;
                    break;
                case "EyeWearable":
                    wornEyewearable = item;
                    break;
                case "Chain":
                    wornChain = item;
                    break;
                case "Glove":
                    wornGloves = item;
                    break;
            }
            if (item.name.Contains("arabic"))
            {
                // Disable Pant
                if (wornPant)
                    wornPant.SetActive(false);

                // Disable Hair
                if (wornHair)
                    wornHair.SetActive(false);
            }
            else if (wornShirt && (wornShirt.name.Contains("arabic") || wornShirt.name.Contains("Arabic")))
            {
                // Yes Arabic Wear , new pant or hair disable
                if (wornPant)
                    wornPant.SetActive(false);

                if (wornHair)
                    wornHair.SetActive(false);
            }
            else
            {
                if (wornPant)
                    wornPant.SetActive(true);
                if (wornHair)
                    wornHair.SetActive(true);
                if (wornChain)
                    wornChain.SetActive(true);
            }

        }


        /// <summary>
        /// To Perform Action Randomly
        /// </summary>
        /// <returns></returns>
        public IEnumerator PerformAction() {
            if (!isPerformingAction)
            {
                isPerformingAction= true;
                //print("PerformAction call");
                yield return new WaitForSeconds(/*Random.Range(1,2)*/0);
                int rand;
                if (isNewlySpwaned)
                {
                   // print("in newly to wander");
                    rand=0;
                    isNewlySpwaned = false;
                }
                else
                {
                    rand = Random.Range(0, 5);
                    print("get random action : " + rand);
                }
           

                switch (rand)
                {
                    case 0:
                       // print("Performing Wander");
                        wandering.Wander();
                        selfie.ForceFullyDisableSelfie();
                        if(emoteCoroutine != null)
                            StopCoroutine(emoteCoroutine);
                        aiEmote.ForceFullyStopEmote();
                        break;
                    case 1:
                        //print("Performing Selfie action");
                        selfie.SelfieAction();
                        if(emoteCoroutine != null)
                            StopCoroutine(emoteCoroutine);
                        aiEmote.ForceFullyStopEmote();
                        break;
                    case 2:
                        // print("Performing Emote");
                       if(emoteCoroutine != null)
                            StopCoroutine(emoteCoroutine);
                        emoteCoroutine= StartCoroutine(aiEmote.PlayEmote());
                        selfie.ForceFullyDisableSelfie();
                        break;
                    case 3:
                       // print("Performing Jump");
                        aIJump.AiJump();
                        if(emoteCoroutine != null)
                            StopCoroutine(emoteCoroutine);
                        aiEmote.ForceFullyStopEmote();
                        break;
                    case 4:
                        //print("Performing Ai free cam");
                        freeCam.PerformFreeCam();
                        if(emoteCoroutine != null)
                            StopCoroutine(emoteCoroutine);
                        aiEmote.ForceFullyStopEmote();
                        break;
                    default:
                        //print("Performing Wander from default");
                        wandering.Wander();
                        selfie.ForceFullyDisableSelfie();
                       if(emoteCoroutine != null)
                            StopCoroutine(emoteCoroutine);
                        aiEmote.ForceFullyStopEmote();
                        break;
                }
            }
        }


        /// <summary>
        /// To Set Ai name
        /// </summary>
        /// <param name="name"></param>
        public void SetAiName(string name){ 
            NameTxt.text = name;
        }


    }
}