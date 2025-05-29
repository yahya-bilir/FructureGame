using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PartsAutoDestroyer : MonoBehaviour
{
    [SerializeField] private float delayBeforeMove = 0.5f;
    [SerializeField] private float jumpDuration = 0.6f;
    [SerializeField] private float jumpPower = 1.2f;

    public void StartFade(Transform playerTrf)
    {
        StartCoroutine(MoveTowardPlayer(playerTrf));
    }

    private IEnumerator MoveTowardPlayer(Transform playerTrf)
    {
        yield return new WaitForSeconds(delayBeforeMove);

        if (playerTrf == null)
        {
            Debug.LogWarning("Player transform referansı eksik.");
            yield break;
        }

        transform.parent = playerTrf;

        // Hedef yerel pozisyon (örnek: yukarı zıplıyor gibi olsun)
        Vector3 targetLocalPosition = Vector3.up * 0.5f;

        transform.DOLocalJump(
            targetLocalPosition,
            jumpPower,
            numJumps: 1,
            duration: jumpDuration
        ).SetEase(Ease.InCubic);

        yield return new WaitForSeconds(jumpDuration);

        Destroy(gameObject);
    }
}