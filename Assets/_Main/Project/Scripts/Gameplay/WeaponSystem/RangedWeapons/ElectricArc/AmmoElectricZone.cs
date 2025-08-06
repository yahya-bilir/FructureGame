using System.Collections.Generic;
using System.Linq;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace WeaponSystem.AmmoSystem
{
    public class AmmoElectricZone : AmmoBase
    {
        [SerializeField] private VisualEffect electricVfx;
        [field: SerializeField] public Collider DetectionCollider { get; private set; }
        [SerializeField] private float damageInterval = 1f;

        private readonly HashSet<Character> _affectedCharacters = new();
        private bool _isActive = false;

        public override void FireAt(Character target)
        {
            if (_isActive) return;
            _isActive = true;

            // Collider aktif
            if (DetectionCollider != null)
                DetectionCollider.enabled = true;

            // VFX başlat
            if (electricVfx != null)
                electricVfx.Play();

            DamageLoop().Forget();
        }

        private async UniTaskVoid DamageLoop()
        {
            while (_isActive)
            {
                foreach (var character in _affectedCharacters.ToList())
                {
                    if (character == null || character.IsCharacterDead) continue;
                    character.CharacterCombatManager.GetDamage(Damage);
                }

                await UniTask.Delay((int)(damageInterval * 1000));
            }
        }

        public void StopArc()
        {
            if (!_isActive) return;
            _isActive = false;

            // Collider kapat
            if (DetectionCollider != null)
                DetectionCollider.enabled = false;

            _affectedCharacters.Clear();
            
            // VFX durdur
            if (electricVfx != null)
            {
                electricVfx.Stop();
                gameObject.SetActive(false);
            }
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering)
        {
            if (!other.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Character character)) return;
            if (character == ConnectedCombatManager.Character) return;
            if (character.IsCharacterDead) return;

            if (isEntering)
                _affectedCharacters.Add(character);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Character character)) return;

            _affectedCharacters.Remove(character);
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering) { }
    }
}
