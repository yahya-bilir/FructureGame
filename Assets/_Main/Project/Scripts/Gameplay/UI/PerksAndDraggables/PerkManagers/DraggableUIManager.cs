using Cysharp.Threading.Tasks;
using Perks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PerksAndDraggables.PerkManagers
{
    public class DraggableUIManager : ClickableUIManager
    {
        private readonly ClickableActionInfo _clickableActionInfo;
        private readonly Image _mainImageHolder;
        private readonly TextMeshProUGUI _titleTextHolder;
        private readonly TextMeshProUGUI _descHolder;
        private readonly TextMeshProUGUI _circle;

        public DraggableUIManager(ClickableActionInfo clickableActionInfo, Image mainImageHolder, TextMeshProUGUI titleTextHolder, TextMeshProUGUI descHolder, TextMeshProUGUI circle) : base(clickableActionInfo, mainImageHolder, titleTextHolder, descHolder)
        {
            _clickableActionInfo = clickableActionInfo;
            _mainImageHolder = mainImageHolder;
            _titleTextHolder = titleTextHolder;
            _descHolder = descHolder;
            _circle = circle;
            circle.text = ClickableActionInfo.ReadOnlyInfoAreaToFormat;
            AdjustSize().Forget();

        }

        private async UniTask AdjustSize()
        {
            await UniTask.WaitForSeconds(0.1f);
            if (_clickableActionInfo.ShouldResize)
            {
                _mainImageHolder.SetNativeSize();
                _mainImageHolder.transform.localScale = Vector3.one * 0.075f;
            }
        }
    }
}