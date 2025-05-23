using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Screens
{
    public class GameplayUI : MonoBehaviour
    {
        private UIDocument _document;
        private VisualElement _rootElement;
        private BottomScreen _bottomScreen;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            _rootElement = _document.rootVisualElement;
            _bottomScreen = new BottomScreen(_rootElement);
        }

        [Button]
        private void DoAnim()
        {
            _bottomScreen.ContinuesShineAnim();   
        }
    }
}