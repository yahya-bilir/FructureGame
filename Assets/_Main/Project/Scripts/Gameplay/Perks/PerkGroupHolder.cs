using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PerkSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Perk Group")]
    public class PerkGroupHolder : ScriptableObject
    {
        public List<PerkAction> Perks;

        public List<PerkAction> GetRandomPerks(int count)
        {
            return Perks.OrderBy(x => Random.value).Take(count).ToList();
        }
    }
}