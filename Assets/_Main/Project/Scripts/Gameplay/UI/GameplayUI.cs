using DataSave.Runtime;
using EventBusses;
using Events;
using Sirenix.OdinInspector;
using UI.Screens;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace UI
{
    public class GameplayUI : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _rootElement;
        private BottomScreen _bottomScreen;
        private CoinCollectionSliderScreen _coinCollectionSliderScreen;
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
            _coinCollectionSliderScreen = new CoinCollectionSliderScreen(_rootElement);
            _resolver.Inject(_bottomScreen);
            _resolver.Inject(_coinCollectionSliderScreen);
            _characterResource.CoinCount = 1000;
            _eventBus.Publish(new OnCoinCollectionIncreased(1));
        }

        [Button]
        private void IncreaseAmount(float amount)
        {
            _eventBus.Publish(new OnCoinCollectionIncreased(amount));

        }
        
    }
}