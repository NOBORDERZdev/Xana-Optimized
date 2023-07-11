using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumManager : MonoBehaviour
{
    public static MuseumManager Instance;

    [Header("Environments SubParts")]
    public GameObject m_EnvironmentSubParts;


    [Header("Lobby Dialogue Camera")]
    public GameObject m_LobbyDialogueCamera;

    public GameObject m_MainPlayerCamera;

    public GameObject m_LobbyVirtualCamera;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
