using UnityEngine;

namespace Perks.PerkActions
{
    public abstract class PerkAction : ScriptableObject
    {
        [field: SerializeField] public string PerkName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite Background { get; private set; }
        [field: SerializeField] public Color NameColor { get; private set; } = Color.white;

        public abstract void Execute();
    }
}