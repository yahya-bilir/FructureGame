using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CommonComponents
{
    public class TimerWithText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private bool timeDecreasingEnabled;
        [SerializeField] private float intervalBetweenEachSimulation;
        public UnityEvent OnTimerReachedZero;
        
        private float _remainingSeconds;

        private void OnEnable()
        {
            Start();
        }

        private void Start()
        {
            _remainingSeconds = intervalBetweenEachSimulation;
            timeDecreasingEnabled = true;
            SetTimer();
        }

        private void SetTimer()
        {
            var intRemainingSeconds = (int)Mathf.Round(_remainingSeconds);
            var shouldPrintZero = intRemainingSeconds % 60 < 10 ? "0" : "";
            timerText.text = $"{intRemainingSeconds / 60}:{shouldPrintZero}{intRemainingSeconds % 60}";

            if (_remainingSeconds == 0)
            {
                OnTimerReachedZero?.Invoke();
                timeDecreasingEnabled = false;
            }
        }

        private void Update()
        {
            if(!timeDecreasingEnabled) return;
            _remainingSeconds -= Time.deltaTime;
            SetTimer();
        }

        public void SetDecreasingEnablity(bool isEnabled) => timeDecreasingEnabled = isEnabled;

    }
}