using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Collections;
using Photon.Realtime;

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

    GameObject playerObj;
    Avatar defaultAvatar;
    Animator anim;
    float nameCanvasDefaultYpos;
    ArrowManager arrowManager;
    GameObject gangsterCharacter;
    bool isInitialise = false;
    [PunRPC]
    void Init(int pvID, int avatarIndex, string RuntimeItemID)
    { }
    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        StartCoroutine(SyncingCoroutine());
    }

    private IEnumerator SyncingCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        string avatarChangeData = photonView.Owner.CustomProperties["avatarChanger"].ToString();
        string[] parts = avatarChangeData.Split(',');
        int avatarIndex = int.Parse(parts[0]);
        string RuntimeItemID = parts[1];
        int viewID = int.Parse(parts[2]);
        playerObj = PhotonView.Find(viewID).gameObject;
        yield return new WaitForSeconds(0.1f);
        if (playerObj != null)
        {
            AvatarController avatarController = playerObj.GetComponent<AvatarController>();
            CharcterBodyParts charcterBodyParts = playerObj.GetComponent<CharcterBodyParts>();
            playerHair = avatarController.wornHair.GetComponent<SkinnedMeshRenderer>();
            playerPants = avatarController.wornPant.GetComponent<SkinnedMeshRenderer>();
            playerShirt = avatarController.wornShirt.GetComponent<SkinnedMeshRenderer>();
            playerShoes = avatarController.wornShose.GetComponent<SkinnedMeshRenderer>();
            playerBody = charcterBodyParts.Body;
            playerHead = charcterBodyParts.Head.GetComponent<SkinnedMeshRenderer>();
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
            gangsterCharacter.transform.localEulerAngles = Vector3.zero;
            //gangsterCharacter.SetActive(false);
            Vector3 pos = gangsterCharacter.transform.position;
            pos.y = GamificationComponentData.instance.AvatarChangerModelNames[avatarIndex] == "Bear05" ? 0.1f : 0;
            transform.position = pos;
            transform.SetParent(gangsterCharacter.transform);
            transform.localPosition= Vector3.up * (GamificationComponentData.instance.AvatarChangerModelNames[avatarIndex] == "Bear05" ? 0.1f : 0);
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
        }
    }

    void CharacterDisable(bool state)
    {
        if (!isInitialise)
            return;
        playerHair.enabled = state;
        playerBody.enabled = state;
        playerHead.enabled = state;
        playerPants.enabled = state;
        playerShirt.enabled = state;
        playerShoes.enabled = state;
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
}
