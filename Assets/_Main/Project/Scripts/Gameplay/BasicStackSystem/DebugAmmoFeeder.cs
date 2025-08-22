using BasicStackSystem;
using UnityEngine;
using WeaponSystem.AmmoSystem;

public class DebugAmmoSpawner : MonoBehaviour
{
    [SerializeField] private StackableAmmo ammoPrefab;
    [SerializeField] private BasicStack targetStack;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 1f;

    private float _timer;

    private void Update()
    {
        if (ammoPrefab == null || targetStack == null) return;

        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;

        if (!targetStack.IsThereAnySpace) return;

        var pos = spawnPoint != null ? spawnPoint.position : targetStack.transform.position;
        var rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        var ammo = Instantiate(ammoPrefab, pos, rot);
        ammo.gameObject.SetActive(true);

        // AmmoBase zaten IStackable ise:
        targetStack.TryAddFromOutside(ammo);
    }
}