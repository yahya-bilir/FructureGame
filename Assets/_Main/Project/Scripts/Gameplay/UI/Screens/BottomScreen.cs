using Cysharp.Threading.Tasks;
using DataSave;
using DataSave.Runtime;
using DG.Tweening;
using EventBusses;
using Events;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using Utils.UIComponents.UIToolkit;
using VContainer;

namespace UI.Screens
{
    public class BottomScreen : UIView
    {
        private VisualElement _shineElement;
        private VisualElement _imageContainer;
        private Button _enhanceButton;
        private Label _priceLabel;
        private Label _levelLabel;
        private Label _attackSpeedLabel;
        private Label _damageLabel;
        private Label _itemNameLabel;
        
        private bool _increasing = true;
        private GameData _gameData;
        private GameDatabase _gameDatabase;
        private IEventBus _eventBus;
        private int _currentPrice;
        public BottomScreen(VisualElement rootElement) : base(rootElement)
        {
        }

        protected override void SetVisualElements()
        {
            _shineElement = _rootElement.Q("ImageShine");
            _imageContainer = _rootElement.Q("ImageContainer");
            _priceLabel = _rootElement.Q<Label>("EnhanceCoin");
            _itemNameLabel = _rootElement.Q<Label>("ItemNameField");
            _damageLabel = _rootElement.Q<Label>("ATKCalculationText");
            _attackSpeedLabel = _rootElement.Q<Label>("ATKSpeedCalculationText");
            _levelLabel = _rootElement.Q<Label>("LevelText");
            _enhanceButton = _rootElement.Q<Button>("EnhanceButton");
            
            Show();
            //ContinuesShineAnim();
        }
        

        protected override void RegisterButtonCallbacks()
        {
        }
        
        [Inject]
        private void Inject(GameData gameData, GameDatabase gameDatabase, IEventBus eventBus)
        {
            _gameData = gameData;
            _gameDatabase = gameDatabase;
            _eventBus = eventBus;
            SetAfterInjection();
            
        }

        private void SetAfterInjection()
        {
            _eventBus.Subscribe<OnCoinCountChanged>(CheckIfButtonShouldBeEnabled);
            _eventBus.Subscribe<OnWeaponUpgraded>(OnWeaponUpgraded);
            SetPriceAndLabel(_gameData.EnhanceButtonData.ButtonClickedCount);
        }

        private void OnWeaponUpgraded(OnWeaponUpgraded eventData)
        {
            _itemNameLabel.text = $"{Extensions.ColoredText(eventData.Stage.Prefix, eventData.Stage.PrefixColor)} {eventData.ObjectUIIdentifierSo.ObjectName}";
            _attackSpeedLabel.text = eventData.AttackSpeed.ToString("F2");
            _levelLabel.text = $"LV. {eventData.Level.ToString()}";

            var plusDamageText = $"(+{eventData.Damage - 5})";
            _damageLabel.text = $"{eventData.Damage}{Extensions.ColoredText(plusDamageText, new Color(0.572f, 0.847f, 0.337f, 1f))}";
            
        }

        private void SetPriceAndLabel(int buttonLevel)
        {
            var initialPrice = _gameDatabase.EnhanceButtonDatabase.InitialEnhancePrice;
            var incrementPrice = _gameDatabase.EnhanceButtonDatabase.IncrementOnEachUpgrade;
            var multipliedPrice = incrementPrice * buttonLevel;
            _currentPrice = initialPrice + multipliedPrice;
            _priceLabel.text = _currentPrice.ToString();
        }

        private void CheckIfButtonShouldBeEnabled(OnCoinCountChanged eventData)
        {
            SetPriceAndLabel(_gameData.EnhanceButtonData.TemporaryButtonClickedCount);
            _enhanceButton.clickable.clicked -= OnUpgradeButtonPressed;
            
            if (eventData.CurrentCoinCount >= _currentPrice)
            {
                ToolkitUtils.ChangeClasses(_priceLabel, "stat-text", "stat-text-unavailable");
                ToolkitUtils.ChangeClasses(_enhanceButton, "enhance-button-available", "enhance-button-unavailable");

                _enhanceButton.clickable.clicked += OnUpgradeButtonPressed;
            }
            else
            {
                ToolkitUtils.ChangeClasses(_priceLabel, "stat-text-unavailable", "stat-text");
                ToolkitUtils.ChangeClasses(_enhanceButton, "enhance-button-unavailable", "enhance-button-available");
            }
        }

        private void OnUpgradeButtonPressed()
        {
            _gameData.CharacterResource.CoinCount -= _currentPrice;
            _gameData.EnhanceButtonData.TemporaryButtonClickedCount++;
            
            _eventBus.Publish(new OnCoinCountChanged(_gameData.CharacterResource.CoinCount));
            _eventBus.Publish(new OnUpgradeButtonPressed());
        }
        
        public void ContinuesShineAnim()
        {
            DOVirtual.Float(97f, 103f, 0.5f, value =>
                {
                    _shineElement.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(new Length(value, LengthUnit.Percent), new Length(value, LengthUnit.Percent)));
                })
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            
            ScaleUp().Forget();
        }

        private async UniTask ScaleUp()
        {
            var initialImageContainer = _imageContainer.resolvedStyle.width;
            var targetSize = initialImageContainer * 1.2f;

            _imageContainer.experimental.animation
                .Size(new Vector2(targetSize, targetSize), 600)
                //.Ease(t => t * t) // InQuad easing
                .OnCompleted(() =>
                {
                    _imageContainer.experimental.animation
                        .Size(new Vector2(initialImageContainer, initialImageContainer), 600)
                        .Ease(t => 1 - Mathf.Pow(2, -10 * t)); // OutExpo easing
                });

            await UniTask.Yield(); // Asenkron yapıya uygunluk için eklenmiştir
        }

        public override void Dispose()
        {
            base.Dispose();
            _eventBus.Unsubscribe<OnCoinCountChanged>(CheckIfButtonShouldBeEnabled);
            _eventBus.Unsubscribe<OnWeaponUpgraded>(OnWeaponUpgraded);


        }
    }
}