using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Collections;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class AvatarChangerSyncing : MonoBehaviourPun
{
    [Header("Player Body Parts")]
    [SerializeField]
    private SkinnedMeshRenderer playerHair;
    [SerializeField]
    private SkinnedMeshRenderer playerBody;
    [SerializeField]
    private SkinnedMeshRenderer playerShirt;
    [SerializeField]
    private SkinnedMeshRenderer playerPants;
    [SerializeField]
    public SkinnedMeshRenderer playerShoes;
    [Header("Player Head")]
    [SerializeField]
    private SkinnedMeshRenderer playerHead;
    [SerializeField]
    public SkinnedMeshRenderer[] playerEyebrow;

    GameObject playerObj;
    Avatar defaultAvatar;
    Animator anim;
    float nameCanvasDefaultYpos;
    ArrowManager arrowManager;
    GameObject gangsterCharacter;
    bool isInitialise = false;
    MeshRenderer[] meshRenderers;
    SkinnedMeshRenderer[] skinnedMeshes;

    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        meshRenderers = gameObject.GetComponents<MeshRenderer>();
        skinnedMeshes = gameObject.GetComponents<SkinnedMeshRenderer>();
        foreach (var item in meshRenderers)
        {
            item.enabled = false;
        }
        foreach (var item in skinnedMeshes)
        {
            item.enabled = false;
        }
        StartCoroutine(SyncingCoroutine());
    }

    private IEnumerator SyncingCoroutine()
    {
        //Wait for MultiplayerComponent downloadfrom addressables
        do
        {
            yield return new WaitForSeconds(0.1f);
        }
        while (!GamificationComponentData.instance.IsMultiplayerComponentLoaded);

        string avatarChangeData = photonView.Owner.CustomProperties["avatarChanger"].ToString();
        string[] parts = avatarChangeData.Split(',');
        int avatarIndex = int.Parse(parts[0]);
        string RuntimeItemID = parts[1];
        int viewID = int.Parse(parts[2]);
        playerObj = FindPlayerusingPhotonView(photonView);
        yield return new WaitForSeconds(0.1f);
        if (playerObj != null)
        {
            AvatarController ac = playerObj.GetComponent<AvatarController>();
            CharacterBodyParts charcterBodyParts = playerObj.GetComponent<CharacterBodyParts>();
            if (ac.wornHair)
                playerHair = ac.wornHair.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornPant)
                playerPants = ac.wornPant.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShirt)
                playerShirt = ac.wornShirt.GetComponent<SkinnedMeshRenderer>();
            if (ac.wornShoes)
                playerShoes = ac.wornShoes.GetComponent<SkinnedMeshRenderer>();
            int index = 0;
            if (ac.wornEyebrow.Length > 0)
            {
                playerEyebrow = new SkinnedMeshRenderer[ac.wornEyebrow.Length];
                foreach (var eyeBrow in ac.wornEyebrow)
                {
                    playerEyebrow[index] = eyeBrow.GetComponent<SkinnedMeshRenderer>();
                    index++;
                }
                index = 0;
            }
            playerBody = charcterBodyParts.body;
            playerHead = charcterBodyParts.head;
            anim = playerObj.GetComponent<Animator>();
            defaultAvatar = anim.avatar;
            avatarIndex = avatarIndex - 1;
            arrowManager = playerObj.GetComponent<ArrowManager>();
            nameCanvasDefaultYpos = arrowManager.nameCanvas.transform.localPosition.y;
            if (avatarIndex == 1)
            {
                Vector3 canvasPos = arrowManager.nameCanvas.transform.localPosition;
                canvasPos.y = -1.3f;
                arrowManager.nameCanvas.transform.localPosition = canvasPos;
            }
            gangsterCharacter = new GameObject("AvatarChange");
            gangsterCharacter.transform.SetParent(playerObj.transform);
            gangsterCharacter.transform.localPosition = Vector3.zero;
            gangsterCharacter.transform.localEulerAngles = avatarIndex == 2 ? Vector3.up * -180 : Vector3.zero;
            //gangsterCharacter.SetActive(false);
            Vector3 pos = gangsterCharacter.transform.position;
            pos.y = GamificationComponentData.instance.AvatarChangerModelNames[avatarIndex] == "Bear05" ? 0.1f : 0;
            transform.position = pos;
            transform.SetParent(gangsterCharacter.transform);
            transform.localPosition = Vector3.up * (GamificationComponentData.instance.AvatarChangerModelNames[avatarIndex] == "Bear05" ? 0.1f : 0);
            transform.localEulerAngles = Vector3.zero;
            if (avatarIndex == 2)
            {
                var item = GamificationComponentData.instance.xanaItems.FirstOrDefault(x => x.itemData.RuntimeItemID == RuntimeItemID);
                GameObject cloneObject = Instantiate(item.gameObject);

                Component[] components = cloneObject.GetComponents<Component>();
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (!(components[i] is Transform))
                    {
                        Destroy(components[i]);
                    }
                }

                cloneObject.transform.SetParent(this.transform);
                cloneObject.transform.localPosition = Vector3.zero;
                cloneObject.transform.localEulerAngles = Vector3.zero;
                cloneObject.SetActive(true);
            }
            anim.avatar = gangsterCharacter.GetComponentInChildren<Animator>().avatar;
            //gangsterCharacter.SetActive(true);

            isInitialise = true;
            CharacterDisable(false);
            foreach (var item in meshRenderers)
            {
                item.enabled = true;
            }
            foreach (var item in skinnedMeshes)
            {
                item.enabled = true;
            }
        }
    }

    void CharacterDisable(bool state)
    {
        if (!isInitialise)
            return;

        if (playerHair)
            playerHair.enabled = state;
        if (playerBody)
            playerBody.enabled = state;
        if (playerHead)
            playerHead.enabled = state;
        if (playerPants)
            playerPants.enabled = state;
        if (playerShirt)
            playerShirt.enabled = state;
        if (playerShoes)
            playerShoes.enabled = state;
        if (playerEyebrow.Length > 0)
        {
            foreach (var eyeBrow in playerEyebrow)
            {
                eyeBrow.enabled = state;
            }
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
            return;
        if (!isInitialise)
            return;
        CharacterDisable(true);
        anim.avatar = defaultAvatar;
        Vector3 canvasPos = arrowManager.nameCanvas.transform.localPosition;
        canvasPos.y = nameCanvasDefaultYpos;
        arrowManager.nameCanvas.transform.localPosition = canvasPos;
        if (gangsterCharacter != null)
            Destroy(gangsterCharacter);
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in MutiplayerController.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                return playerObject;
            }
        }
        return null;
    }
}
