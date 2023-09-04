using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XanaAi
{
    public class AiAppearance : MonoBehaviour
    {
        public Toggle wearableToggle;
        public bool isWearableOrNot;

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

        private void Start()
        {
            //aiManager = GetComponent<AiManager>();

        }

        public void ToggleUpdate()
        {
            isWearableOrNot = wearableToggle;
        }
        public IEnumerator GetAppearance(AiController ai)
        {
            yield return new WaitForSeconds(0.2f);
            if (isWearableOrNot)
            {
                int rand = Random.Range(0, Uppers.Count);
                yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Uppers[rand], "Chest", ai));
                Uppers.RemoveAt(rand);
                rand = Random.Range(0, Lower.Count);
                yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Lower[rand], "Legs", ai));
                Lower.RemoveAt(rand);
                rand = Random.Range(0, Hair.Count);
                yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Hair[rand], "Hair", ai));
                Hair.RemoveAt(rand);
                rand = Random.Range(0, Shoes.Count);
                yield return StartCoroutine(aiManager.DownloadAddressableWearableWearable(Shoes[rand], "Feet", ai));
                Shoes.RemoveAt(rand);
            }

            ai.isPerformingAction = false;
            if (ai.ActionCoroutine !=null)
            {
                ai.StopCoroutine(ai.ActionCoroutine);
            }
            ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
        }

    }
}