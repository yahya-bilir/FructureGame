using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace CommonComponents
{
    public abstract class GenericPanelActions : MonoBehaviour
    {
        [SerializeField] protected Transform mainPanel;

        [SerializeField] protected UnityEvent onPanelEnabled;
        [SerializeField] protected UnityEvent onPanelDisabled;
        
        [Button]
        public abstract void PanelSystemEnabled();

        [Button]
        public abstract void CloseAllPanels();
    }
}
