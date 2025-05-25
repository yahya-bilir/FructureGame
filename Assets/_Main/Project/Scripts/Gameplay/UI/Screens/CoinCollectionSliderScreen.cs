using DataSave.Runtime;
using DG.Tweening;
using EventBusses;
using Events;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.UIComponents.UIToolkit;
using VContainer;

namespace UI.Screens
{
    public class CoinCollectionSliderScreen : UIView
    {
        private VisualElement _slider;
        private Label _coinCountHolder;
        private GameData _gameData;
        private IEventBus _eventBus;
        private float _currentCoinCollectionPerSecond;
        public CoinCollectionSliderScreen(VisualElement rootElement) : base(rootElement)
        {
        }

        [Inject]
        private void Inject(GameData gameData, IEventBus eventBus)
        {
            _gameData = gameData;
            _eventBus = eventBus;
            SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            _eventBus.Subscribe<OnCoinCollectionIncreased>(OnCoinCountIncreased);
            _eventBus.Subscribe<OnCoinCountChanged>(SetSliderText);
        }

        protected override void SetVisualElements()
        {
            _slider = _rootElement.Q("SliderFill");    
            _coinCountHolder = _rootElement.Q<Label>("CollectedCoinText");
            
            Show();
        }

        protected override void RegisterButtonCallbacks()
        {
        }

        private void OnCoinCountIncreased(OnCoinCollectionIncreased eventData)
        {
            _currentCoinCollectionPerSecond = eventData.CoinCollectionPerSeconds;
            StartSliderFillAnimation();
        }

        private void StartSliderFillAnimation()
        {
            DOTween.Kill("CoinCollectionSliderAnimation");
            
            var tween = DOTween.Sequence();
            tween.SetId("CoinCollectionSliderAnimation");
            tween.OnStart(() =>
            {
                _slider.transform.scale = Vector3.zero;
                
            });
            tween.Append(DOVirtual.Float(0, 1, 1 / _currentCoinCollectionPerSecond, (value) =>
            {
                _slider.transform.scale = new Vector3(value, 1, 1);
            }));
            tween.OnStepComplete(() =>
            {
                _gameData.CharacterResource.CoinCount += 1;
            });
            tween.SetEase(Ease.Linear);
            tween.SetLoops(-1, LoopType.Restart);

        }

        private void SetSliderText(OnCoinCountChanged eventData)
        {
            _coinCountHolder.text = eventData.CurrentCoinCount.ToString();
        }

        public override void Dispose()
        {
            base.Dispose();
            _eventBus.Unsubscribe<OnCoinCollectionIncreased>(OnCoinCountIncreased);
            _eventBus.Unsubscribe<OnCoinCountChanged>(SetSliderText);


        }
    }
}