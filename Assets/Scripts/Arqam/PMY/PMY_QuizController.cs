using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine.Events;


namespace PMY
{
    public class PMY_QuizController : MonoBehaviour
    {
        public string quizJson;
        //public GameObject quizComponentUI;
        public Button nextButton;
        public GameObject quizParentReference;
        public GameObject scoreCanvas;
        public Button[] options = new Button[4];
        public Sprite wrongImage;
        public Sprite correctImage;
        public GameObject[] correctWrongImageObjects;
        public bool isDissapearing = false;
        public TMP_Text numberOfQuestions;
        public TMP_Text quizButtonTextInformation;
        public TMP_Text correctText;
        public TMP_Text wrongText;
        public TMP_Text scorePercentage;
        [Space(5)]
        public UnityEvent passFirstQuiz;
        public UnityEvent passSecondQuiz;

        [Tooltip("Quiz data fetch from server")]
        public QuizComponentData quizComponentData = new();
        private TMP_Text nextButtonText;
        string confirm = "Confirm";
        string result = "Result";
        string next = "Next";
        int questionIndex;
        int numOfQuestions;
        int correct, wrong;
        int currentAnswer;
        readonly int inputFieldsPerQuestion = 5; //one question and four options
        bool isOptionSelected = false;
        bool isFirstQuestion = true;
        bool isPassFirstQuiz = false;
        Outline currentOutline;


        private void OnEnable()
        {
            //quizComponentData = JsonUtility.FromJson<QuizComponentData>(quizJson);
            //EnableQuizComponentUI(quizComponentData);
        }
        public void SetQuizData(QuizData _quizData)
        {

            QuizComponentData data = new QuizComponentData();
            data.question = _quizData.question;
            data.answer = _quizData.answer;
            data.correct = _quizData.correct;

            data.rewritingStringList.Add(_quizData.question);
            for (int i=0; i<_quizData.answer.Count; i++)
            {
                data.rewritingStringList.Add(_quizData.answer[i]);
            }
            EnableQuizComponentUI(data);

        }

        void EnableQuizComponentUI(QuizComponentData quizComponentData)
        {
            confirm = "Confirm";
            result = "Result";
            next = "Next";
            //quizComponentUI.SetActive(true);
            StartQuiz(quizComponentData);
        }

        public void StartQuiz(QuizComponentData data)
        {
            for (int i = 0; i < options.Length; i++)
                options[i].onClick.RemoveAllListeners();

            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(delegate { DisplayNextQuestion(); });
            for (int i = 0; i < options.Length; i++)
            {
                int c = i;
                options[c].onClick.AddListener(delegate { OnSelectOption(c); });
                options[c].GetComponent<Outline>().enabled = false;
                correctWrongImageObjects[c].SetActive(false);
            }

            this.quizComponentData = null;
            quizParentReference.SetActive(false);
            this.quizComponentData = data;

            SetInitialValues();

            if (scoreCanvas.activeInHierarchy)
                scoreCanvas.SetActive(false);

            DisplayNextQuestion();
            quizParentReference.SetActive(true);
        }

        private void SetInitialValues()
        {
            numOfQuestions = 1;// quizComponentData.answer.Count;

            questionIndex = 0;
            correct = 0;
            wrong = 0;

            nextButtonText = nextButton.GetComponentInChildren<TMP_Text>();
            confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
            nextButtonText.text = confirm;

            isFirstQuestion = true;
            isOptionSelected = false;
        }

        private void CheckAnswer()
        {
            if (quizComponentData.answer[currentAnswer] == quizComponentData.correct)
                UpdateQuizData(0);
            else
            {
                currentOutline.enabled = false;
                UpdateQuizData(1);
            }

            questionIndex += 1;
            next = TextLocalization.GetLocaliseTextByKey("Next");
            result = TextLocalization.GetLocaliseTextByKey("Result");

            nextButtonText.text = (questionIndex < numOfQuestions) ? next : result;
            SetButtonInteractability(true, false);
        }

