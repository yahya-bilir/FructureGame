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
        private readonly List<Vector2> _jumpingTakenPoints = new();
        private readonly Dictionary<int, float> _layerRadius = new();
        private readonly Dictionary<int, float> _layerYOffsets = new();
        private readonly Collider2D _placingPosCollider;
        private IEventBus _eventBus;
        private bool _jumpAreaCached;
        private EnemyFactory _playerCharacters;

        public IslandJumpingActions(Collider2D jumpingPosCollider, Transform formationAnchor, Island island,
            Collider2D placingPosCollider)
        {
            _jumpingPosCollider = jumpingPosCollider;
            _formationAnchor = formationAnchor;
            _island = island;
            _placingPosCollider = placingPosCollider;
        }

        public bool JumpingCanStart { get; private set; }

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
                await UniTask.Yield();

            JumpingCanStart = false;
        }

        public async UniTask WaitForCharactersToGetIntoJumpingPosition()
        {
            while (_playerCharacters.SpawnedEnemies.Any(i => i.CharacterIslandController.WalkingToJumpingPosition))
                await UniTask.Yield();

            JumpingCanStart = true;
        }

        public async UniTask MakeCharacterJump()
        {
            foreach (var character in _playerCharacters.SpawnedEnemies)
            {
                character.CharacterIslandController.SetCanJumpEnabled();
                await UniTask.WaitForSeconds(0.01f);
            }
        }


        public Vector2 GetJumpPosition(Vector2 startPos)
        {
            if (!_jumpAreaCached)
            {
                Debug.LogWarning("Jump area not cached! Caching now.");
                CacheJumpArea();
            }

            var bounds = _jumpingPosCollider.bounds;

            var minSpacing = 0.5f; // minimum mesafe
            var distForward = 1.5f; // ne kadar ileri yürüsün

            // 1) Dümdüz ileri yön
            var dir = ((Vector2)_formationAnchor.position - startPos).normalized;
            dir.x = 0; // sadece Y aksı

            var candidate = startPos + dir * distForward;

            // Clamp to collider
            candidate.x = Mathf.Clamp(candidate.x, bounds.min.x, bounds.max.x);
            candidate.y = Mathf.Clamp(candidate.y, bounds.min.y, bounds.max.y);

            // Spacing check
            var overlaps = false;
            foreach (var p in _jumpingTakenPoints)
                if (Vector2.Distance(candidate, p) < minSpacing)
                {
                    overlaps = true;
                    break;
                }

            if (!overlaps)
            {
                _jumpingTakenPoints.Add(candidate);
                return candidate;
            }

            // 2) Alternatif: Y ekseninde kaydırma
            var tries = 10;
            for (var attempt = 0; attempt < tries; attempt++)
            {
                var yOffset = Random.Range(-0.5f, 0.5f); // +/- Y kayması
                var altCandidate = candidate + new Vector2(0, yOffset);

                altCandidate.x = Mathf.Clamp(altCandidate.x, bounds.min.x, bounds.max.x);
                altCandidate.y = Mathf.Clamp(altCandidate.y, bounds.min.y, bounds.max.y);

                overlaps = false;
                foreach (var p in _jumpingTakenPoints)
                    if (Vector2.Distance(altCandidate, p) < minSpacing)
                    {
                        overlaps = true;
                        break;
                    }

                if (!overlaps)
                {
                    _jumpingTakenPoints.Add(altCandidate);
                    return altCandidate;
                }
            }

            // 3) Alternatif: X aksı sapması
            for (var attempt = 0; attempt < tries; attempt++)
            {
                var xOffset = Random.Range(-0.3f, 0.3f);
                var altCandidate = candidate + new Vector2(xOffset, 0);

                altCandidate.x = Mathf.Clamp(altCandidate.x, bounds.min.x, bounds.max.x);
                altCandidate.y = Mathf.Clamp(altCandidate.y, bounds.min.y, bounds.max.y);

                overlaps = false;
                foreach (var p in _jumpingTakenPoints)
                    if (Vector2.Distance(altCandidate, p) < minSpacing)
                    {
                        overlaps = true;
                        break;
                    }

                if (!overlaps)
                {
                    _jumpingTakenPoints.Add(altCandidate);
                    return altCandidate;
                }
            }

            // Fallback
            Debug.LogWarning("No unique jump start found, fallback to anchor.");
            return _formationAnchor.position;
        }


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