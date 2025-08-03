using Characters;
using UnityEngine;
using WeaponSystem.AmmoSystem;

public class FlamethrowerWeapon : RangedWeapon
{
    private AmmoFlamethrowerZone _ammoZone;
    private bool _isFiring;

    public override void Shoot(Character character)
    {
        if (_isFiring) return;
        _isFiring = true;

        if (_ammoZone == null)
        {
            if (_projectilePool.Count == 0)
                ExpandPool();

            _ammoZone = _projectilePool.Dequeue() as AmmoFlamethrowerZone;
            Debug.Log("AmmoFlamethrowerZone is null");
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
    }

    public void StopFiring()
    {
        if (!_isFiring || _ammoZone == null) return;
        _isFiring = false;
        _ammoZone.StopBurning();
    }
}