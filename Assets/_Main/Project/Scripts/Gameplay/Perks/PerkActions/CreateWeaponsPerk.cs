using Characters.StationaryGunHolders;
using Events;
using UnityEngine;

namespace Perks.PerkActions
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Create Weapons Perk")]

    public class CreateWeaponsPerk : PerkAction
    {
        [SerializeField] private StationaryGunHolderCharacter[] stationaryGunHolderCharacters;
        
        public override void Execute()
        {
            EventBus.Publish(new OnWeaponsCreated(stationaryGunHolderCharacters));
        }
    }
}