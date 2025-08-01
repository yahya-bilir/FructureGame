using System;
using System.Collections.Generic;
using Database;
using UnityEngine;
using PerkSystem; // Eğer PerkAction burada tanımlıysa

namespace GameProgression
{
    public class XPManager : MonoBehaviour
    {
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private int baseXpNeeded = 100;
        [SerializeField] private float xpGrowthRate = 1.2f;
        [SerializeField] private GameDatabase gameDatabase;
        [SerializeField] private Gameplay.UI.InGameView.PerkView perkView;

        private int _currentLevel;
        private int _currentXP;
        private int _xpToNextLevel;

        private void Awake()
        {
            _currentLevel = startingLevel;
            _currentXP = 0;
            _xpToNextLevel = CalculateXpNeededForLevel(_currentLevel);
        }

        public void AddXP(int amount)
        {
            _currentXP += amount;

            while (_currentXP >= _xpToNextLevel)
            {
                _currentXP -= _xpToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            _currentLevel++;
            _xpToNextLevel = CalculateXpNeededForLevel(_currentLevel);
            Debug.Log($"LEVEL UP! New Level: {_currentLevel}");

            var perks = gameDatabase.GetPerksForLevel(_currentLevel);
            if (perks != null && perks.Count > 0)
            {
                perkView.CreatePerks(perks);
            }
        }

        private int CalculateXpNeededForLevel(int level)
        {
            return Mathf.RoundToInt(baseXpNeeded * Mathf.Pow(xpGrowthRate, level - 1));
        }
    }
}