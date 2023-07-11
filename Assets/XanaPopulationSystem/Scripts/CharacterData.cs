using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "CharacterData" , menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    private GameObject[] AvailableModels;

    public Vector3 initScale;

    private int _currentIndex;

    public GameObject CreateRandomCharacter()
    {
        Random.InitState((int)DateTime.Now.Ticks);
        GameObject go =  Instantiate(AvailableModels[Random.Range(0, AvailableModels.Length - 1)]);
        go.transform.localScale = initScale;
        return go;
    }

    public GameObject CreateSequentialCharacter()
    {
        if (_currentIndex > AvailableModels.Length - 1)
        {
            _currentIndex = 0;
        }
        GameObject go =  Instantiate(AvailableModels[_currentIndex]);
        go.transform.localScale = initScale;
        _currentIndex++;
        return go;
    }
}
