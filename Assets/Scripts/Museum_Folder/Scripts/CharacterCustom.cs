using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustom : MonoBehaviour
{
    public GameObject Bear, Bunny, Cat;
    
    public UnityEngine.Avatar[] avatars;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

        switch (CanvusHandler.canvusHandlerInstance.customCharacterSelected)
               {
            case CanvusHandler.CustomCharacter.cat:
                Bunny.SetActive(false);
                Bear.SetActive(false);
                Cat.SetActive(true);
                this.GetComponent<Animator>().avatar = (UnityEngine.Avatar)avatars.GetValue(0);
                break;
            case CanvusHandler.CustomCharacter.bunny:
                Bunny.SetActive(true);
                Bear.SetActive(false);
                Cat.SetActive(false);
                this.GetComponent<Animator>().avatar = (UnityEngine.Avatar)avatars.GetValue(1);
                break;
            case CanvusHandler.CustomCharacter.bear:

                Bunny.SetActive(false);
                Bear.SetActive(true);
                Cat.SetActive(false);
                this.GetComponent<Animator>().avatar = (UnityEngine.Avatar)avatars.GetValue(2);

                break;
            default:
            break;
        }
    }
}
