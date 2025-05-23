using UnityEngine;

namespace WeaponSystem
{
    public static class StaticCounter
    {
        private static int _counter = 0;
        
        public static void LogCounter()
        {
            Debug.Log($"Counter: {_counter}");
            _counter++;
        }
    }
}