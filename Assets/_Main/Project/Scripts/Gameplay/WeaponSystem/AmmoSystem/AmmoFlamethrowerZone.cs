using System.Collections.Generic;
using System.Linq;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class AmmoFlamethrowerZone : AmmoBase
    {
        [SerializeField] private ParticleSystem flameParticle;
        [SerializeField] private float damageInterval = 1f;
        [SerializeField] private float particleKillRadius = 0.5f; // çarpışma konumuna yakınlık toleransı

        private readonly HashSet<Character> _burningCharacters = new();
        private bool _isBurning = false;

        private ParticleSystem.Particle[] _particleBuffer;

        public override void FireAt(Character target)
        {
            if (_isBurning) return;
            _isBurning = true;

            if (!flameParticle.isPlaying)
                flameParticle.Play();

            DamageLoop().Forget();
        }

        private async UniTaskVoid DamageLoop()
        {
            while (_isBurning)
            {
                foreach (var character in _burningCharacters.ToList())
                {
                    if (character == null || character.IsCharacterDead) continue;
                    character.CharacterCombatManager.GetDamage(Damage);
                }

                await UniTask.Delay((int)(damageInterval * 1000));
            }
        }

        public void StopBurning()
        {
            if (!_isBurning) return;
            _isBurning = false;

            if (flameParticle.isPlaying)
                flameParticle.Stop();

            _burningCharacters.Clear();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!other.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Character character)) return;
            if (character == ConnectedCombatManager.Character) return;

            _burningCharacters.Add(character);

            // 🔥 Çarpışan partikülleri silmek için:
            KillParticlesNear(other.transform.position);
        }

        private void KillParticlesNear(Vector3 collisionPos)
        {
            int maxParticles = flameParticle.main.maxParticles;

            _particleBuffer ??= new ParticleSystem.Particle[maxParticles];

            int count = flameParticle.GetParticles(_particleBuffer);

            for (int i = 0; i < count; i++)
            {
                float dist = Vector3.Distance(_particleBuffer[i].position, collisionPos);
                if (dist < particleKillRadius)
                {
                    _particleBuffer[i].remainingLifetime = 0f; // yok et
                }
            }

            flameParticle.SetParticles(_particleBuffer, count);
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering) { }
        protected override void TryProcessTrigger(Collider2D other, bool isEntering) { }
    }
}
