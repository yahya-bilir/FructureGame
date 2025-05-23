using DataSave.Runtime;
using EventBusses;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace UI.Screens
{
    public class GameplayUI : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _rootElement;
        private BottomScreen _bottomScreen;
        private IObjectResolver _resolver;
        private IEventBus _eventBus;
        private CharacterResource _characterResource;
        private void Awake()
        {
            _document = GetComponent<UIDocument>();
        }

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus, GameData gameData)
        {
            _resolver = resolver;
            _eventBus = eventBus;
            _characterResource = gameData.CharacterResource;
        }
        
        private void Start()
        {
            _rootElement = _document.rootVisualElement;
            _bottomScreen = new BottomScreen(_rootElement);
            _resolver.Inject(_bottomScreen);
            
            _eventBus.Publish(new OnCoinCountChanged(_characterResource.CoinCount));
        }
    }
}