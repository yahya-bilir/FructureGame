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
        private IObjectResolver _resolver;

        [Inject]
        private void Inject(GameDatabase database, IEventBus eventBus, IObjectResolver resolver)
        {
            _transformPathDatabase = database.CharacterTransformPathDatabase;
            _eventBus = eventBus;
            _resolver = resolver;
        }
        
        public void TryUpgradeCharacters(List<Character> characters)
        {
            List<(Character current, Character nextPrefab)> upgradePairs = new();

            foreach (var character in characters)
            {
                var currentId = character.CharacterPropertiesSo?.EntityId;
                if (string.IsNullOrEmpty(currentId))
                    continue;

                foreach (var path in _transformPathDatabase.CharacterUpgradePaths)
                {
                    var sequence = path.SequentialTransformableCharacters;
                    if (sequence == null || sequence.Count < 2)
                        continue;

                    var index = sequence.FindIndex(c =>
                        c != null &&
                        c.CharacterPropertiesSo != null &&
                        c.CharacterPropertiesSo.EntityId == currentId);

                    if (index == -1 || index >= sequence.Count - 1)
                        continue;

                    var nextCharacterPrefab = sequence[index + 1];
                    if (nextCharacterPrefab == null)
                        continue;

                    upgradePairs.Add((character, nextCharacterPrefab));
                    break;
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
            _resolver.Inject(newCharacter);
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