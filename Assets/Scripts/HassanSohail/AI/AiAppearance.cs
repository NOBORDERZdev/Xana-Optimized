using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XanaAi
{
    public class AiAppearance : MonoBehaviour
    {
        public Toggle wearableToggle;
        [Header("Wearable items")]
        [SerializeField] public List<string> Uppers;
        [SerializeField] public List<string> Lower;
        [SerializeField] public List<string> Hair;
        [SerializeField] public List<string> Shoes;
        [SerializeField] public List<string> Makeup;
        [SerializeField] public List<string> EyeTexture;
        [SerializeField] public List<string> EyeBrrow;
        [SerializeField] public List<string> EyeLashes;
        private PhotonAIController pac;
        private int rand = 0;
        private bool isDownloadWearables = true;

        private void Awake()
        {
            pac = GetComponent<PhotonAIController>();
        }

        public IEnumerator GetAppearance(AiController ai)
        {
            yield return new WaitForSeconds(0.2f);

            if (isDownloadWearables)
            {
                rand = Random.Range(0, Uppers.Count);
                pac.StartCoroutine(pac.DownloadAddressableWearableWearable(Uppers[rand], "Chest", ai));
                Uppers.RemoveAt(rand);
                rand = Random.Range(0, Lower.Count);
                pac.StartCoroutine(pac.DownloadAddressableWearableWearable(Lower[rand], "Legs", ai));
                Lower.RemoveAt(rand);
                rand = Random.Range(0, Hair.Count);
                pac.StartCoroutine(pac.DownloadAddressableWearableWearable(Hair[rand], "Hair", ai));
                Hair.RemoveAt(rand);
                rand = Random.Range(0, Shoes.Count);
                pac.StartCoroutine(pac.DownloadAddressableWearableWearable(Shoes[rand], "Feet", ai));
                Shoes.RemoveAt(rand);
            }

            ai.isPerformingAction = false;
            if (ai.ActionCoroutine !=null)
            {
                ai.StopCoroutine(ai.ActionCoroutine);
            }
            ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
        }

        public void UpdateWearableStatus()
        {
            isDownloadWearables = wearableToggle.isOn;
        }


    }
}