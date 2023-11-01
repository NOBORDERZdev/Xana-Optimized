using UnityEngine;
using Photon.Pun;
using System.Linq;

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

    [PunRPC]
    void Init(int pvID, int avatarIndex, string RuntimeItemID)
    {
        playerObj = PhotonView.Find(pvID).gameObject;
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
        gangsterCharacter.SetActive(false);
        transform.SetParent(gangsterCharacter.transform);
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
        gangsterCharacter.transform.SetParent(playerObj.transform);
        gangsterCharacter.transform.localPosition = Vector3.zero;
        gangsterCharacter.transform.localEulerAngles = Vector3.zero;
        anim.avatar = gangsterCharacter.GetComponentInChildren<Animator>().avatar;
        gangsterCharacter.SetActive(true);

        CharacterDisable(false);
    }

    void CharacterDisable(bool state)
    {
        playerHair.enabled = state;
        playerBody.enabled = state;
        playerHead.enabled = state;
        playerPants.enabled = state;
        playerShirt.enabled = state;
        playerShoes.enabled = state;
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
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
