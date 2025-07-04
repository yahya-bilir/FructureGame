using Perks;
using Perks.Base;
using TMPro;
using UnityEngine.UI;

namespace UI.PerksAndDraggables
{
    public class ClickableUIManager
    {
        private readonly ClickableActionInfo _clickableActionInfo;
        private readonly Image _mainImageHolder;
        private readonly TextMeshProUGUI _titleTextHolder;
        private readonly TextMeshProUGUI _descHolder;

        public ClickableUIManager(ClickableActionInfo clickableActionInfo, Image mainImageHolder,
            TextMeshProUGUI titleTextHolder, TextMeshProUGUI descHolder)
        {
            _clickableActionInfo = clickableActionInfo;
            _mainImageHolder = mainImageHolder;
            _titleTextHolder = titleTextHolder;
            _descHolder = descHolder;
        }

        public void Initialize()
        {
            _clickableActionInfo.Info = _clickableActionInfo.ReadOnlyInfoAreaToFormat;
            
            _mainImageHolder.sprite = _clickableActionInfo.Icon;
            _titleTextHolder.text = _clickableActionInfo.Name;
            _descHolder.text = _clickableActionInfo.Info;
        }
        
    }
}