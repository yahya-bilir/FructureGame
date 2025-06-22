using UnityEngine.TextCore.Text;

namespace Events
{
    public class OnCharacterSelected
    {
        public Character SelectedCharacter { get; private set; }
        
        public OnCharacterSelected(Character selectedCharacter)
        {
            SelectedCharacter = selectedCharacter;
        }
    }
}