// gen version: 2
// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// HOMA Belly
// -----------------------
using System;

public static class DVR
{
#if HOMA_BELLY && UNITY_IOS
    
#elif HOMA_BELLY && UNITY_ANDROID
    
#else
    
#endif
}

#if !HOMA_BELLY
namespace HomaGames.Geryon
{
    public class Observable<T>
    {
        public T Value { get; internal set; }

        public Observable()
        {
        }

        public Observable(T value)
        {
        }

        public static implicit operator T(Observable<T> obj)
        {
            return default;
        }

        public void Subscribe(Action<T> onValueChanged, bool notifyOnSubscribe = true)
        {
        }

        public void Unsubscribe(Action<T> callback)
        {
        }
    }
}
#endif