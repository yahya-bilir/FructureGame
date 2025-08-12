using System.Collections.Generic;
using Perks.PerkActions;
using PerkSystem;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Scriptable Objects/Gamedatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        [field: SerializeField] public Material DissolveMaterial { get; private set; }
        
        [field: SerializeField] public List<PerkByLevel> PerksByLevel { get; private set; }

        public List<PerkAction> GetPerksForLevel(int level)
        {
            if(level >= PerksByLevel.Count - 1) level =  PerksByLevel.Count - 1;
            foreach (var data in PerksByLevel)
            {
                if (data.Level == level)
                    return data.PerkGroup != null ? data.PerkGroup.GetRandomPerks(3) : null;
            }

            return null;
        }
        

    }
}