using Trains;

namespace Events
{
    public class OnEngineSelected
    {
        public TrainEngine Engine { get; }
        public int SystemIndex { get; }

        public OnEngineSelected(TrainEngine engine, int systemIndex)
        {
            Engine = engine;
            SystemIndex = systemIndex;
        }
    }

}