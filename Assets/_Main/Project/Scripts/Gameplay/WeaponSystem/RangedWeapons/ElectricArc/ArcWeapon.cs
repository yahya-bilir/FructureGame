using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Characters;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using WeaponSystem.AmmoSystem;

public class ArcWeapon : RangedWeapon
{
    [SerializeField] private AmmoElectricZone arcZone;
    [SerializeField] private List<Transform> positions;
    private List<Vector3> _cachedPositions;
    
    private bool _canShoot = true;
    private bool _isFiring = false;

    private void Awake()
    {
        _cachedPositions = new List<Vector3>();
        foreach (var position in positions)
        {
            _cachedPositions.Add(position.localPosition);
            position.localPosition = Vector3.zero;
        }
    }

    public override void Shoot(Character character)
    {
        if (!_canShoot || _isFiring) return;

        _isFiring = true;
        _canShoot = false;

        if (!arcZone.gameObject.activeInHierarchy)
        {
            arcZone.gameObject.SetActive(true);
            arcZone.SetOwnerAndColor(this, _currentColor);
            arcZone.Initialize(ConnectedCombatManager, Damage);
        }

        arcZone.FireAt(character);
        ArcCycle(character.transform).Forget();
    }

    private async UniTaskVoid ArcCycle(Transform character)
    {
        // 3 saniye aktif
        await PositionSetter(character);
        await UniTask.Delay(3000);
        StopFiring();

        // 2 saniye bekleme
        await UniTask.Delay(2000);
        _canShoot = true;
    }

    private async UniTask PositionSetter(Transform character)
    {
        // Başlangıçta collider'ı positions[^1]'a yapıştır
        if (arcZone != null && arcZone.DetectionCollider is BoxCollider box)
        {
            Vector3 size = box.size;
            Vector3 center = box.center;

            size.z = 0f;
            center.z = 0f;

            box.size = size;
            box.center = center;
        }

        // İlk segmentleri yerine koy
        for (var i = 0; i < positions.Count - 1; i++)
        {
            var position = positions[i];
            position.DOLocalMove(_cachedPositions[i], 0.5f);
        }

        // Uç noktayı hedefe taşı ve collider'ı eş zamanlı uzat
        positions[^1]
            .DOMove(character.position, 0.5f)
            .OnUpdate(() =>
            {
                if (arcZone != null && arcZone.DetectionCollider is BoxCollider boxCollider)
                {
                    float distance = Vector3.Distance(positions[0].position, positions[^1].position);
                    Vector3 size = boxCollider.size;
                    Vector3 center = boxCollider.center;

                    size.z = distance;
                    center.z = distance / 2f;

                    boxCollider.size = size;
                    boxCollider.center = center;
                }
            });

        await UniTask.WaitForSeconds(0.5f);
    }



    public void StopFiring()
    {
        if (!_isFiring) return;

        foreach (var pos in positions)
        {
            pos.localPosition = Vector3.zero;
        }
        
        _isFiring = false;
        
        
        arcZone.StopArc();
    }
}