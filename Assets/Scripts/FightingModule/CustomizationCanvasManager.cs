using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationCanvasManager : MonoBehaviour
{
    #region Vars
    public AudioClip clip_1, clip_2;

    [SerializeField] Sprite defaultSelection, highlightSelection;
    [SerializeField] GameObject _buttonPanel, _animationsPanel;
    [SerializeField] GameObject _buttonPrefab;
    [SerializeField] GameObject _buttonParent;
    [SerializeField] TextMeshProUGUI hintLabel;
    [SerializeField] GameObject _presetSaveIndicator;
    [SerializeField] List<Button> _animParent;

    List<Button> _animButtons;
    private Animator animator;
    private Transform player1Transform;
    GameObject otherPlayer;
    private Animator otherPlayerAnimator;
    private string comboTitle = "";
    Vector3 _otherPlayerPosition;
    Quaternion _otherPlayerRotation;
    public int _noofRounds = 1;
    int animAdder = 0; // Using for selecting animation combo
    public static CustomizationCanvasManager _instance;
    public CustomAnimManager _customAnimManager;
    public Button[] roundButton;

    public Transform[] grabTranform;
    public Transform[] specialGrabTranform;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        //PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        animator = CustomizationManager._instance._animator;
        player1Transform = animator.transform;
        //   animator = _otherAnim;
        otherPlayer = CustomizationManager._instance.reactionPlayer; // Using other player to demonstrate it for grab animations
        otherPlayerAnimator = otherPlayer.GetComponent<Animator>();
        _otherPlayerPosition = otherPlayer.transform.position;
        _otherPlayerRotation = otherPlayer.transform.rotation;
        _animButtons = new List<Button>();
        //_customAnimManager = GetComponent<CustomAnimManager>();
        CheckPresetsConfig();
        if (PlayerPrefs.GetInt("saved") == 1)
        {
            LoadPreset();
        }
        // Disabling the button for the current round number and enabling others
        for (int i = 0; i < roundButton.Length; i++)
        {
            roundButton[i].interactable = (i + 1) != _noofRounds;
        }
    }
    #endregion

    #region User Functions
    // combos key for combo selection
    public void GetName(string _comboName)
    {
        AudioManager.instance.PlayAudioClip(clip_1);
        comboTitle = _comboName;
        int index = GetComboIndex(comboTitle);
        for (int i = 0; i < _animParent.Count; i++)
        {
            if (i != index)
            {
                _animParent[i].enabled = true;
                _animParent[i].gameObject.GetComponent<Image>().sprite = defaultSelection;
            }
            else
            {
                _animParent[i].enabled = false;
                _animParent[i].gameObject.GetComponent<Image>().sprite = highlightSelection;
            }
        }
    }

    private int GetComboIndex(string comboTitle)
    {
        int index = -1;
        switch (comboTitle)
        {
            case "basicCombo":
                index = 0;
                break;
            case "basicCombo2":
                index = 1;
                break;
            case "specialCombo":
                index = 2;
                break;
            case "throw":
                index = 3;
                break;
            case "specialGrab":
                index = 4;
                break;
        }
        return index;
    }

    //Number of rounds selection
    public void SetRounds(int _value)
    {
        AudioManager.instance.PlayAudioClip(clip_2);
        _noofRounds = _value;
        // Disabling the button for the current round number and enabling others
        for (int i = 0; i < roundButton.Length; i++)
        {
            roundButton[i].interactable = (i + 1) != _noofRounds;
        }
    }

    //Generating Buttons for each combos i.e punch, kick, etc.
    public void GenerateButtons(int _noofbuttons)
    {
        _animButtons.Clear();
        if (_buttonParent.transform.childCount > 0)
        {
            for (int i = 0; i < _buttonParent.transform.childCount; i++)
            {
                Destroy(_buttonParent.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < _noofbuttons; i++)
        {
            GameObject go = Instantiate(_buttonPrefab, _buttonParent.transform);
            int animnumber = i + 1;
            TextMeshProUGUI showName = go.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            showName.text = "Animation " + animnumber;
            _animButtons.Add(go.GetComponent<Button>());
            int index = GetComboSelectedIndex(comboTitle);
            for (int a = 0; a < _animButtons.Count; a++)
            {
                if (a != index)
                {
                    _animButtons[a].gameObject.GetComponent<Image>().sprite = defaultSelection;
                }
                else
                {
                    _animButtons[a].gameObject.GetComponent<Image>().sprite = highlightSelection;
                }
            }
            int send = i;
            if (comboTitle == "basicCombo" || comboTitle == "basicCombo2")
            {
                int _stmachinNo = 0;
                if (comboTitle == "basicCombo")
                {
                    _stmachinNo = 1;
                }
                else if (comboTitle == "basicCombo2")
                {
                    _stmachinNo = 2;
                }
                go.GetComponent<Button>().onClick.AddListener(delegate { PlayAnimBasicCombos(comboTitle, _stmachinNo, send); });
            }
            else
            {
                go.GetComponent<Button>().onClick.AddListener(delegate { PlayAnim(comboTitle, send); });

            }
        }
    }

    private int GetComboSelectedIndex(string comboTitle)
    {
        int index = -1;
        switch (comboTitle)
        {
            case "basicCombo":
                index = ShotsSelectionData._instance.combo1Selected;
                break;
            case "basicCombo2":
                index = ShotsSelectionData._instance.combo2Selected;
                break;
            case "specialCombo":
                index = ShotsSelectionData._instance._specialComboVal;
                break;
            case "throw":
                index = ShotsSelectionData._instance._throwVal;
                break;
                //case "specialGrab":
                //    index = ShotsSelectionData._instance._grabVal;
                //    break;
        }
        return index;
    }

    //Plays respective animation when button is clicked
    public void PlayAnim(string _blendTitle, int _blendValue)
    {
        if (animator == null)
        {
            animator = CustomizationManager._instance._animator;
        }
        else { otherPlayer.SetActive(false); }
        for (int i = 0; i < _animButtons.Count; i++)
        {
            _animButtons[i].interactable = false;
        }
        for (int i = 0; i < _animParent.Count; i++)
        {
            _animParent[i].interactable = false;
        }
        //animator.SetFloat(_blendTitle, -1);
        animator.SetFloat(_blendTitle, _blendValue);
        if (_blendTitle == "throw")// || _blendTitle == "specialGrab")
        {
            if (_blendTitle == "throw")
            {
                otherPlayer.transform.position = grabTranform[_blendValue].position;
                otherPlayer.transform.rotation = grabTranform[_blendValue].rotation;
            }
            //else if (_blendTitle == "specialGrab")
            //{
            //    otherPlayer.transform.position = specialGrabTranform[_blendValue].position;
            //    otherPlayer.transform.rotation = specialGrabTranform[_blendValue].rotation;
            //}
            otherPlayer.transform.LookAt(player1Transform);
            otherPlayer.SetActive(true);
            otherPlayerAnimator.SetFloat(_blendTitle, _blendValue + 12);
        }
        switch (_blendTitle)
        {
            case "specialCombo":
                ShotsSelectionData._instance._specialComboVal = _blendValue;
                break;
            case "throw":
                ShotsSelectionData._instance._throwVal = _blendValue;
                break;
                //case "specialGrab":
                //    ShotsSelectionData._instance._grabVal = _blendValue;
                //    break;
        }
        StartCoroutine(StopAnimation(_blendTitle));
    }

    /// <summary>
    ///Plays basic Punch and kick animations 
    /// </summary>

    public void PlayAnimBasicCombos(string _blendTitle, int _blendValue, int _animNumber)
    {
        print("BLEND VALUE HERE : " + _blendValue);
        if (animator == null)
        {
            animator = CustomizationManager._instance._animator;
        }
        for (int i = 0; i < _animButtons.Count; i++)
        {
            _animButtons[i].interactable = false;
        }
        for (int i = 0; i < _animParent.Count; i++)
        {
            _animParent[i].interactable = false;
        }
        //  animator.SetFloat(_blendTitle, -1);
        //   animator.SetFloat(_blendTitle, _blendValue);

        int sender = _blendValue + 1;
        //Text Show In UI
        switch (_blendTitle)
        {
            case "basicCombo":
                animator.runtimeAnimatorController = _customAnimManager._combo1ActionAnims[_animNumber];
                ShotsSelectionData._instance._basicCombo1Val = _blendValue;
                ShotsSelectionData._instance.combo1ActionAnims = _customAnimManager._combo1ActionAnims[_animNumber];
                ShotsSelectionData._instance.combo1ReactionAnims = _customAnimManager._combo1ReactionAnims[_animNumber];
                ShotsSelectionData._instance.combo1Selected = _animNumber;
                animator.SetTrigger("BasicCombo_1");
                print("Setting Value for 1 : " + _blendValue);
                break;
            case "basicCombo2":
                animator.runtimeAnimatorController = _customAnimManager._combo2ActionAnims[_animNumber];
                ShotsSelectionData._instance._basicCombo2Val = _blendValue;
                ShotsSelectionData._instance.combo2ActionAnims = _customAnimManager._combo2ActionAnims[_animNumber];
                ShotsSelectionData._instance.combo2ReactionAnims = _customAnimManager._combo2ReactionAnims[_animNumber];
                ShotsSelectionData._instance.combo2Selected = _animNumber;
                animator.SetTrigger("BasicCombo_2");
                print("Setting Value for 2 : " + _blendValue);
                break;
        }
        animator.SetBool("Hit_1", true);
        animator.SetBool("Hit_2", true);
        animator.SetBool("Hit_3", true);
        StartCoroutine(BasicCombosStopAnimation(_blendTitle, _blendValue));
    }
    /// <summary>
    /// Stops animation once end and set button interactable
    /// </summary>

    IEnumerator StopAnimation(string _title)
    {
        yield return new WaitForSeconds(0.25f);
        AnimatorControllerParameter[] parameters = animator.parameters;
        AnimationClip currentClip = null;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        int currentAnimationHash = stateInfo.fullPathHash;
        float blendParameter = animator.GetFloat(_title);
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Float && animator.GetFloat(parameter.name) == blendParameter)
            {
                currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                break;
            }
        }
        float animationLength = (currentClip != null) ? currentClip.length : 0;
        yield return new WaitForSeconds(animationLength + 0.25f);
        animator.SetFloat(_title, -1);
        otherPlayerAnimator.SetFloat(_title, -1);
        otherPlayer.gameObject.SetActive(false);
        int index = GetComboSelectedIndex(_title);
        for (int i = 0; i < _animButtons.Count; i++)
        {
            if (i != index)
            {
                _animButtons[i].gameObject.GetComponent<Image>().sprite = defaultSelection;
            }
            else
            {
                _animButtons[i].gameObject.GetComponent<Image>().sprite = highlightSelection;
            }
            _animButtons[i].interactable = true;
        }
        int index_2 = GetComboIndex(comboTitle);
        for (int i = 0; i < _animParent.Count; i++)
        {
            if (i != index_2)
            {
                _animParent[i].enabled = true;
                _animParent[i].gameObject.GetComponent<Image>().sprite = defaultSelection;
            }
            else
            {
                _animParent[i].enabled = false;
                _animParent[i].gameObject.GetComponent<Image>().sprite = highlightSelection;
            }
            _animParent[i].interactable = true;
        }
    }

    /// <summary>
    /// Stops basic combo on end animation and set button interactable
    /// </summary>
    IEnumerator BasicCombosStopAnimation(string _title, int _currentAnim)
    {
        while (animator.GetBool("Hit_3"))
        {
            yield return null;
        }

        animator.SetFloat(_title, -1);
        otherPlayerAnimator.SetFloat(_title, -1);
        otherPlayer.gameObject.SetActive(false);
        int index = GetComboSelectedIndex(_title);
        for (int i = 0; i < _animButtons.Count; i++)
        {
            if (i != index)
            {
                _animButtons[i].gameObject.GetComponent<Image>().sprite = defaultSelection;
            }
            else
            {
                _animButtons[i].gameObject.GetComponent<Image>().sprite = highlightSelection;
            }
            _animButtons[i].interactable = true;
        }
        int index_2 = GetComboIndex(comboTitle);
        for (int i = 0; i < _animParent.Count; i++)
        {
            if (i != index_2)
            {
                _animParent[i].enabled = true;
                _animParent[i].gameObject.GetComponent<Image>().sprite = defaultSelection;
            }
            else
            {
                _animParent[i].enabled = false;
                _animParent[i].gameObject.GetComponent<Image>().sprite = highlightSelection;
            }
            _animParent[i].interactable = true;
        }
    }

    IEnumerator Check()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Hit_1", true);
        animator.SetBool("Hit_2", true);
        animator.SetBool("Hit_3", true);
        animator.SetTrigger("BasicCombo_1");
    }
    #endregion

    /// <summary>
    /// Below section checks if we have preset saved then load it and save mechanism is also implemented 
    /// </summary>
    #region PresetConfig
    private void CheckPresetsConfig()
    {
        if (PlayerPrefs.GetInt("saved") == 1)
        {
            return;
        }
        if (!PlayerPrefs.HasKey("setBasicCombo1")) { PlayerPrefs.SetInt("setBasicCombo1", 0); }
        if (!PlayerPrefs.HasKey("setBasicCombo2")) { PlayerPrefs.SetInt("setBasicCombo2", 0); }
        if (!PlayerPrefs.HasKey("setSpecialCombo")) { PlayerPrefs.SetInt("setSpecialCombo", 0); }
        if (!PlayerPrefs.HasKey("setThrowCombo")) { PlayerPrefs.SetInt("setThrowCombo", 0); }
        if (!PlayerPrefs.HasKey("setGrabCombo")) { PlayerPrefs.SetInt("setGrabCombo", 0); }
        if (!PlayerPrefs.HasKey("setbasic1Anim")) { PlayerPrefs.SetInt("setbasic1Anim", 0); }
        if (!PlayerPrefs.HasKey("setbasic2Anim")) { PlayerPrefs.SetInt("setbasic2Anim", 0); }
        if (PlayerPrefs.HasKey("saved")) { PlayerPrefs.SetInt("saved", 0); }
    }

    public void LoadPreset()
    {
        ShotsSelectionData._instance._basicCombo1Val = -1;
        ShotsSelectionData._instance._basicCombo2Val = -1;
        ShotsSelectionData._instance._specialComboVal = -1;
        ShotsSelectionData._instance._throwVal = -1;
        // ShotsSelectionData._instance._grabVal = -1;// _noofRounds = 1;
        ShotsSelectionData._instance._basicCombo1Val = PlayerPrefs.GetInt("setBasicCombo1");
        ShotsSelectionData._instance._basicCombo2Val = PlayerPrefs.GetInt("setBasicCombo2");
        ShotsSelectionData._instance._specialComboVal = PlayerPrefs.GetInt("setSpecialCombo");
        ShotsSelectionData._instance._throwVal = PlayerPrefs.GetInt("setThrowCombo");
        //   ShotsSelectionData._instance._grabVal = PlayerPrefs.GetInt("setGrabCombo");
        ShotsSelectionData._instance.combo1Selected = PlayerPrefs.GetInt("setbasic1Anim");
        ShotsSelectionData._instance.combo2Selected = PlayerPrefs.GetInt("setbasic2Anim");
        ShotsSelectionData._instance.combo1ActionAnims = _customAnimManager._combo1ActionAnims[ShotsSelectionData._instance.combo1Selected];
        ShotsSelectionData._instance.combo2ActionAnims = _customAnimManager._combo2ActionAnims[ShotsSelectionData._instance.combo2Selected];
        ShotsSelectionData._instance.combo1ReactionAnims = _customAnimManager._combo1ReactionAnims[ShotsSelectionData._instance.combo1Selected];
        ShotsSelectionData._instance.combo2ReactionAnims = _customAnimManager._combo2ReactionAnims[ShotsSelectionData._instance.combo2Selected];
    }

    public void SavePreset()
    {
        AudioManager.instance.PlayAudioClip(clip_2);
        if (ShotsSelectionData._instance.combo1Selected == -1 || ShotsSelectionData._instance.combo2Selected == -1 || ShotsSelectionData._instance._specialComboVal == -1
            || ShotsSelectionData._instance._throwVal == -1)//||ShotsSelectionData._instance._grabVal == -1 )
        {
            _presetSaveIndicator.SetActive(false);
            hintLabel.text = "Pick all combos at first.";
            hintLabel.gameObject.SetActive(true);
            StartCoroutine(PresetOff());
            return;
        }
        PlayerPrefs.SetInt("setBasicCombo1", ShotsSelectionData._instance._basicCombo1Val);
        PlayerPrefs.SetInt("setBasicCombo2", ShotsSelectionData._instance._basicCombo2Val);
        PlayerPrefs.SetInt("setSpecialCombo", ShotsSelectionData._instance._specialComboVal);
        PlayerPrefs.SetInt("setThrowCombo", ShotsSelectionData._instance._throwVal);
        //PlayerPrefs.SetInt("setGrabCombo", ShotsSelectionData._instance._grabVal);
        PlayerPrefs.SetInt("setbasic1Anim", ShotsSelectionData._instance.combo1Selected);
        PlayerPrefs.SetInt("setbasic2Anim", ShotsSelectionData._instance.combo2Selected);
        PlayerPrefs.SetInt("saved", 1);
        // hintLabel.GetComponent<TextMeshProUGUI>().text = "Saved preset.";
        _presetSaveIndicator.SetActive(true);
        StartCoroutine(PresetOff());
    }

    IEnumerator PresetOff()
    {
        yield return new WaitForSeconds(2f);
        hintLabel.gameObject.SetActive(false);
        _presetSaveIndicator.SetActive(false);
    }
    #endregion
}
