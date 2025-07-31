using Trains;

namespace Events
{
    public class OnEngineSelected
    {
        public TrainEngine Engine { get; }

        public OnEngineSelected(TrainEngine engine)
        {
            Engine = engine;
        }
    }
}