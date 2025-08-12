using System;
using Database;
using EventBusses;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

// Eğer PerkAction burada tanımlıysa

namespace Perks
{
    public class XPManager : MonoBehaviour
    {
        [SerializeField] private int startingLevel;
        [SerializeField] private int baseXpNeeded = 100;
        [SerializeField] private float xpGrowthRate = 1.2f;
        private GameDatabase _gameDatabase;
        private IEventBus _eventBus;

        private int _currentLevel;
        private int _currentXP;
        private int _xpToNextLevel;
        private IObjectResolver _objectResolver;

        [Inject]
        private void Inject(GameDatabase gameDatabase, IEventBus eventBus, IObjectResolver objectResolver)
        {
            _gameDatabase = gameDatabase;
            _eventBus = eventBus;
            _objectResolver = objectResolver;
        }
        
        private void Awake()
        {
            _currentLevel = startingLevel;
            _currentXP = 0;
            _xpToNextLevel = CalculateXpNeededForLevel(_currentLevel);
        }

        private void Start()
        {
            foreach (var perkByLevel in _gameDatabase.PerksByLevel)
            {
                foreach (var perk in perkByLevel.PerkGroup.Perks)
                {
                    _objectResolver.Inject(perk);
                }
            }
        }

        [Button]
        public void AddXP(int amount)
        {
            _currentXP += amount;

            while (_currentXP >= _xpToNextLevel)
            {
                _currentXP -= _xpToNextLevel;
                LevelUp();
            }
        }

        [Button]
        private void LevelUp()
        {
            _xpToNextLevel = CalculateXpNeededForLevel(_currentLevel);

            var perks = _gameDatabase.GetPerksForLevel(_currentLevel);
            if (perks != null && perks.Count > 0)
            {
                _eventBus.Publish(new OnLevelUpgraded(perks));
            }

            _currentLevel++;
            Debug.Log($"LEVEL UP! New Level: {_currentLevel}");
        }

        private int CalculateXpNeededForLevel(int level)
        {
            return Mathf.RoundToInt(baseXpNeeded * Mathf.Pow(xpGrowthRate, level - 1));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LevelUp();
            }
        }
    }
}