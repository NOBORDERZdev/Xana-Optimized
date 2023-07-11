using Metaverse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metaverse
{
    public class AssignAvatar : MonoBehaviour
    {
        // Start is called before the first frame update
    
        public int _characterIndex;
        public Text animationText;
        public Image avatarImg;
        public Image avatarHighLighter;
        public GameObject _character;
        private Button _onBtnpress;
       
        //assign Data to Spawn build in  Avatar buttons.
        void Start()
        {
            animationText.text = gameObject.name.ToString();
            _onBtnpress = gameObject.GetComponent<Button>();
            _onBtnpress.onClick.AddListener(SelectAvatar);
        }
        public void Init(GameObject character ,int modelIndex, Sprite img)
        {
            StartCoroutine(WaitForInit(character, modelIndex, img));
        }

        IEnumerator WaitForInit(GameObject character, int modelIndex, Sprite img)
        {
            LoadingHandler.Instance.UpdateLoadingSlider(0.9f, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("Assigning Character Values");

            yield return new WaitForSeconds(0.5f);

            _character = character;
            _characterIndex = modelIndex;
            avatarImg.sprite = img;
        }

        //assign  avatar to avatarSelection
       
        public void SelectAvatar()
        {
           AvatarManager.Instance.SelectedAvatarPreview(_character, _characterIndex);
           // _character.GetComponent<PlayerSendController>().ChangeAvatar(_characterIndex);
        }
    }

}

