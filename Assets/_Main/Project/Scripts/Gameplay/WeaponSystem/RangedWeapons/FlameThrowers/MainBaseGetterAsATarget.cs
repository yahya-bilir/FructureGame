using Characters;
using Characters.BaseSystem;
using VContainer;

public class MainBaseGetterAsATarget
{
    public Character CurrentTarget { get; private set; }
    private MainBase _mainBase;

    [Inject]
    private void Inject(MainBase mainBase)
    {
        _mainBase = mainBase;
    }

    public void UpdateTarget(Character character)
    {
        // if (character != null && !character.IsCharacterDead)
        //     CurrentTarget = character;
        CurrentTarget = _mainBase;
    }

    public bool HasValidTarget =>
        CurrentTarget != null && !CurrentTarget.IsCharacterDead;
}