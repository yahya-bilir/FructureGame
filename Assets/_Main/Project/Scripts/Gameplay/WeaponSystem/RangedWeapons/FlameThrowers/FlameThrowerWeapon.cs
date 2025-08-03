using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WeaponSystem.AmmoSystem;

public class FlamethrowerWeapon : RangedWeapon
{
    private AmmoFlamethrowerZone _ammoZone;
    private bool _canShoot = true;
    private bool _isFiring = false;

    public override void Shoot(Character character)
    {
        if (!_canShoot || _isFiring) return;

        _isFiring = true;
        _canShoot = false;

        if (_ammoZone == null)
        {
            if (_projectilePool.Count == 0)
                ExpandPool();

            _ammoZone = _projectilePool.Dequeue() as AmmoFlamethrowerZone;
            if (_ammoZone == null)
            {
                Debug.LogError("AmmoFlamethrowerZone not found in pool or wrong type.");
                return;
            }

            _ammoZone.transform.SetParent(projectileCreationPoint);
            _ammoZone.transform.localPosition = Vector3.zero;
            _ammoZone.transform.localRotation = Quaternion.identity;
            _ammoZone.SetOwnerAndColor(this, _currentColor);
            _ammoZone.Initialize(ConnectedCombatManager, Damage);
            _ammoZone.gameObject.SetActive(true);
        }

        _ammoZone.FireAt(character);
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

        _ammoZone?.StopBurning();
    }
}