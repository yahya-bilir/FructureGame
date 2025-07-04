using UI.PerksAndDraggables;
using UI.PerksAndDraggables.PerkManagers;
using UnityEngine;
using VContainer;

namespace Perks.Base
{
    public class DraggablePerk : Draggable
    {
        protected BottomPerkManager BottomPerkManager;
        protected MiddlePerkManager MiddlePerkManager;

        [Inject]
        private void Inject(BottomPerkManager bottomPerkManager, MiddlePerkManager middlePerkManager)
        {
            BottomPerkManager = bottomPerkManager;
            MiddlePerkManager = middlePerkManager;
        }
        
        public override void OnClickedSuccessfully()
        {
            MiddlePerkManager.RemoveClickableFromList(this);
            BottomPerkManager.TransferDraggableToBottom(this);
        }
    }
}