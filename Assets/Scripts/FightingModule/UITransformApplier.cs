using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransformApplier : MonoBehaviour
{

    [SerializeField] string keyID = "";
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SaveUIOriginalData();
        LoadUIData();
    }
    private void OnEnable()
    {
        UITransformController.saveAndCloseEvent += LoadUIData;
    }
    private void OnDisable()
    {
        UITransformController.saveAndCloseEvent -= LoadUIData;
    }

    private void SaveUIOriginalData()
    {
        if (!PlayerPrefs.HasKey(keyID + "def"))
        {

            Vector3 scale = rectTransform.localScale;
            Vector3 position = rectTransform.localPosition;

            string dataValue = scale.x + "," + scale.y + "," + scale.z + "," + position.x + "," + position.y;

            PlayerPrefs.SetString(keyID + "def", dataValue);
            PlayerPrefs.Save();
            Debug.Log("Saved original "+ dataValue);
        }
    }
    private void LoadUIData()
    {
        if (PlayerPrefs.HasKey(keyID + "mod"))
        {
            string dataValue = PlayerPrefs.GetString(keyID + "mod");
            string[] dataComponents = dataValue.Split(',');

            if (dataComponents.Length == 5)
            {
                float scaleX = float.Parse(dataComponents[0]);
                float scaleY = float.Parse(dataComponents[1]);
                float scaleZ = float.Parse(dataComponents[2]);
                Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

                float positionX = float.Parse(dataComponents[3]);
                float positionY = float.Parse(dataComponents[4]);
                Vector3 position = new Vector3(positionX, positionY, rectTransform.localPosition.z);

                rectTransform.localScale = scale;
                rectTransform.localPosition = position;
                Debug.Log("<color=orange> Applied Saved Data </color>");
            }
        }
    }
}
