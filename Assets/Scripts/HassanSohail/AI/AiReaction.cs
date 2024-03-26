using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XanaAi
{
    public class AiReaction : MonoBehaviour
    {
        [SerializeField] Image reactionDisplay;
        [SerializeField] float minReactionTime;
        [SerializeField] float maxReactionTime;
        private void Start()
        {
            StartCoroutine(ApplyReact());
            // Invoke(nameof(ApplyReact),1/*Random.Range(minReactionTime, maxReactionTime)*/);
        }

        public IEnumerator ApplyReact()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minReactionTime, maxReactionTime));
                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();

                if (UserReactionsHandler.Instance != null && UserReactionsHandler.Instance.reactDataClass.Count > 0)
                {
                    int rand = Random.Range(0, (UserReactionsHandler.Instance.reactDataClass.Count > 5 ? 5 : UserReactionsHandler.Instance.reactDataClass.Count));
                    if (rand < UserReactionsHandler.Instance.reactDataClass.Count)
                    {
                        string iconUrl = UserReactionsHandler.Instance.reactDataClass[rand].thumb;
                        if (iconUrl != "")
                        {
                            AssetCache.Instance.EnqueueOneResAndWait(iconUrl, iconUrl, (success) =>
                            {
                                if (success)
                                {
                                    AssetCache.Instance.LoadSpriteIntoImage(reactionDisplay, iconUrl, changeAspectRatio: true);
                                    reactionDisplay.gameObject.SetActive(true);
                                }
                                else
                                {
                                    Debug.Log("Download Failed");
                                }
                            });
                        }
                    }
                }

            }

        }



    }
}
