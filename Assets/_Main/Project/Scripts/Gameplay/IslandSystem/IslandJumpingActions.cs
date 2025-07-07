using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events.IslandEvents;
using Factories;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace IslandSystem
{
    public class IslandJumpingActions : IDisposable
    {
        private readonly Dictionary<Character, Vector2> _characterTargetPositions = new();
        private readonly Transform _formationAnchor;
        private readonly Island _island;

        private readonly Vector2[] _jumpAreaCorners = new Vector2[4];
        private readonly Vector2[,] _jumpAreaEdges = new Vector2[4, 2];
        private readonly Collider2D _jumpingPosCollider;
        private readonly Dictionary<int, float> _layerRadius = new();
        private readonly Dictionary<int, float> _layerYOffsets = new();
        private readonly Collider2D _placingPosCollider;
        private IEventBus _eventBus;
        private bool _jumpAreaCached;
        private EnemyFactory _playerCharacters;
        public bool JumpingCanStart { get; private set; }
        private readonly List<Vector2> _jumpingTakenPoints = new();

        public IslandJumpingActions(Collider2D jumpingPosCollider, Transform formationAnchor, Island island,
            Collider2D placingPosCollider)
        {
            _jumpingPosCollider = jumpingPosCollider;
            _formationAnchor = formationAnchor;
            _island = island;
            _placingPosCollider = placingPosCollider;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        }

        [Inject]
        private void Inject(IEventBus eventBus, EnemyManager enemyManager)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);
            _playerCharacters = enemyManager.PlayerArmyFactory;
        }

        public void CacheJumpArea()
        {
            if (_jumpingPosCollider == null)
            {
                Debug.LogWarning("JumpingPosCollider is null!");
                return;
            }

            var bounds = _jumpingPosCollider.bounds;

            _jumpAreaCorners[0] = new Vector2(bounds.min.x, bounds.min.y);
            _jumpAreaCorners[1] = new Vector2(bounds.max.x, bounds.min.y);
            _jumpAreaCorners[2] = new Vector2(bounds.max.x, bounds.max.y);
            _jumpAreaCorners[3] = new Vector2(bounds.min.x, bounds.max.y);

            if (_jumpAreaEdges != null)
            {
                _jumpAreaEdges[0, 0] = _jumpAreaCorners[0];
                _jumpAreaEdges[0, 1] = _jumpAreaCorners[1];
                _jumpAreaEdges[1, 0] = _jumpAreaCorners[1];
                _jumpAreaEdges[1, 1] = _jumpAreaCorners[2];
                _jumpAreaEdges[2, 0] = _jumpAreaCorners[2];
                _jumpAreaEdges[2, 1] = _jumpAreaCorners[3];
                _jumpAreaEdges[3, 0] = _jumpAreaCorners[3];
                _jumpAreaEdges[3, 1] = _jumpAreaCorners[0];
            }

            _jumpAreaCached = true;
        }

        public async UniTask WaitForCharacterJumps()
        {
            while (_playerCharacters.SpawnedEnemies.Any(i => i.CharacterIslandController.IsJumping))
            {
                await UniTask.Yield();
            }

            JumpingCanStart = false;
        }        
        
        public async UniTask WaitForCharactersToGetIntoJumpingPosition()
        {
            while (_playerCharacters.SpawnedEnemies.Any(i => i.CharacterIslandController.WalkingToJumpingPosition))
            {
                await UniTask.Yield();
            }

            JumpingCanStart = true;
        }

        public async UniTask MakeCharacterJump()
        {
            foreach (var character in _playerCharacters.SpawnedEnemies)
            {
                character.CharacterIslandController.SetCanJumpEnabled();
                await UniTask.WaitForSeconds(0.05f);
            }
        }
        
        

        public Vector2 GetJumpPosition(Vector2 startPos)
        {
            if (!_jumpAreaCached)
            {
                Debug.LogWarning("Jump area not cached! Caching now.");
                CacheJumpArea();
            }

            var closestPoint = startPos;
            var minDistance = float.MaxValue;

            for (var i = 0; i < 4; i++)
            {
                var a = _jumpAreaEdges[i, 0];
                var b = _jumpAreaEdges[i, 1];
                var projection = ProjectPointOnLineSegment(a, b, startPos);

                var dist = Vector2.Distance(startPos, projection);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestPoint = projection;
                }
            }

            return closestPoint;
        }

