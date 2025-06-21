using System.Collections.Generic;
using Database;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Characters.Transforming
{
    public class CharacterTransformManager
    {
        private CharacterTransformPathDatabase _transformPathDatabase;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(GameDatabase database, IEventBus eventBus)
        {
            _transformPathDatabase = database.CharacterTransformPathDatabase;
            _eventBus = eventBus;
        }
        
        public void TryUpgradeCharacters(List<Character> characters)
        {
            List<(Character current, Character nextPrefab)> upgradePairs = new();

            foreach (var character in characters)
            {
                var currentId = character.CharacterPropertiesSo.EntityId;

                foreach (var path in _transformPathDatabase.CharacterUpgradePaths)
                {
                    var sequence = path.SequentialTransformableCharacters;
                    var index = sequence.FindIndex(c => c.CharacterPropertiesSo.EntityId == currentId);

                    if (index != -1 && index < sequence.Count - 1)
                    {
                        var nextCharacterPrefab = sequence[index + 1];
                        upgradePairs.Add((character, nextCharacterPrefab));
                        break;
                    }
                }
            }

            UpgradeCharacters(upgradePairs);
        }

        public void TryUpgradeCharacter(Character character)
        {
            TryUpgradeCharacters(new List<Character> { character });
        }
        
        public void UpgradeCharacters(List<(Character current, Character nextPrefab)> upgradePairs)
        {
            foreach (var (current, next) in upgradePairs)
            {
                UpgradeCharacter(current, next);
            }
        }

        public void UpgradeCharacter(Character currentCharacter, Character nextCharacterPrefab)
        {
            var position = currentCharacter.transform.position;
            var rotation = currentCharacter.transform.rotation;

            var newCharacter = Object.Instantiate(nextCharacterPrefab, position, rotation);
            newCharacter.InitializeOnSpawn(currentCharacter.Faction);

            _eventBus.Publish(new OnCharacterUpgraded(currentCharacter, newCharacter));

            CleanupCharacter(currentCharacter);
        }

        private void CleanupCharacter(Character character)
        {
            // Bağlı sistemlerde çözülmesi gereken işler varsa burada yapılır
            // Örn: _eventBus.Publish(new CharacterDestroyedEvent(character));
            Object.Destroy(character.gameObject);
        }

    }
}