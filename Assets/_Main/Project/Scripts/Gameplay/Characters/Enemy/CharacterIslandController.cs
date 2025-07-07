using IslandSystem;
using UnityEngine;

namespace Characters.Enemy
{
    public class CharacterIslandController
    {
        private readonly Character _character;
        public Island NextIsland { get; private set; }
        public Island PreviousIsland { get; private set; }
        public bool IsJumping { get; private set; }
        public bool CanJump { get; private set; }
        public bool WalkingToJumpingPosition { get; private set; }
        public CharacterIslandController(Character character)
        {
            _character = character;
        }

        public void SetNextIsland(Island nextIsland)
        {
            NextIsland = nextIsland;
        }        
        
        public void SetPreviousIsland(Island previousIsland)
        {
            PreviousIsland = previousIsland;
        }

        public Vector2 GetNextIslandLandingPosition()
        {
            return NextIsland != null ? NextIsland.JumpingActions.GetLandingPosForCharacter(_character) : (Vector2)_character.transform.position;
        }

        public Vector2 GetJumpingPosition()
        {
            return PreviousIsland != null ? PreviousIsland.JumpingActions.GetJumpPosition(_character.transform.position) : (Vector2)_character.transform.position;
        }
        
        
        public void StartWalkingToJumpingPosition() => WalkingToJumpingPosition = true;
        public void StopWalkingToJumpingPosition() => WalkingToJumpingPosition = false;
        public void StartJumpingActions() => IsJumping = true;
        public void StopJumping() => IsJumping = false;        
        public void SetCanJumpEnabled() => CanJump = true;
        public void SetCanJumpDisabled() => CanJump = false;
    }
}