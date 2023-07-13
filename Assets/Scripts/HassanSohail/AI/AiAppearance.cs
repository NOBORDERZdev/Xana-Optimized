using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XanaAi
{
    public class AiAppearance : MonoBehaviour
    {
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

        public IEnumerator GetAppearance(AiController ai)
        {
            yield return new WaitForSeconds(0.2f);
            int rand = Random.Range(0, Uppers.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableWearableWearable( Uppers[rand], "Chest", ai));
            Uppers.RemoveAt(rand);
            rand = Random.Range(0, Lower.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableWearableWearable( Lower[rand], "Legs", ai));
            Lower.RemoveAt(rand);
            rand = Random.Range(0, Hair.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableWearableWearable( Hair[rand], "Hair", ai));
            Hair.RemoveAt(rand);
            rand = Random.Range(0, Shoes.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableWearableWearable( Shoes[rand], "Feet", ai));
            Shoes.RemoveAt(rand);

            rand = Random.Range(0, Makeup.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableTexture(Makeup[rand], "Makeup", ai));
            rand = Random.Range(0, EyeTexture.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableTexture( EyeTexture[rand], "EyeTexture", ai));
            rand = Random.Range(0, EyeBrrow.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableTexture( EyeBrrow[rand], "EyeBrrow", ai));
            rand = Random.Range(0, EyeLashes.Count);
            aiManager.StartCoroutine(aiManager.DownloadAddressableTexture(EyeLashes[rand], "EyeLashes", ai));

            rand = Random.Range(0, ai.GetComponent<CharcterBodyParts>().lipColor.Count);
            ai.GetComponent<CharcterBodyParts>().ChangeLipColor(rand);
            rand = Random.Range(0, ai.GetComponent<CharcterBodyParts>().lipColor.Count);
            ai.GetComponent<CharcterBodyParts>().ChangeLipColor(rand);
            rand = Random.Range(0, ai.GetComponent<CharcterBodyParts>().lipColor.Count);
            ai.GetComponent<CharcterBodyParts>().ChangeLipColor(rand);
            yield return new WaitForSeconds(Random.Range(1,2));
            ai.isPerformingAction = false;
            if (ai.ActionCoroutine !=null)
            {
                ai.StopCoroutine(ai.ActionCoroutine);
            }
            ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
        }

    }
}