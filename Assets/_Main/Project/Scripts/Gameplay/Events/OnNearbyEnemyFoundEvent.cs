using Characters;
using UnityEngine;

namespace Events
{
    public class OnNearbyEnemyFoundEvent
    {
        public Character FoundEnemy { get; }
        
        public OnNearbyEnemyFoundEvent(Character enemy)
        {
            FoundEnemy = enemy;
        }
    }
}