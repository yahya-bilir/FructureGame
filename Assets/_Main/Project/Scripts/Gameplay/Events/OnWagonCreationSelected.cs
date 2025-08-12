using Trains;

namespace Events
{
    public class OnWagonCreationSelected
    {
        public TrainEngine TrainEngine { get; private set; }
        public int WagonCountToSpawn { get; private set; }

        public OnWagonCreationSelected(TrainEngine trainEngine, int wagonCountToSpawn)
        {
            TrainEngine = trainEngine;
            WagonCountToSpawn = wagonCountToSpawn;
        }
    }
}