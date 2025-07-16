using System;
using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IslandSystem
{
    public class IslandCharactersController : IDisposable

    {
        private readonly List<OpeningSection> _openingSections;
        private readonly IEventBus _eventBus;
        private readonly Island _island;
        private readonly bool _isFirstIsland;
        private List<Character> _characters = new();

        public IslandCharactersController(List<OpeningSection> openingSections, IEventBus eventBus, Island island, bool isFirstIsland)
        {
            _openingSections = openingSections;
            _eventBus = eventBus;
            _island = island;
            _isFirstIsland = isFirstIsland;
        }

        public void Initialize()
        {
            DeactivateSections();
            _eventBus.Subscribe<OnCharacterDied>(OnCharacterDied);
            _eventBus.Subscribe<OnCharacterSpawned>(OnCharacterSpawned);
            foreach (var openingSection in _openingSections)
            {
                foreach (var enemy in openingSection.Enemies)
                {
                    _characters.Add(enemy);
                }
            }
        }

        private void OnCharacterDied(OnCharacterDied eventData)
        {
            var character = eventData.Character;
            if(!_characters.Contains(character)) return;
            _characters.Remove(character);
            Debug.Log(_characters.Count);
            if(_characters.Count > 0) return;
            if (_isFirstIsland)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                return;
            }
            _eventBus.Publish(new OnAllIslandEnemiesKilled(_island));
        }

        private void DeactivateSections()
        {
            foreach (var section in _openingSections)
            foreach (var enemy in section.Enemies)
                enemy.gameObject.SetActive(false);
        }

        public async UniTask ActivateSections()
        {
            foreach (var section in _openingSections)
            {
                foreach (var enemy in section.Enemies)
                {
                    enemy.gameObject.SetActive(true);
                    enemy.CharacterVisualEffects.SpawnCharacter();
                }

                await UniTask.WaitForSeconds(0.1f);
            }
        }

        private void OnCharacterSpawned(OnCharacterSpawned eventData)
        {
            if (eventData.SpawnedIsland != _island)
            {
                return;
            }
            _characters.Add(eventData.SpawnedCharacter);
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
            _eventBus.Unsubscribe<OnCharacterSpawned>(OnCharacterSpawned);
        }
    }
}