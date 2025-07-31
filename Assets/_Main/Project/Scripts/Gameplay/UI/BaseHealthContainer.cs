using Characters.BaseSystem;
using Database;
using DataSave.Runtime;
using EventBusses;
using UnityEngine.UIElements;
using Utils.UIComponents.UIToolkit;
using VContainer;

namespace UI
{
    public class BaseHealthContainer : UIView
    {
        private Label _healthMax;
        private Label _currentHealth;
        private IEventBus _eventBus;
        private MainBase _mainBase;
        
        public BaseHealthContainer(VisualElement rootElement) : base(rootElement)
        {
        }
        
        [Inject]
        private void Inject(IEventBus eventBus, MainBase mainBase)
        {
            _eventBus = eventBus;
            _mainBase = mainBase;
            SetAfterInjection();

        }

        private void SetAfterInjection()
        {
            _rootElement.userData = _mainBase.CharacterPropertyManager;
        }

        protected override void SetVisualElements()
        {
            _healthMax = _rootElement.Q<Label>("MaxHealth");
            _currentHealth = _rootElement.Q<Label>("CurrentHealth");
            Show();

        }

        protected override void RegisterButtonCallbacks()
        {
            
        }
    }
}