// private void GenerateFormationPositions(List<Character> playerCharacters)
// {
//     _characterTargetPositions.Clear();
//
//     var groupedByOrder = playerCharacters
//         .GroupBy(c => c.CharacterDataHolder.Order)
//         .ToList();
//
//     var uniqueOrders = groupedByOrder.Select(g => g.Key).OrderBy(o => o).ToList();
//     int totalLayers = uniqueOrders.Count;
//
//     float baseYOffset = -2.0f;
//     float layerSpacing = 1.0f; // Katmanlar arası mesafe
//
//     float totalHeight = totalLayers * layerSpacing;
//     float startY = baseYOffset - (totalHeight / 2f) + layerSpacing / 2f;
//
//     foreach (var group in groupedByOrder)
//     {
//         int orderValue = group.Key;
//         int orderIndex = uniqueOrders.IndexOf(orderValue);
//
//         int count = group.Count();
//         float yOffset = startY - orderIndex * layerSpacing;
//
//         // === GRID + ARC PARAMETRELER ===
//         int colCount = Mathf.CeilToInt(Mathf.Sqrt(count));
//         int rowCount = Mathf.CeilToInt((float)count / colCount);
//
//         float colSpacing = 0.7f + orderIndex * 0.1f;  // X ekseni yayılma
//         float rowSpacing = 0.5f;                      // Y ekseni alt satır aralığı
//
//         // Hafif yay: Arc açısı
//         float arcAngle = 90f;  // 90 derece yay
//         float arcRadius = 2.0f + orderIndex * 0.5f;
//
//         int i = 0;
//         for (int row = 0; row < rowCount; row++)
//         {
//             for (int col = 0; col < colCount; col++)
//             {
//                 if (i >= count) break;
//
//                 // === ARC SPREAD ===
//                 float arcStep = arcAngle / Mathf.Max(colCount - 1, 1);
//                 float startAngle = -arcAngle / 2f;
//
//                 float angle = startAngle + col * arcStep;
//                 float angleRad = angle * Mathf.Deg2Rad;
//
//                 // X: yay + grid yayılma
//                 float x = Mathf.Sin(angleRad) * arcRadius + (col - (colCount - 1) / 2f) * colSpacing;
//
//                 // Y: katman offset + row spacing
//                 float y = yOffset - (row * rowSpacing) + Mathf.Cos(angleRad) * arcRadius * 0.1f;
//
//                 // Hafif random scatter:
//                 x += Random.Range(-0.05f, 0.05f);
//                 y += Random.Range(-0.05f, 0.05f);
//
//                 Vector2 localOffset = new Vector2(x, y);
//                 Vector2 finalPos = (Vector2)_formationAnchor.position + localOffset;
//
//                 _characterTargetPositions[group.ElementAt(i)] = finalPos;
//
//                 i++;
//             }
//         }
//     }
// }

        private void GenerateFormationPositions(List<Character> playerCharacters)
        {
            _characterTargetPositions.Clear();

            if (_placingPosCollider == null)
            {
                Debug.LogError("_placingPosCollider is null!");
                return;
            }

            var bounds = _placingPosCollider.bounds;

            var minSpacing = 1f;
            var maxSpacing = 1.5f;
            var spacing = Mathf.Lerp(minSpacing, maxSpacing, Mathf.Clamp01(playerCharacters.Count / 50f));

            List<Vector2> placedPoints = new();
            var triesPerCharacter = 30;

            foreach (var character in playerCharacters)
            {
                Vector2 startPos = character.transform.position;

                // 1) Karşı hedef tahmini
                var dir = ((Vector2)_formationAnchor.position - startPos).normalized;
                var dist = 2.0f; // parametrelenebilir
                var candidate = startPos + dir * dist;

                // Collider sınırına uydur
                candidate.x = Mathf.Clamp(candidate.x, bounds.min.x, bounds.max.x);
                candidate.y = Mathf.Clamp(candidate.y, bounds.min.y, bounds.max.y);

                var overlaps = false;
                foreach (var p in placedPoints)
                    if (Vector2.Distance(candidate, p) < spacing)
                    {
                        overlaps = true;
                        break;
                    }

                if (!overlaps)
                {
                    _characterTargetPositions[character] = candidate;
                    placedPoints.Add(candidate);
                    continue;
                }

                // 2) Poisson-style sapmalar
                var placed = false;
                for (var attempt = 0; attempt < triesPerCharacter; attempt++)
                {
                    var offset = Random.insideUnitCircle * spacing; // spacing radius kadar dairede yayıl
                    var altCandidate = candidate + offset;

                    altCandidate.x = Mathf.Clamp(altCandidate.x, bounds.min.x, bounds.max.x);
                    altCandidate.y = Mathf.Clamp(altCandidate.y, bounds.min.y, bounds.max.y);

                    overlaps = false;
                    foreach (var p in placedPoints)
                        if (Vector2.Distance(altCandidate, p) < spacing)
                        {
                            overlaps = true;
                            break;
                        }

                    if (!overlaps)
                    {
                        _characterTargetPositions[character] = altCandidate;
                        placedPoints.Add(altCandidate);
                        placed = true;
                        break;
                    }
                }

                // 3) Fallback random
                if (!placed)
                {
                    var fallback = new Vector2(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y)
                    );
                    _characterTargetPositions[character] = fallback;
                    placedPoints.Add(fallback);
                }
            }
        }


        /// <summary>
        ///     Belirli bir karakterin landing pozisyonunu döner.
        /// </summary>
        public Vector2 GetLandingPosForCharacter(Character c)
        {
            return _characterTargetPositions.TryGetValue(c, out var pos) ? pos : _formationAnchor.position;
        }

        private Vector2 ProjectPointOnLineSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            var ab = b - a;
            var abSquared = Vector2.Dot(ab, ab);
            if (abSquared == 0) return a;

            var ap = p - a;
            var t = Mathf.Clamp01(Vector2.Dot(ap, ab) / abSquared);
            return a + t * ab;
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            if (eventData.SelectedIsland != _island) return;

            GenerateFormationPositions(_playerCharacters.SpawnedEnemies);
        }
    }
}