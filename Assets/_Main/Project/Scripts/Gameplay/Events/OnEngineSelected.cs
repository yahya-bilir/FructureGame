using Trains;

namespace Events
{
    public class OnEngineSelected
    {
        public TrainEngine Engine { get; }
        public int SystemIndex { get; }
        public int WagonCount { get; private set; }

        public OnEngineSelected(TrainEngine engine, int systemIndex, int wagonCount)
        {
            Engine = engine;
            SystemIndex = systemIndex;
            WagonCount = wagonCount;
        }
    }

}