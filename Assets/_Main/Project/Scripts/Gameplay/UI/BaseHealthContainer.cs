using Characters.BaseSystem;
using EventBusses;
using Events;
using PropertySystem;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.UIComponents.UIToolkit;
using VContainer;

namespace UI
{
    public class BaseHealthContainer : UIView
    {
        private Label _healthMax;
        private Label _currentHealth;
        private VisualElement _slider;

        private IEventBus _eventBus;
        private CharacterPropertyManager _mainBasePropertyManager;
        private MainBase _mainBase;

        public BaseHealthContainer(VisualElement rootElement) : base(rootElement)
        {
        }
        
        [Inject]
        private void Inject(IEventBus eventBus, MainBase mainBase)
        {
            // _eventBus = eventBus;
            // _mainBase = mainBase;
            //
            // _mainBasePropertyManager = mainBase.CharacterPropertyManager;
            // SetAfterInjection();
        }

        private void SetAfterInjection()
        {
            // _eventBus.Subscribe<OnBaseGotAttacked>(OnBaseGotAttacked);
            // OnBaseGotAttacked(null);
        }


        protected override void SetVisualElements()
        {
            // _healthMax = _rootElement.Q<Label>("MaxHealth");
            // _currentHealth = _rootElement.Q<Label>("CurrentHealth");
            // _slider = _rootElement.Q("SliderFill");    
            //
            // Show();
        }

        protected override void RegisterButtonCallbacks()
        {
            
        }

        private void OnBaseGotAttacked(OnBaseGotAttacked eventData)
        {
            // float currentHealth = _mainBasePropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue;
            // float maxHealth = _mainBasePropertyManager.GetProperty(PropertyQuery.MaxHealth).TemporaryValue;
            //
            // // Slider fill için scale.x değerini ayarla
            // float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
            // _slider.transform.scale = new UnityEngine.Vector3(fillAmount, 1, 1);
            //
            // // Label'lara integer olarak yaz
            // _currentHealth.text = ((int)currentHealth).ToString();
            // _healthMax.text = ((int)maxHealth).ToString();
        }


        public override void Dispose()
        {
            //_eventBus.Unsubscribe<OnBaseGotAttacked>(OnBaseGotAttacked);
        }
    }
}