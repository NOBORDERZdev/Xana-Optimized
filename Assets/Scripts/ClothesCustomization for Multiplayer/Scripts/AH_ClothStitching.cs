using UnityEngine;

public class AH_ClothStitching : MonoBehaviour
{
    public GameObject character; // Reference to the character with the body mesh
    public GameObject[] maleHair;
    public GameObject[] malePants;
    public GameObject[] maleShirts;
    public GameObject[] maleShoes;

    public GameObject[] femaleHair;
    public GameObject[] femalePants;
    public GameObject[] femaleShirts;
    public GameObject[] femaleShoes;

    private Stitcher stitcher;
    int presetIndexMale = -1;
    int presetIndexFemale = -1;

    public GameObject maleFace;
    public GameObject maleBody;
    public GameObject femaleFace;
    public GameObject femaleBody;
    bool isSwitched = false;
    public enum CharacterType { Male, Female };
    public CharacterType characterType;

    private GameObject _tempHair;
    private GameObject _tempShirt;
    private GameObject _tempPant;
    private GameObject _tempShoe;
    //private void Start()
    //{
    //    CharacterSwitch();
    //}
    private void OnEnable()
    {
        if (presetIndexMale < malePants.Length - 1)
        {
            presetIndexMale++;
            Debug.Log("presetIndexMale: " + presetIndexMale);

        }
        else
        {
            isSwitched = true;
            if (presetIndexFemale < femalePants.Length - 1)
            {
                presetIndexFemale++;
                Debug.Log("presetIndexFemale: " + presetIndexFemale);
            }
            else
            {
                presetIndexMale = 0;
                presetIndexFemale = 0;
                isSwitched = false;

            }
        }

        CharacterSwitch();

    }
    public void CharacterSwitch()
    {
        UnStitchItem();
        //isSwitched = Random.Range(0, 2) == 1; //random gender selection

        if (isSwitched)
        {
            characterType = CharacterType.Female;
            //isSwitched = false;
            maleFace.SetActive(false);
            maleBody.SetActive(false);
            femaleFace.SetActive(true);
            femaleBody.SetActive(true);
        }
        else
        {
            characterType = CharacterType.Male;
            //isSwitched = true;
            maleFace.SetActive(true);
            maleBody.SetActive(true);
            femaleFace.SetActive(false);
            femaleBody.SetActive(false);
        }
        //chose random preset
        //if (characterType == CharacterType.Male)
        //{
        //    presetIndexMale = Random.Range(0, malePants.Length);
        //}
        //else if (characterType == CharacterType.Female)
        //{
        //    presetIndexMale = Random.Range(0, femalePants.Length);
        //}
        StitchItem();
    }

    public void PresetsSwitch(bool isNextBtnPressed)
    {
        UnStitchItem();
        if (isNextBtnPressed)
        {
            presetIndexMale++;
            if (presetIndexMale >= malePants.Length)
            {
                presetIndexMale = malePants.Length - 1;
            }
        }
        else
        {
            presetIndexMale--;
            if (presetIndexMale < 0)
            {
                presetIndexMale = 0;
            }
        }
        StitchItem();
    }

    private void StitchItem()
    {
        stitcher = new Stitcher();
        if (characterType == CharacterType.Male)
        {
            _tempHair = stitcher.Stitch(maleHair[presetIndexMale], character);
            _tempPant = stitcher.Stitch(malePants[presetIndexMale], character);
            _tempShirt = stitcher.Stitch(maleShirts[presetIndexMale], character);
            if (maleShoes[presetIndexMale])
            {
                _tempShoe = stitcher.Stitch(maleShoes[presetIndexMale], character);
            }

        }
        else if (characterType == CharacterType.Female)
        {
            _tempHair = stitcher.Stitch(femaleHair[presetIndexFemale], character);
            _tempPant = stitcher.Stitch(femalePants[presetIndexFemale], character);
            _tempShirt = stitcher.Stitch(femaleShirts[presetIndexFemale], character);
            _tempShoe = stitcher.Stitch(femaleShoes[presetIndexFemale], character);
        }
    }
    private void UnStitchItem()
    {
        if (_tempHair != null)
            Destroy(_tempHair.gameObject);
        if (_tempShirt != null)
            Destroy(_tempShirt.gameObject);
        if (_tempPant != null)
            Destroy(_tempPant.gameObject);
        if (_tempShoe != null)
            Destroy(_tempShoe.gameObject);
    }
}