        void UpdateQuizData(int option)
        {
            string colorString = "";
            Sprite image = correctImage;

            //0 if the answer is correct, 1 if wrong, 2 is for selecting only (answer not yet confirmed)
            switch (option)
            {
                case 0:
                    correct++;
                    image = correctImage;
                    if (ReferrencesForDynamicMuseum.instance)
                        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.QuizCorrect);

                    colorString = "#36C34E";
                    break;

                case 1:
                    wrong++;
                    image = wrongImage;
                    if (ReferrencesForDynamicMuseum.instance)
                        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.QuizWrong);

                    break;

                case 2:
                    colorString = "#008FFF";
                    break;
            }

            if (option == 0 || option == 1)
            {
                correctWrongImageObjects[currentAnswer].SetActive(true);
            }

            correctWrongImageObjects[currentAnswer].GetComponent<Image>().sprite = image;

            Outline thisButtonOutine = options[currentAnswer].GetComponent<Outline>();

            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                thisButtonOutine.effectColor = color;
            }

            if (option == 0 || option == 2)
            {
                thisButtonOutine.enabled = true;
                currentOutline = thisButtonOutine;
            }

            if (option == 1)
            {
                WrongAnswerUIAdjustments(correctImage, "#36C34E");
            }
        }

        private void WrongAnswerUIAdjustments(Sprite image, string colorString)
        {
            correctWrongImageObjects[questionIndex].SetActive(true);
            correctWrongImageObjects[questionIndex].GetComponent<Image>().sprite = image;

            Outline thisButtonOutine = options[questionIndex].GetComponent<Outline>();

            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                thisButtonOutine.effectColor = color;
            }

            thisButtonOutine.enabled = true;
            currentOutline = thisButtonOutine;
        }

        private void SetButtonInteractability(bool isOptions, bool isInteractable)
        {
            if (isOptions)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    options[i].transition = Selectable.Transition.None;
                    options[i].interactable = isInteractable;
                    if (isInteractable)
                    {
                        options[i].GetComponent<Outline>().enabled = false;
                        correctWrongImageObjects[i].SetActive(false);
                    }
                }
            }
        }

        void OnSelectOption(int answer)
        {
            currentAnswer = answer;
            SetButtonInteractability(true, true);
            UpdateQuizData(2);

            if (!isOptionSelected)
            {
                isOptionSelected = true;
                confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
                //Debug.Log("TextLocalization==>" + confirm);

                nextButtonText.text = confirm;
            }
        }

        public void QuizResultPopupClose()
        {
            PMY_Nft_Manager.Instance.ActionOnExitBtn();
            if (scoreCanvas.activeInHierarchy)
                scoreCanvas.SetActive(false);
            CheckScorePercentage();
        }

        public void CheckScorePercentage()
        {
            StartCoroutine(CheckScorePercentageRoutine());
        }

        IEnumerator CheckScorePercentageRoutine()
        {
            float division = (((float)correct / numOfQuestions) * 100);

            if (division >= quizComponentData.correctAnswerRate)
            {
                isDissapearing = true;
                yield return new WaitForSeconds(0);
                isDissapearing = false;

                if (!isPassFirstQuiz)
                {
                    isPassFirstQuiz = true;
                    passFirstQuiz.Invoke();
                }
                else
                    passSecondQuiz.Invoke();
                ResetCredentials();
            }
            else
            {
                if (scoreCanvas.activeInHierarchy)
                    scoreCanvas.SetActive(false);
                quizParentReference.SetActive(true);
                ResetParams();
                ShowQuestion();
            }
            
        }

        void ResetCredentials()
        {
            ResetParams();
            DisableQuizComponentUI();
        }

        private void ResetParams()
        {
            correct = 0;
            wrong = 0;
            questionIndex = 0;
        }
        internal void DisableQuizComponentUI()
        {
            //quizComponentUI.SetActive(false);
            if (CanvasButtonsHandler.inst.gameObject.activeInHierarchy)
            {
                CanvasButtonsHandler.inst.gamePlayUIParent.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }

        private void DisplayNextQuestion()
        {
            if (nextButtonText.text == confirm)
            {
                if (!isOptionSelected)
                {
                    if (isFirstQuestion)
                        ShowQuestion();
                    else
                        Debug.Log("Please Select an Option First");

                    isFirstQuestion = false;
                }
                else
                {
                    CheckAnswer();
                }

                return;
            }

            if (nextButtonText.text == next)
            {
                ShowQuestion();
                return;
            }

            if (nextButtonText.text == result)
            {
                quizParentReference.SetActive(false);
                ShowScoreCanvasRoutine();
            }
        }

        void ShowQuestion()
        {
            SetButtonInteractability(true, true);
            confirm = TextLocalization.GetLocaliseTextByKey("Confirm");
            nextButtonText.text = confirm;

            if (questionIndex < numOfQuestions)
            {
                string s = TextLocalization.GetLocaliseTextByKey("Question");
                string s2 = "/";
                string s3 = TextLocalization.GetLocaliseTextByKey("Q");

                numberOfQuestions.text = s + " " + (questionIndex + 1) + " " + s2 + " " + numOfQuestions;
                quizButtonTextInformation.text = s3 + ": " + quizComponentData.rewritingStringList[questionIndex * inputFieldsPerQuestion];

                for (int i = 1; i < inputFieldsPerQuestion; i++)
                {
                    string sb = quizComponentData.rewritingStringList[i + (questionIndex * inputFieldsPerQuestion)];
                    options[i - 1].GetComponentInChildren<TMP_Text>().text =
                        sb;

                    if (GameManager.currentLanguage == "ja" || CustomLocalization.forceJapanese || ContainsJapaneseText(sb))
                        options[i - 1].GetComponentInChildren<TMP_Text>().fontSize = 11.3f;
                    else
                        options[i - 1].GetComponentInChildren<TMP_Text>().fontSize = 12;
                }
            }

            isOptionSelected = false;
        }

        private void ShowScoreCanvasRoutine()
        {
            scoreCanvas.SetActive(true);
            string s = TextLocalization.GetLocaliseTextByKey("Correct");
            string s2 = TextLocalization.GetLocaliseTextByKey("Wrong");
            string s3 = TextLocalization.GetLocaliseTextByKey("Correct Answer is");
            string s4 = TextLocalization.GetLocaliseTextByKey("Confirm");

            correctText.text = s + ": " + correct;
            wrongText.text = s2 + ": " + wrong;
            float percentage = (((float)correct / numOfQuestions) * 100);
            scorePercentage.text = s3 + " " + percentage.ToString("0.#") + "%";

            isFirstQuestion = true;
            isOptionSelected = false;
            nextButtonText.text = s4;
        }

        bool ContainsJapaneseText(string input)
        {
            foreach (char c in input)
            {
                // Check if the character belongs to the Unicode category of Japanese scripts
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category == UnicodeCategory.OtherLetter || category == UnicodeCategory.LetterNumber)
                {
                    return true;
                }
            }
            return false;
        }


        [System.Serializable]
        public class QuizComponentData
        {
            public string question;
            public List<string> answer;
            public string correct;

            public bool IsActive;
            public bool isOptionSelected;
            public List<TMPro.TMP_InputField> rewritingInputList;
            public List<string> rewritingStringList;
            public List<int> answers;
            public List<int> charLimit;
            public float correctAnswerRate;

            public QuizComponentData()
            {
                IsActive = false;
                isOptionSelected = false;
                rewritingInputList = new List<TMPro.TMP_InputField>();
                rewritingStringList = new List<string>();
                answer = new List<string>();
                charLimit = new List<int>();
                correctAnswerRate = 100;

            }
            public void Reset()
            {
                IsActive = false;
                isOptionSelected = false;
                rewritingInputList = new List<TMPro.TMP_InputField>();
                rewritingStringList = new List<string>();
                answer = new List<string>();
                charLimit = new List<int>();
                correctAnswerRate = 100;
            }
            public QuizComponentData(QuizComponentData data)
            {
                IsActive = data.IsActive;
                isOptionSelected = data.isOptionSelected;
                rewritingInputList = data.rewritingInputList;
                rewritingStringList = data.answer;//rewritingStringList;
                answer = data.answer;
                charLimit = data.charLimit;
                correctAnswerRate = data.correctAnswerRate;
            }

        }

    }
}
