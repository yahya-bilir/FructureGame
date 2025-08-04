using Characters;

public class FlamethrowerLeadTargetProvider
{
    public Character CurrentTarget { get; private set; }

    public void UpdateTarget(Character character)
    {
        if (character != null && !character.IsCharacterDead)
            CurrentTarget = character;
    }

    public bool HasValidTarget =>
        CurrentTarget != null && !CurrentTarget.IsCharacterDead;
}