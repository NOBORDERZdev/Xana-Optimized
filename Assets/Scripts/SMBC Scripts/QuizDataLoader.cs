using UnityEngine;
using System;
using Models;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SMBC Quiz data", menuName = "ScriptableObjects/SMBCQuizdata")]
public class QuizDataLoader : ScriptableObject
{
    public QuizData SMBC_Forest_Planet;
    public QuizData SMBC_Icy_Planet;
    public QuizData SMBC_Volcanic_Planet;

    public string ForestQuizJsonData;
    public string IceQuizJsonData;
    public string FireQuizJsonData;

    public QuizData GetQuizData(string planetName)
    {
        switch (planetName)
        {
            case "SMBC_Forest_Planet":
                SMBC_Forest_Planet.WorldQuizComponentData = JsonUtility.FromJson<QuizComponentData>(ForestQuizJsonData);
                return SMBC_Forest_Planet;
            case "SMBC_Icy_Planet":
                SMBC_Icy_Planet.WorldQuizComponentData = JsonUtility.FromJson<QuizComponentData>(IceQuizJsonData);
                return SMBC_Icy_Planet;
            case "SMBC_Volcanic_Planet":
                SMBC_Volcanic_Planet.WorldQuizComponentData = JsonUtility.FromJson<QuizComponentData>(FireQuizJsonData);
                return SMBC_Volcanic_Planet;
            default:
                return null;
        }
    }
}

[Serializable]
public class QuizData
{
    public QuizComponentData WorldQuizComponentData = new QuizComponentData();
    public List<string> Explanation = new List<string>();
}
