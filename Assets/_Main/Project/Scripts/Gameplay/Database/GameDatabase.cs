using System.Collections.Generic;
using PerkSystem;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Scriptable Objects/Gamedatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        [field: SerializeField] public Material DissolveMaterial { get; private set; }
        
        [SerializeField] private List<PerkByLevel> perksByLevel;

        public List<PerkAction> GetPerksForLevel(int level)
        {
            foreach (var data in perksByLevel)
            {
                if (data.Level == level)
                    return data.PerkGroup != null ? data.PerkGroup.GetRandomPerks(3) : null;
            }

            return null;
        }
        

    }
}