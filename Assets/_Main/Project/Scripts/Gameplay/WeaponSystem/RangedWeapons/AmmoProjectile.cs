using System;
using System.Threading;
using _Main.Project.Scripts.Utils;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeaponSystem.RangedWeapons
{
    public class AmmoProjectile : TriggerWeapon
{
    private Rigidbody2D _rigidbody;
    private float _speed;
    private RangedWeapon _ownerWeapon;
    private bool _hasReturnedToPool = false;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        var so = ObjectUIIdentifierSo as AmmoProjectileSO;
        _speed = so.Speed;
    }

    public void SetOwnerAndColor(RangedWeapon owner, Color color)
    {
        _ownerWeapon = owner;
        _hasReturnedToPool = false;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        modelRenderer.material.SetColor("_OuterOutlineColor", color);
        var trailRenderer = GetComponent<TrailRenderer>();
        color.a /= 2;
        trailRenderer.startColor = color;
        trailRenderer.endColor = color;
    }

    protected override void TryProcessTrigger(Collider2D other, bool isEntering)
    {
        if (!isEntering || !other.CompareTag(Tags.Enemy)) return;
        if (!other.TryGetComponent(out Character character)) return;
        if (character == ConnectedCombatManager.Character) return;

        character.CharacterCombatManager.GetDamage(Damage);
        DisableAndEnqueue();
    }

    public void SendProjectileToDirection(Vector2 direction)
    {
        _rigidbody.gravityScale = 0;
        direction.Normalize();
        _rigidbody.linearVelocity = direction * _speed;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        AutoDisableAfterTime(_cts.Token).Forget();
    }

    private async UniTaskVoid AutoDisableAfterTime(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(5000, cancellationToken: token);
            DisableAndEnqueue();
        }
        catch (OperationCanceledException)
        {
            // ignore cancellation
        }
    }

    private void DisableAndEnqueue()
    {
        if (_hasReturnedToPool) return;
        _hasReturnedToPool = true;
        _cts?.Cancel();
        gameObject.SetActive(false);
        _ownerWeapon.ReturnProjectileToPool(this);
    }
}
}