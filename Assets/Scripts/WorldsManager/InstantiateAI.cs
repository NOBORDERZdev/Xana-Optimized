using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateAI : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3 playerPos;
    public Vector3[] playerPosAll;
    public Quaternion[] playerRotAll;
    public string[] playerNames;
    public string[] dialogueString;
    public string[] DialoueString1 = { "Hey there!",
    "How's it going?",
    "What's new with you?",
    "Long time no chat!",
    "How's your day been so far?",
    "I hope you're doing well.",
    "What have you been up to lately?",
    "Any exciting plans for the weekend?",
    "Tell me more about [topic].",
    "That's interesting! Tell me more.",
    "I totally agree!",
    "I see what you mean.",
    "You have a point there.",
    "I couldn't agree more.",
    "How's the weather where you are?",
    "Have you watched any good movies lately?",
    "Do you have any book recommendations?",
    "What's your favorite way to relax?",
    "What's your go-to comfort food?",
    "I'm feeling a bit tired today.",
    "I had a busy day at work/school.",
    "What's your favorite type of music?",
    "Do you enjoy cooking?",
    "How's your family doing?",
    "How's your pet doing?",
    "What's the most memorable place you've visited?",
    "What's the last thing that made you laugh?",
    "What's on your mind right now?",
    "What's your dream vacation destination?",
    "I'm feeling a bit under the weather today.",
    "What's your favorite hobby?",
    "Do you follow any sports teams?",
    "What's your favorite season of the year?",
    "I had a delicious meal for dinner.",
    "I'm so excited for [upcoming event].",
    "How's your day shaping up?",
    "What's the best piece of advice you've received?",
    "Can you recommend a good podcast?",
    "How do you like to spend your free time?",
    "It's always great chatting with you!"};


    public string[] presetJsons;
    public string[] names = { "Human","AI"};
    public string[] japanese_names1 ={
     "Yukihiro", "Akiko-AI", "Haruki", "Sakura-AI", "Ryoichi",
    "Natsumi-AI", "Hiroshi", "Aya-AI", "Kazuki", "Mai-AI",
    "Taichi", "Hana-AI", "Satoshi", "Yui-AI", "Kaito",
    "Mio-AI", "Ryota", "Yuka-AI", "Kenji", "Nao-AI",
    "Ayumu", "Yuriko-AI", "Yusuke", "Yumi-AI", "Takashi",
    "Kaori-AI", "Koji", "Emi-AI", "Riku", "Saki-AI",
    "Toshiro", "Sora-AI", "Shinichi", "Asami-AI", "Daichi",
    "Akane-AI", "Ryo", "Yumiko-AI", "Haruto", "Rena-AI"
    };
    public RuntimeAnimatorController[] playerAnimations;
    int counter=0;
    private RaycastHit hit;
    public static int totalplayer=0;
    public static InstantiateAI instance;

    public static bool startChating;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        GamePlayButtonEvents.inst.OnPeope += InstantiateAIOnClick;
        GamePlayButtonEvents.inst.OnInvite += OnInviteClick;

    }

    public void OnInviteClick()
    {
        startChating = true;
    }


    public void InstantiateAIOnClick()
    {
        Vector3 tempPos = LoadFromFile.instance.mainController.transform.position;
        Quaternion playerRot= LoadFromFile.instance.mainController.transform.rotation;

    CheckAgain:
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(tempPos, -transform.up, out hit, 2000))
        {
            if (hit.collider.gameObject.tag == "PhotonLocalPlayer" || hit.collider.gameObject.layer == LayerMask.NameToLayer("NoPostProcessing") || hit.collider.gameObject.tag == "Player")
            {
                tempPos = new Vector3(tempPos.x + UnityEngine.Random.Range(-1f, 1f), tempPos.y, tempPos.z + UnityEngine.Random.Range(-1f, 1f));
                goto CheckAgain;
            } //else if()

            else if (hit.collider.gameObject.GetComponent<NPCRandomMovement>())
            {
                tempPos = new Vector3(tempPos.x + UnityEngine.Random.Range(-2, 2), tempPos.y, tempPos.z + UnityEngine.Random.Range(-2, 2));
                goto CheckAgain;
            }

            tempPos = new Vector3(tempPos.x, hit.point.y, tempPos.z);
        }

        GameObject playerAi = Instantiate(playerPrefab, tempPos, playerRot);
        playerAi.GetComponent<Animator>().runtimeAnimatorController=playerAnimations[UnityEngine.Random.Range(0,playerAnimations.Length)];
        playerAi.GetComponent<ArrowManager>().PhotonUserName.text = names[(UnityEngine.Random.Range(0, names.Length))];
        playerAi.GetComponent<AvatarController>().DownloadAICloths(true, presetJsons[counter]);
        playerAi.GetComponent<ArrowManager>().GenerateAIChat();
        counter++;
        totalplayer++;
        if (counter > presetJsons.Length-1)
        {
            counter = 0;
            
        }
    }




}
