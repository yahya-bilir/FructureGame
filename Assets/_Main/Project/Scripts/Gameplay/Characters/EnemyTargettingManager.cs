using System.Collections.Generic;
using System.Linq;
using Factions;
using UnityEngine;

namespace Characters
{
    public class EnemyTargetingManager
    {
        private readonly Dictionary<Character, HashSet<Character>> _enemyToAttackers = new();

        public void RegisterTarget(Character attacker, Character enemy)
        {
            UnregisterTarget(attacker);
        
            if (!_enemyToAttackers.ContainsKey(enemy))
                _enemyToAttackers[enemy] = new HashSet<Character>();

            _enemyToAttackers[enemy].Add(attacker);
        }

        public void UnregisterTarget(Character attacker)
        {
            foreach (var kvp in _enemyToAttackers)
            {
                if (kvp.Value.Contains(attacker))
                {
                    kvp.Value.Remove(attacker);
                    break;
                }
            }
        }

        public Character FindBestEnemy(Vector3 origin, Faction faction, float searchRadius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, searchRadius, LayerMask.GetMask("AI"));
        
            var candidates = hits
                .Select(c => c.GetComponent<Character>())
                .Where(c => c != null && c.Faction != faction && !c.IsCharacterDead)
                .ToList();

            // En az saldırılan düşmanları bul
            var best = candidates
                .OrderBy(c => _enemyToAttackers.ContainsKey(c) ? _enemyToAttackers[c].Count : 0)
                .ThenBy(c => Vector2.Distance(origin, c.transform.position))
                .FirstOrDefault();

            return best;
        }
    }
}