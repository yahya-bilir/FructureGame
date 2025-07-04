using UnityEngine;

namespace Perks
{
    [CreateAssetMenu(fileName = "PerkViewInfo",
        menuName = "Scriptable Objects/Perks/Perk View Info")]
    public class ClickableActionInfo : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        
        [field: SerializeField] public string ReadOnlyInfoAreaToFormat { get; private set; }
        public string Info { get; set; }
    }
}