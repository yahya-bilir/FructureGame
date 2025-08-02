using PerkSystem;
using UnityEngine;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Create Fire Train Perk")]

    public class CreateFireTrainPerk : PerkAction
    {
        public override void Execute()
        {
            Debug.Log("Create Fire Train Perk");
        }
    }
}