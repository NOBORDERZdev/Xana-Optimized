using System.Collections;
using TMPro;
using UnityEngine;

namespace FightingModule
{
    public class LoadingManager : MonoBehaviour
    {
        public TextMeshProUGUI textField;

        private float duration = 1f;
        private float targetValue = 100f;
        private float currentValue = 0f;

        void Start()
        {
            // Start the coroutine
            StartCoroutine(AnimateTextField());
        }

        IEnumerator AnimateTextField()
        {
            float timer = 0f;
            float startValue = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                currentValue = Mathf.Lerp(startValue, targetValue, timer / duration);
                textField.text = currentValue.ToString("F0") + "%";
                yield return null;
            }

            // Ensure the final value is set correctly
            textField.text = targetValue.ToString("F0") + "%";
        }
    }
}