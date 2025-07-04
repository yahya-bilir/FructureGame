using System;
using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;

namespace IslandSystem
{
    public class IslandCharactersController : IDisposable

    {
        private readonly List<OpeningSection> _openingSections;
        private readonly IEventBus _eventBus;
        private readonly Island _island;
        private List<Character> _characters = new();

        public IslandCharactersController(List<OpeningSection> openingSections, IEventBus eventBus, Island island)
        {
            _openingSections = openingSections;
            _eventBus = eventBus;
            _island = island;
        }

        public void Initialize()
        {
            DeactivateSections();
            _eventBus.Subscribe<OnCharacterDied>(OnCharacterDied);
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
            
            if(_characters.Count > 0) return;
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

                await UniTask.WaitForSeconds(0.25f);
            }
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);
        }
    }
}