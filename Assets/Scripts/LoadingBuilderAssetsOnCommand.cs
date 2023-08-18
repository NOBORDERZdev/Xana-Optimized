using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBuilderAssetsOnCommand : MonoBehaviour
{
    

    public InputField textField;

    public Button enterbutton;
    public Button animationButton;

    public Material skyBoxMat;
    public GameObject[] builderAssets;
    public string[] command;

    int x = 0;
    bool alreadyRunning = true;

    public void LoadAsset()
    {
        if (alreadyRunning)
        {
            alreadyRunning = false;
            StartCoroutine(LoadAssetDelay());
        }
    }

    IEnumerator LoadAssetDelay()
    {
        enterbutton.gameObject.SetActive(false);
        animationButton.gameObject.SetActive(true);

        GameObject temp = Instantiate(builderAssets[x]);

        temp.SetActive(true);

        for (int i = 0; i < temp.transform.childCount; i++)
        {
            yield return new WaitForSeconds(1f);
            temp.transform.GetChild(i).gameObject.SetActive(true);
        }

        if(x==0)
        {

            ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.localPosition = new Vector3(14.76f,4.5f,-25.21f);

            RenderSettings.skybox = skyBoxMat;
            DynamicGI.UpdateEnvironment();

            BuilderMapDownload._instance.terrainPlane.SetActive(false);
            BuilderMapDownload._instance.lights.SetActive(false);
        }

        x++;
        textField.text = "";
        alreadyRunning = true;

        animationButton.gameObject.SetActive(false);
        enterbutton.gameObject.SetActive(true);
    }
}
