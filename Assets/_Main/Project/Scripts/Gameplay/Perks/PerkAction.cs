using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace PerkSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Perk Action")]
    public abstract class PerkAction : ScriptableObject
    {
        [Header("Perk Görsel Bilgileri")]
        public string PerkName;
        [TextArea] public string Description;
        public Sprite Icon;
        public Sprite Background;
        public Color NameColor = Color.white;

        public static event Action<PerkAction> OnPerkExecuted;

        public virtual async UniTask Execute()
        {
            Debug.Log($"Perk Executed: {PerkName}");
            OnPerkExecuted?.Invoke(this);
            await UniTask.CompletedTask;
        }
    }
}