using System;
using EventBusses;
using Events;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Initialization
{
    public class SceneManagement : IStartable, IDisposable
    {
        private readonly IEventBus _eventBus;

        public SceneManagement(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Start()
        {
            _eventBus.Subscribe<OnBaseDied>(OnBaseDied);
        }

        private void OnBaseDied(OnBaseDied eventData)
        {
            SceneManager.LoadScene(0);
        }
        public void Dispose()
        {
            _eventBus.Unsubscribe<OnBaseDied>(OnBaseDied);
        }
    }
}