using UnityEngine;
using Photon.Pun;
public class ContestantsAnimationHandler : MonoBehaviourPunCallbacks
{
    private Animator contestantAnimator;
    void Start()
    {
        contestantAnimator = GetComponent<Animator>();
    }
    public void PlaySingingAnimation(bool isPlay)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(PlayeSingingDance), RpcTarget.All, isPlay);
        }
    }
    [PunRPC]
    private void PlayeSingingDance(bool flag)
    {
        contestantAnimator.SetBool("ShowStart", flag);
    }
}