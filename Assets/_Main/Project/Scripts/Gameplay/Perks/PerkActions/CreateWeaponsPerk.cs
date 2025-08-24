using Events;
using UnityEngine;

namespace Perks.PerkActions
{
    public class CreateWeaponsPerk : PerkAction
    {
        [SerializeField] private StationaryGunHolderCharacter[] stationaryGunHolderCharacters;
        
        public override void Execute()
        {
            EventBus.Publish(new OnWeaponsCreated(stationaryGunHolderCharacters));
        }
    }
}