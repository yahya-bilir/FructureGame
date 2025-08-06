using System;
using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WeaponSystem.AmmoSystem;
public class FlamethrowerWeapon : RangedWeapon
{
    [SerializeField] private List<FlamethrowerOutput> flameOutputs;

    private bool _canShoot = true;
    private bool _isFiring = false;

    public override void Shoot(Character character)
    {
        if (!_canShoot || _isFiring) return;

        _isFiring = true;
        _canShoot = false;

        // Listedeki tüm çıkışlardan ateş et
        foreach (var output in flameOutputs)
        {
            var zone = output.FlameZone;
            var target = output.TargetCharacter;

            if (!zone.gameObject.activeInHierarchy)
            {
                zone.gameObject.SetActive(true);
                zone.SetOwnerAndColor(this, _currentColor);
                zone.Initialize(ConnectedCombatManager, Damage);
            }

            zone.FireAt(target);
        }

        FlameCycle().Forget();
    }

    private async UniTaskVoid FlameCycle()
    {
        // 🔥 3 saniye aktif ateş
        await UniTask.Delay(3000);
        StopFiring();

        // 🧊 2 saniye cooldown
        await UniTask.Delay(2000);
        _canShoot = true;
    }

    public void StopFiring()
    {
        if (!_isFiring) return;
        _isFiring = false;

        foreach (var output in flameOutputs)
        {
            output.FlameZone.StopBurning();
            //output.FlameZone.gameObject.SetActive(false);
        }
    }
}


[Serializable]
public struct FlamethrowerOutput
{
    [field: SerializeField] public AmmoFlamethrowerZone FlameZone { get; private set; }
    [field: SerializeField] public Character TargetCharacter { get; private set; }
}