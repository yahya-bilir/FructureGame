using System;
using UnityEngine;

namespace Video
{
    public static class Events
    {
        public static Action<Transform> OnBallSpawned;
        public static Action<Transform> OnBallClashed;
    }
}