using System.Collections.Generic;
using EventBusses;
using UnityEngine;
using VContainer;

namespace UI.PerksAndDraggables
{
    public class PerkManager : MonoBehaviour
    {
        [SerializeField] protected GameObject holder;
        
        protected IEventBus EventBus;
        protected readonly List<ClickableAndConnectedTransform> ClickableAndConnectedTransforms = new();

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            EventBus = eventBus;
        }
    }
}