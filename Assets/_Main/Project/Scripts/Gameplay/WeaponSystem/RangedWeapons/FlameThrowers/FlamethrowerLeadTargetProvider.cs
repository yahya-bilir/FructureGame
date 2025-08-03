using Characters;

public static class FlamethrowerLeadTargetProvider
{
    public static Character CurrentTarget { get; private set; }

    public static void UpdateTarget(Character character)
    {
        CurrentTarget = character;
    }
}
