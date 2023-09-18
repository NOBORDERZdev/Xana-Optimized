using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

namespace XanaAi
{
    public class AiAppearance : MonoBehaviour
    {
        //public Toggle wearableToggle;
        //public bool isWearableOrNot;

        [Header("Wearable items")]
        [SerializeField] public List<string> Uppers;
        [SerializeField] public List<string> Lower;
        [SerializeField] public List<string> Hair;
        [SerializeField] public List<string> Shoes;
        [SerializeField] public List<string> Makeup;
        [SerializeField] public List<string> EyeTexture;
        [SerializeField] public List<string> EyeBrrow;
        [SerializeField] public List<string> EyeLashes;
        public AiManager aiManager;
        public int tempCounter = 0;

        private void Start()
        {

        }

        //public void ToggleUpdate()
        //{
        //    isWearableOrNot = wearableToggle;
        //}

        public void StartWandering(AiController ai)
        {
            // perform ai actions
            ai.isPerformingAction = false;
            if (ai.ActionCoroutine != null)
            {
                ai.StopCoroutine(ai.ActionCoroutine);
            }
            ai.ActionCoroutine = ai.StartCoroutine(ai.PerformAction());
        }

        //public IEnumerator GetAppearance(AiController ai)
        //{
        //    DecorateAI(ai);
        //    //yield return new WaitForSeconds(0.2f);
        //    //StartWandering(ai);
        //    yield return null;
        //}


        public void DecorateAI(AiController ai)
        {
            //if (isWearableOrNot)
            //{
                int rand = Random.Range(0, Uppers.Count);
                switch (tempCounter)
                {
                    case 0:
                        //yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Uppers[rand], "Chest", ai));
                        aiManager.DownloadAddressableWearableWearable(Uppers[rand], "Chest", ai);
                        Uppers.RemoveAt(rand);
                        break;

                    case 1:
                        rand = Random.Range(0, Lower.Count);
                        //yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Lower[rand], "Legs", ai));
                        aiManager.DownloadAddressableWearableWearable(Lower[rand], "Legs", ai);
                        Lower.RemoveAt(rand);
                        break;

                    case 2:
                        rand = Random.Range(0, Hair.Count);
                        //yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Hair[rand], "Hair", ai));
                        aiManager.DownloadAddressableWearableWearable(Hair[rand], "Hair", ai);
                        Hair.RemoveAt(rand);
                        break;

                    case 3:
                        rand = Random.Range(0, Shoes.Count);
                        //yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Shoes[rand], "Feet", ai));
                        aiManager.DownloadAddressableWearableWearable(Shoes[rand], "Feet", ai);
                        Shoes.RemoveAt(rand);
                        break;
                }
            //}
        }

        public void CheckMoreAIDresses(AiController ai)
        {
            tempCounter++;
            if (tempCounter >= 4)
            {
                tempCounter = 0;
                aiManager.decoratedAi++;
                aiManager.InitilizeAI();
                return;
            }
            DecorateAI(ai);
        }


    }
}

