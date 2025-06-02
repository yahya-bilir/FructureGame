using System;
using EventBusses;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Characters.Player
{
    public class PlayerDeathController : IDisposable
    {
        private readonly PlayerController _playerController;
        private IEventBus _eventBus;

        public PlayerDeathController(PlayerController playerController)
        {
            _playerController = playerController;
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnCharacterDiedEvent>(OnCharacterDied);
        }

        private void OnCharacterDied(OnCharacterDiedEvent eventData)
        {
            if (eventData.Character != _playerController)
            {
                Debug.Log("Not player character");
                return;
            }
            Debug.Log("Player character");
            SceneManager.LoadScene(0);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnCharacterDiedEvent>(OnCharacterDied);
        }
    }
}