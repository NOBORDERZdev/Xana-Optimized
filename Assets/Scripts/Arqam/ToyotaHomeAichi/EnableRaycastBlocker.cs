using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toyota
{
    public class EnableRaycastBlocker : MonoBehaviour
    {
        public AR_Nft_Manager nft_Manager;
        [System.Serializable]
        public class RayBlock
        {
            public string name;
            public GameObject actualObj;
            public GameObject[] rayBlockerObjects;
        }
        [Space(5)]
        public RayBlock[] rayBlock;

        private void Awake()
        {
            for (int i = 0; i < rayBlock.Length; i++)
            {
                rayBlock[i].actualObj.SetActive(false);
                for (int j = 0; j < rayBlock[i].rayBlockerObjects.Length; j++)
                {
                    if (rayBlock[i].rayBlockerObjects[j])
                        rayBlock[i].rayBlockerObjects[j].SetActive(false);
                }
            }
        }

        public void EnableRayBlockerContent()
        {
            for (int i = 0; i < rayBlock.Length; i++)  //nft_Manager.worldInfos.Count
            {
                //if (i == 0) continue;
                rayBlock[i].actualObj.SetActive(true);
                switch (nft_Manager.worldInfos[i].pmyRatio)
                {
                    case PMY_Ratio.NineXSixteenWithDes:       //9:16
                        if (rayBlock[i].rayBlockerObjects.Length >= 1)
                            rayBlock[i].rayBlockerObjects[0].SetActive(true);
                        break;
                    case PMY_Ratio.NineXSixteenWithoutDes:       //9:16
                        if (rayBlock[i].rayBlockerObjects.Length >= 1)
                            rayBlock[i].rayBlockerObjects[0].SetActive(true);
                        break;

                    case PMY_Ratio.SixteenXNineWithDes:       //16:9
                        if (rayBlock[i].rayBlockerObjects.Length > 1)
                            rayBlock[i].rayBlockerObjects[1].SetActive(true);
                        break;
                    case PMY_Ratio.SixteenXNineWithoutDes:       //16:9
                        if (rayBlock[i].rayBlockerObjects.Length > 1)
                            rayBlock[i].rayBlockerObjects[1].SetActive(true);
                        break;

                    case PMY_Ratio.OneXOneWithDes:             //1:1
                        if (rayBlock[i].rayBlockerObjects.Length >= 2)
                            rayBlock[i].rayBlockerObjects[2].SetActive(true);
                        break;
                    case PMY_Ratio.OneXOneWithoutDes:             //1:1
                        if (rayBlock[i].rayBlockerObjects.Length >= 2)
                            rayBlock[i].rayBlockerObjects[2].SetActive(true);
                        break;

                    case PMY_Ratio.FourXThreeWithDes:        //4:3
                        if (rayBlock[i].rayBlockerObjects.Length >= 3)
                            rayBlock[i].rayBlockerObjects[3].SetActive(true);
                        break;
                    case PMY_Ratio.FourXThreeWithoutDes:        //4:3
                        if (rayBlock[i].rayBlockerObjects.Length >= 3)
                            rayBlock[i].rayBlockerObjects[3].SetActive(true);
                        break;
                }
            }
        }


    }
}
