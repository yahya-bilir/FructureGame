using System.Collections.Generic;
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
using WeaponSystem;

namespace UI.Screens
{
    public class BottomScreen : UIView
    {
        private VisualElement _shineElement;
        private VisualElement _imageContainer;
        private VisualElement _backgroundImage;
        private VisualElement _innerBackground;
        private List<VisualElement> _stars;


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
        private bool _isAnimationPlaying;
        private float _initialImageContainerSizeX;
        private float _initialImageContainerSizeY;
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
            _backgroundImage = _rootElement.Q("ImageColorBG");
            _innerBackground = _rootElement.Q("ImageMainColor");

            _stars = _rootElement.Query<VisualElement>(name: "InnerStar").ToList();
            Show();
            _rootElement.RegisterCallback<GeometryChangedEvent>(InitializeOnStart);
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

        private void InitializeOnStart(GeometryChangedEvent evt)
        {
            _imageContainer.style.height = _imageContainer.style.width;
            _initialImageContainerSizeX = _imageContainer.resolvedStyle.width;
            _initialImageContainerSizeY = _imageContainer.resolvedStyle.height;
        }

        private void SetAfterInjection()
        {
            _eventBus.Subscribe<OnCoinCountChanged>(CheckIfButtonShouldBeEnabled);
            _eventBus.Subscribe<OnWeaponUpgraded>(OnWeaponUpgraded);
            SetPriceAndLabel(_gameData.EnhanceButtonData.ButtonClickedCount);
            ContinuesShineAnim();
        }

        private void OnWeaponUpgraded(OnWeaponUpgraded eventData)
        {
            _itemNameLabel.text = $"{Extensions.ColoredText(eventData.Stage.Prefix, eventData.Stage.PrefixColor)} {eventData.ObjectUIIdentifierSo.ObjectName}";
            _attackSpeedLabel.text = eventData.AttackSpeed.ToString("F2");
            _levelLabel.text = $"LV. {eventData.Level.ToString()}";

            var plusDamageText = $"(+{eventData.Damage - 5})";
            _damageLabel.text = $"{eventData.Damage}{Extensions.ColoredText(plusDamageText, new Color(0.572f, 0.847f, 0.337f, 1f))}";
            ScaleUp(eventData.Stage);
            SetPriceAndLabel(_gameData.EnhanceButtonData.TemporaryButtonClickedCount);
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
            _eventBus.Publish(new OnUpgradeButtonPressed());
        }
        
        private void ContinuesShineAnim()
        {
            DOVirtual.Float(97f, 103f, 0.5f, value =>
                {
                    _shineElement.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(new Length(value, LengthUnit.Percent), new Length(value, LengthUnit.Percent)));
                })
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void ScaleUp(WeaponStagesSO stage)
        {
            DOTween.Kill("ScaleUpTween");

            var initialImageContainer = _initialImageContainerSizeX;
            var targetSize = initialImageContainer * 1.2f;

            var tween = DOTween.Sequence();
            tween.SetId("ScaleUpTween");
            tween.SetAutoKill(false);
            tween.OnKill(() =>
            {
                _imageContainer.style.width = _initialImageContainerSizeX;
                _imageContainer.style.height = _initialImageContainerSizeY;

            });
            tween.Append(DOVirtual.Float(initialImageContainer, targetSize, 0.6f, value =>
            {
                _imageContainer.style.width = new Length(value, LengthUnit.Pixel);
                _imageContainer.style.height = new Length(value, LengthUnit.Pixel);
            }));
            tween.AppendCallback(() =>
            {
                _backgroundImage.style.backgroundImage = new StyleBackground(stage.BackgroundBorderSprite);
                _innerBackground.style.backgroundImage = new StyleBackground(stage.BackgroundInnerSprite);
                
                foreach (var star in _stars)
                {
                    ToolkitUtils.ChangeClasses(star, "", "star-unshown");
                }
            
                for (var i = _stars.Count - 1; i >= stage.StarCount; i--)
                {
                    var star = _stars[i];
                    ToolkitUtils.ChangeClasses(star, "star-unshown", "");
                }
            });
            tween.Append(DOVirtual.Float(targetSize, initialImageContainer, 0.6f, value =>
            {
                _imageContainer.style.width = new Length(value, LengthUnit.Pixel);
                _imageContainer.style.height = new Length(value, LengthUnit.Pixel);
            }));
            tween.OnComplete(() =>
            {

            
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            _eventBus.Unsubscribe<OnCoinCountChanged>(CheckIfButtonShouldBeEnabled);
            _eventBus.Unsubscribe<OnWeaponUpgraded>(OnWeaponUpgraded);
        }
    }
}