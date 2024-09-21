using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTutorial : MonoBehaviour
{
    public MeshRenderer ObjectMeshRenderer;
    public Texture2D EnglishText;
    public Texture2D JPText;
    // Start is called before the first frame update
    void Start()
    {
        string newLanguage = Application.systemLanguage.ToString();

        if (newLanguage == "English")
        {
            ObjectMeshRenderer.materials[2].mainTexture = EnglishText;
            ObjectMeshRenderer.materials[2].SetTexture("_EmissionMap", EnglishText);
        }
        else if (newLanguage == "Japanese")
        {

            ObjectMeshRenderer.materials[2].mainTexture = JPText;
            ObjectMeshRenderer.materials[2].SetTexture("_EmissionMap", JPText);
        }
        else
        {

            ObjectMeshRenderer.materials[2].mainTexture = JPText;
            ObjectMeshRenderer.materials[2].SetTexture("_EmissionMap", JPText);
        }
    }

   
}
