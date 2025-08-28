using AI.Base.Interfaces;
using TMPro;
using WeaponSystem.RangedWeapons;

namespace AI.StationaryGunHolderStates
{
    public class WaitingForWeaponToBeLoaded : IState

    {
        private readonly RangedWeaponWithExternalAmmo rangedWeapon;
        private readonly TextMeshPro aiText;

        public WaitingForWeaponToBeLoaded(RangedWeaponWithExternalAmmo rangedWeapon, TextMeshPro aiText)
        {
            this.rangedWeapon = rangedWeapon;
            this.aiText = aiText;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            aiText.text = "Waiting For Weapon To Be Loaded";
        }

        public void OnExit()
        {
            
        }
    }
}