using System.Collections.Generic;
using CollectionSystem;
using Perks.PerkActions;
using UnityEngine;
using WeaponSystem.AmmoSystem;
using WeaponSystem.AmmoSystem.Logic;

namespace Database
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Scriptable Objects/GameDatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        [field: SerializeField] public Material DissolveMaterial { get; private set; }
        [field: SerializeField] public List<PerkByLevel> PerksByLevel { get; private set; }
        [field: SerializeField] public CollectionSystemDataHolder CollectionSystemDataHolder { get; private set; }

        [field: SerializeField] public List<AmmoPrefabEntry> AmmoPrefabs { get; private set; }

        public List<PerkAction> GetPerksForLevel(int level)
        {
            if (level >= PerksByLevel.Count - 1)
                level = PerksByLevel.Count - 1;

            foreach (var data in PerksByLevel)
            {
                if (data.Level == level)
                    return data.PerkGroup != null ? data.PerkGroup.GetRandomPerks(3) : null;
            }

            return null;
        }

        public AmmoBase GetAmmoPrefab(AmmoLogicType logic, ElementType element)
        {
            foreach (var entry in AmmoPrefabs)
            {
                if (entry.LogicType == logic && entry.ElementType == element)
                    return entry.Prefab;
            }

            Debug.LogWarning($"[GameDatabase] No matching prefab for Logic: {logic}, Element: {element}");
            return null;
        }

        [System.Serializable]
        public class AmmoPrefabEntry
        {
            public AmmoLogicType LogicType;
            public ElementType ElementType;
            public AmmoBase Prefab;
        }
    }
}