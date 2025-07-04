
using Characters;

namespace Events
{
    public class OnCharacterDeselected
    {
        public Character DeselectedCharacter { get; private set; }
        
        public OnCharacterDeselected(Character deselectedCharacter)
        {
            DeselectedCharacter = deselectedCharacter;
        }
    }
}