namespace AI.Base.Interfaces
{
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
    }
}