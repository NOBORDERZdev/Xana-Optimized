using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GamificationComponentData : MonoBehaviour
{
    public static GamificationComponentData instance;

    //References for Gamification Components
    public BuildingDetect buildingDetect;
    public Volume postProcessVol;
    public RuntimeAnimatorController cameraBlurEffect;
    public GameObject specialItemParticleEffect;
    public Material hologramMaterial;
    public Shader superMarioShader;
    public Shader superMarioShader2;
    public Shader skinShader;
    public Shader cloathShader;
    public GameObject[] FootSteps;
    internal PlayerControllerNew playerControllerNew;
    internal AvatarController avatarController;
    internal CharcterBodyParts charcterBodyParts;
    internal IKMuseum ikMuseum;

    public Vector3 spawnPointPosition;
    public GameObject raycast;
    public GameObject katanaPrefab;
    public GameObject shurikenPrefab;
    public GameObject throwAimPrefab;
    public Material lineMaterial;
    public Ball ThrowBall;
    public GameObject handBall;

    public GameObject helpParentReference;
    public GameObject worldSpaceCanvas;

    public AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        BuilderEventManager.ReSpawnPlayer += PlayerSpawnBlindfoldedDisplay;
    }

    private void OnDisable()
    {
        BuilderEventManager.ReSpawnPlayer -= PlayerSpawnBlindfoldedDisplay;
    }

    public void PlayerSpawnBlindfoldedDisplay()
    {
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.2f);
        playerControllerNew.transform.position = spawnPointPosition;
        yield return new WaitForSeconds(0.3f);
        Physics.IgnoreLayerCollision(9, 22, false);

        Debug.Log("Blindfolded spawned");
    }
}
