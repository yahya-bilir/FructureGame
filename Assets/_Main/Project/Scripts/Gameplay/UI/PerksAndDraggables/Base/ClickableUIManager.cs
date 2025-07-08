using Perks;
using TMPro;
using UnityEngine.UI;

namespace UI.PerksAndDraggables
{
    public class ClickableUIManager
    {
        protected readonly ClickableActionInfo ClickableActionInfo;
        private readonly Image _mainImageHolder;
        private readonly TextMeshProUGUI _titleTextHolder;
        private readonly TextMeshProUGUI _descHolder;

        public ClickableUIManager(ClickableActionInfo clickableActionInfo, Image mainImageHolder,
            TextMeshProUGUI titleTextHolder, TextMeshProUGUI descHolder)
        {
            ClickableActionInfo = clickableActionInfo;
            _mainImageHolder = mainImageHolder;
            _titleTextHolder = titleTextHolder;
            _descHolder = descHolder;
        }

        public void Initialize()
        {
            ClickableActionInfo.Info = ClickableActionInfo.ReadOnlyInfoAreaToFormat;
            
            _mainImageHolder.sprite = ClickableActionInfo.Icon;
            _titleTextHolder.text = ClickableActionInfo.Name;
            _descHolder.text = ClickableActionInfo.Info;
        }
    }
}