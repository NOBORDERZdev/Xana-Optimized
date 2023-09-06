using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UFE3D;
using Photon.Pun;

public class DefaultLoadingBattleScreen : LoadingBattleScreen
{
    #region public instance properties
    public AudioClip onLoadSound;
    public AudioClip music;
    public float delayBeforeMusic = .1f;
    public float delayBeforePreload = .5f;
    public float delayAfterPreload = .5f;
    public Text namePlayer1;
    public Text namePlayer2;
    public RawImage player1RawImage;
    public RawImage player2RawImage;
    public GameObject player1;
    public GameObject player2;
    public Text nameStage;
    public Image portraitPlayer1;
    public Image portraitPlayer2;
    public Image screenshotStage;
    public bool stopPreviousSoundEffectsOnLoad = false;
    #endregion

    #region public override methods
    public override void OnShow()
    {
        base.OnShow();
        StartCoroutine(IEDelay());
        IEnumerator IEDelay()
        {
            player1RawImage.gameObject.SetActive(false);
            player2RawImage.gameObject.SetActive(false);
            player1.GetComponent<AvatarController>().isLoadStaticClothFromJson = true;
            player2.GetComponent<AvatarController>().isLoadStaticClothFromJson = true;
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                player1.GetComponent<AvatarController>().staticClothJson = FightingGameManager.instance.player1Data.clothJson;
            }
            else
            {
                player1.GetComponent<AvatarController>().staticClothJson = FightingGameManager.instance.player2Data.clothJson;
            }
            if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                player2.GetComponent<AvatarController>().staticClothJson = FightingGameManager.instance.player1Data.clothJson;
            }
            else
            {
                player2.GetComponent<AvatarController>().staticClothJson = FightingGameManager.instance.player2Data.clothJson;
            }
            yield return new WaitForSeconds(.1f);
            player1.GetComponent<AvatarController>().OnEnable();
            player2.GetComponent<AvatarController>().OnEnable();

            player1.GetComponent<Animator>().enabled = player2.GetComponent<Animator>().enabled = false;
            yield return new WaitForSeconds(2f);
            player1.GetComponent<Animator>().enabled = player2.GetComponent<Animator>().enabled = true;
            player1RawImage.gameObject.SetActive(true);
            player2RawImage.gameObject.SetActive(true);
            if (this.music != null)
            {
                UFE.DelayLocalAction(delegate () { UFE.PlayMusic(this.music); }, this.delayBeforeMusic);
            }

            if (this.music != null)
            {
                UFE.DelayLocalAction(delegate () { UFE.PlayMusic(this.music); }, this.delayBeforeMusic);
            }

            if (this.stopPreviousSoundEffectsOnLoad)
            {
                UFE.StopSounds();
            }

            if (this.onLoadSound != null)
            {
                UFE.DelayLocalAction(delegate () { UFE.PlaySound(this.onLoadSound); }, this.delayBeforeMusic);
            }
            FightingGameManager.instance.GetPlayerData();
            if (UFE.config.player1Character != null)
            {
                if (this.portraitPlayer1 != null)
                {
                    this.portraitPlayer1.sprite = Sprite.Create(
                        UFE.config.player1Character.profilePictureBig,
                        new Rect(0f, 0f, UFE.config.player1Character.profilePictureBig.width, UFE.config.player1Character.profilePictureBig.height),
                        new Vector2(0.5f * UFE.config.player1Character.profilePictureBig.width, 0.5f * UFE.config.player1Character.profilePictureBig.height)
                    );
                }

                if (this.namePlayer1 != null)
                {
                    //this.namePlayer1.text = UFE.config.player1Character.characterName;
                    if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                    {
                        this.namePlayer1.text = FightingGameManager.instance.player1Data.name.ToUpper();
                    }
                    else
                    {
                        this.namePlayer1.text = FightingGameManager.instance.player2Data.name.ToUpper();
                    }
                }
            }

            if (UFE.config.player2Character != null)
            {
                if (this.portraitPlayer2 != null)
                {
                    this.portraitPlayer2.sprite = Sprite.Create(
                        UFE.config.player2Character.profilePictureBig,
                        new Rect(0f, 0f, UFE.config.player2Character.profilePictureBig.width, UFE.config.player2Character.profilePictureBig.height),
                        new Vector2(0.5f * UFE.config.player2Character.profilePictureBig.width, 0.5f * UFE.config.player2Character.profilePictureBig.height)
                    );
                }

                if (this.namePlayer2 != null)
                {
                    //this.namePlayer2.text = UFE.config.player2Character.characterName.ToString().ToUpper();
                    if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                    {
                        this.namePlayer2.text = FightingGameManager.instance.player1Data.name.ToUpper();
                    }
                    else
                    {
                        this.namePlayer2.text = FightingGameManager.instance.player2Data.name.ToUpper();
                    }
                }
            }

            if (UFE.config.selectedStage != null)
            {
                if (this.screenshotStage != null)
                {
                    this.screenshotStage.sprite = Sprite.Create(
                        UFE.config.selectedStage.screenshot,
                        new Rect(0f, 0f, UFE.config.selectedStage.screenshot.width, UFE.config.selectedStage.screenshot.height),
                        new Vector2(0.5f * UFE.config.selectedStage.screenshot.width, 0.5f * UFE.config.selectedStage.screenshot.height)
                    );

                    Animator anim = this.screenshotStage.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.enabled = UFE.gameMode != GameMode.StoryMode;
                    }
                }

                /*if (this.nameStage != null){
					this.nameStage.text = UFE.config.selectedStage.stageName;
				}*/
            }

            yield return new WaitForSeconds(2f);
            UFE.DelayLocalAction(UFE.PreloadBattle, this.delayBeforePreload);
            UFE.DelayLocalAction(this.StartBattle, UFE.config._preloadingTime);

            // If network synchornization is needed in this screen, use this instead
            //UFE.DelaySynchronizedAction(UFE.PreloadBattle, this.delayBeforePreload);
            //UFE.DelaySynchronizedAction(this.StartBattle, this.delayBeforePreload + UFE.config.preloadingTime + this.delayAfterPreload);
        }
        #endregion
    }
}
