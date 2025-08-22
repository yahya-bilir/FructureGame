using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WeaponSystem.AmmoSystem;
using WeaponSystem.RangedWeapons;

public class RocketLauncher : RangedWeaponWithAmmoPool
{
    [SerializeField] private Transform[] projectileCreationPoints;
    [SerializeField] private float rocketFireInterval = 0.15f; // roketler arası zaman

    public override void Shoot(Character character)
    {
        if (character == null || character.IsCharacterDead) return;
        LaunchRocketSequence(character).Forget();
    }

    private async UniTaskVoid LaunchRocketSequence(Character target)
    {
        foreach (var point in projectileCreationPoints)
        {
            if (_projectilePool.Count == 0)
                ExpandPool();

            var ammo = _projectilePool.Dequeue();
            if (ammo == null)
            {
                Debug.LogError("Ammo is null");
                continue;
            }

            ammo.transform.SetParent(null);
            ammo.transform.position = point.position;
            ammo.transform.rotation = point.rotation;
            ammo.gameObject.SetActive(true);

            ammo.SetOwnerAndColor(this, _currentColor);
            ammo.Initialize(ConnectedCombatManager, Damage);
            ammo.FireAt(target);

            await UniTask.Delay((int)(rocketFireInterval * 1000));
        }
    }
}