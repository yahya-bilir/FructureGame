namespace Events
{
    public class OnWeaponsCreated
    {
        public StationaryGunHolderCharacter[] StationaryGunHolderCharacters { get; private set; }

        public OnWeaponsCreated(StationaryGunHolderCharacter[] weapons)
        {
            StationaryGunHolderCharacters =  weapons;
        }
    }
}