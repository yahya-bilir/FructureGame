using Characters;
using Characters.Enemy;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.AmmoSystem.CustomAmmos
{
    public class VerticalAmmo : AmmoHomingBase
    {
        [SerializeField] private float jumpHeight = 5f;
        [SerializeField] private float duration = 1f;

        private GameObject _targetPartObj;

        protected override void PlayTween(Character target)
        {
            if (!(target is EnemyBehaviour eb)) return;

            var data = eb.EnemyDestructionManager.GetMeshColliderToAttack();
            if (data == null || data.ParentGameObjectOfColliders == null) return;

            _targetPartObj = data.ParentGameObjectOfColliders;

            Vector3 targetPosition = _targetPartObj.transform.position;

            JumpTowards(targetPosition).Forget();
        }

        private async UniTaskVoid JumpTowards(Vector3 targetPos)
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 🎯 Lerp + Parabola
                Vector3 flat = Vector3.Lerp(startPos, targetPos, t);
                float height = Mathf.Sin(Mathf.PI * t) * jumpHeight;
                transform.position = flat + Vector3.up * height;

                await UniTask.Yield();
            }

            transform.position = targetPos;
            OnTweenComplete();
        }

        protected override void OnTweenComplete()
        {
            if (_targetCharacter == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (_targetCharacter is EnemyBehaviour eb && _targetPartObj != null)
            {
                eb.EnemyDestructionManager.DestroyPartIfPossible(_targetPartObj);
            }

            _targetCharacter.CharacterCombatManager.GetDamage(Damage, DamageTypes.Normal, _targetPartObj);

            HitVisualEffect();
            gameObject.SetActive(false);
            _ownerWeapon.OnAmmoDestroyed(this);
            Video.Events.OnBallClashed?.Invoke(transform);
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering) { }
    }
}
