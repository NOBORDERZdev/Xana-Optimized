using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SuperStar.Helpers;

namespace NPC
{
    public class NpcReactions : MonoBehaviour
    {
        [SerializeField] Image reactionDisplay;
        private float minReactionTime = 15;
        private float maxReactionTime = 60;

        void Start()
        {
            StartCoroutine(ApplyReact());
        }

        public IEnumerator ApplyReact()
        {
            yield return new WaitForSeconds(Random.Range(minReactionTime, maxReactionTime));

            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();

            if (ReactScreen.Instance != null && ReactScreen.Instance.reactDataClass.Count > 0)
            {
                int rand = Random.Range(0, (ReactScreen.Instance.reactDataClass.Count > 5 ? 5 : ReactScreen.Instance.reactDataClass.Count));
                if (rand < ReactScreen.Instance.reactDataClass.Count)
                {
                    string iconUrl = ReactScreen.Instance.reactDataClass[rand].thumb;
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
            StartCoroutine(ApplyReact());
        }


    }
}
