using UI.PerksAndDraggables;
using UnityEngine;
using VContainer;

namespace Perks.Base
{
    public class DraggablePerk : Draggable
    {
        protected BottomPerkManager BottomPerkManager;
        
        [Inject]
        private void Inject(BottomPerkManager bottomPerkManager)
        {
            BottomPerkManager = bottomPerkManager;
        }

        public override void OnClickedSuccessfully()
        {
            //TODO send it to bottom half
        }
    }
}