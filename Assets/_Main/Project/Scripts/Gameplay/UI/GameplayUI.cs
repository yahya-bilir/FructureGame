using Gameplay.UI.InGameView;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace UI
{
    public class GameplayUI : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _rootElement;
        private IObjectResolver _resolver;
        
        private BaseHealthContainer _baseHealthContainer;
        private PerkView _perkView;

        [SerializeField] private PerkViewData perkViewData;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            _rootElement = _document.rootVisualElement;
            _baseHealthContainer = new BaseHealthContainer(_rootElement);
            _perkView = new PerkView(_rootElement, perkViewData);
        }

        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        private void Start()
        {
            _resolver.Inject(_baseHealthContainer);
        }
        
    }
}