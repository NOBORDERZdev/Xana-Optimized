using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UFE3D;
using Photon.Pun;
using System.IO;

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
    public Camera player1Cam;
    public Camera player2Cam;
    public GameObject player1;
    public GameObject player2;
    public Text nameStage;
    public Image portraitPlayer1;
    public Image portraitPlayer2;
    public Image screenshotStage;
    public bool stopPreviousSoundEffectsOnLoad = false;
    #endregion

    public string GetFirstNameOfPlayer(string pName)
    {
        string playername = pName;
        if (!string.IsNullOrEmpty(playername))
        {
            if (playername.Contains(" "))
            {
                string[] pnames = playername.Split(" ");
                playername = pnames[0];
            }
        }
        return playername;
    }

    #region public override methods
    public override void OnShow()
    {
        base.OnShow();

        StartCoroutine(IEDelay());
        IEnumerator IEDelay()
        {
            FightingGameManager.instance.GetPlayerData();
            player1RawImage.gameObject.SetActive(false);
            player2RawImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(.1f);
            if (UFE.gameMode == GameMode.TrainingRoom)
            {
                player1.GetComponent<AvatarController>().isLoadStaticClothFromJson = false;
                player1.GetComponent<AvatarController>().staticPlayer = true;
                player2.GetComponent<AvatarController>().isLoadStaticClothFromJson = true;
            }
            else if (UFE.gameMode == GameMode.VersusMode)
            {
                player1.GetComponent<AvatarController>().isLoadStaticClothFromJson = false;
                player1.GetComponent<AvatarController>().staticPlayer = true;
                player2.GetComponent<AvatarController>().isLoadStaticClothFromJson = true;
            }
            else
            {
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
            }
            yield return new WaitForSeconds(.1f);
            player1.GetComponent<AvatarController>().OnEnable();
            player2.GetComponent<AvatarController>().OnEnable();
            player1.GetComponent<Animator>().enabled = player2.GetComponent<Animator>().enabled = false;
            yield return new WaitForSeconds(1f);
            player1.GetComponent<Animator>().enabled = player2.GetComponent<Animator>().enabled = true;
            FightingGameManager.instance.PlayerClothJson = player1.GetComponent<AvatarController>().clothJson;
            FightingGameManager.instance.opponentClothJson = player2.GetComponent<AvatarController>().clothJson;

            SavingCharacterDataClass player1CharacterData = new SavingCharacterDataClass();
            player1CharacterData = player1CharacterData.CreateFromJSON(FightingGameManager.instance.PlayerClothJson);
            
            FightingDataManager.Instance.player1.stamina = player1CharacterData.stamina;
            FightingDataManager.Instance.player1.speed = player1CharacterData.speed;
            FightingDataManager.Instance.player1.profile = player1CharacterData.profile;
            FightingDataManager.Instance.player1.defence = player1CharacterData.defence;
            FightingDataManager.Instance.player1.special_move = player1CharacterData.special_move;
            FightingDataManager.Instance.player1.punch = player1CharacterData.punch;
            FightingDataManager.Instance.player1.kick = player1CharacterData.kick;

            SavingCharacterDataClass player2CharacterData = new SavingCharacterDataClass();
            player2CharacterData = player2CharacterData.CreateFromJSON(FightingGameManager.instance.opponentClothJson);

            FightingDataManager.Instance.player2.stamina = player2CharacterData.stamina;
            FightingDataManager.Instance.player2.speed = player2CharacterData.speed;
            FightingDataManager.Instance.player2.profile = player2CharacterData.profile;
            FightingDataManager.Instance.player2.defence = player2CharacterData.defence;
            FightingDataManager.Instance.player2.special_move = player2CharacterData.special_move;
            FightingDataManager.Instance.player2.punch = player2CharacterData.punch;
            FightingDataManager.Instance.player2.kick = player2CharacterData.kick;

            player1RawImage.gameObject.SetActive(true);
            player2RawImage.gameObject.SetActive(true);

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
                    if (UFE.gameMode == GameMode.TrainingRoom)
                    {
                        this.namePlayer1.text = GetFirstNameOfPlayer(PlayerPrefs.GetString("UserName").ToUpper());
                    }
                    else if (UFE.gameMode == GameMode.VersusMode)
                    {
                        this.namePlayer1.text = GetFirstNameOfPlayer(PlayerPrefs.GetString("UserName").ToUpper());
                    }
                    else
                    {
                        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                        {
                            this.namePlayer1.text = GetFirstNameOfPlayer(FightingGameManager.instance.player1Data.name.ToUpper());
                        }
                        else
                        {
                            this.namePlayer1.text = GetFirstNameOfPlayer(FightingGameManager.instance.player2Data.name.ToUpper());
                        }
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
                    if (UFE.gameMode == GameMode.TrainingRoom)
                    {
                        this.namePlayer2.text = GetFirstNameOfPlayer(ConstantsHolder.xanaConstants.defaultFightingName.ToUpper());
                    }
                    else if (UFE.gameMode == GameMode.VersusMode)
                    {
                        this.namePlayer2.text = GetFirstNameOfPlayer(ConstantsHolder.xanaConstants.defaultFightingName.ToUpper());
                    }
                    else
                    {
                        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                        {
                            this.namePlayer2.text = GetFirstNameOfPlayer(FightingGameManager.instance.player1Data.name.ToUpper());
                        }
                        else
                        {
                            this.namePlayer2.text = GetFirstNameOfPlayer(FightingGameManager.instance.player2Data.name.ToUpper());
                        }
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
            yield return new WaitForSeconds(1.5f);

            while (player1.GetComponent<AvatarController>().characterBodyParts.head.enabled == false && player2.GetComponent<AvatarController>().characterBodyParts.head.enabled == false)
            {
                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(.5f);

            if (!Directory.Exists((Application.persistentDataPath + "/FightingModule")))
            {
                Directory.CreateDirectory((Application.persistentDataPath + "/FightingModule"));
            }
            TakeTransparentScreenshot(player1Cam, 512, 512, Path.Combine((Application.persistentDataPath + "/FightingModule"), "player1.png"));
            TakeTransparentScreenshot(player2Cam, 512, 512, Path.Combine((Application.persistentDataPath + "/FightingModule"), "player2.png"));
            yield return new WaitForSeconds(.5f);
            UFE.DelayLocalAction(UFE.PreloadBattle, this.delayBeforePreload);
            UFE.DelayLocalAction(this.StartBattle, UFE.config._preloadingTime);
        }
        // If network synchornization is needed in this screen, use this instead
        //UFE.DelaySynchronizedAction(UFE.PreloadBattle, this.delayBeforePreload);
        //UFE.DelaySynchronizedAction(this.StartBattle, this.delayBeforePreload + UFE.config.preloadingTime + this.delayAfterPreload);
    }

    public void TakeTransparentScreenshot(Camera cam, int width, int height, string savePath)
    {
        // Depending on your render pipeline, this may not work.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels(grab_area, 0, 0);
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(savePath, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);
        Texture2D.Destroy(tex_transparent);
    }
    #endregion
}
