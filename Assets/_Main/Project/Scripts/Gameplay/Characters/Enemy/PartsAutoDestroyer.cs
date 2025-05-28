using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PartsAutoDestroyer : MonoBehaviour
{
    [SerializeField] private float delayBeforeFade = 0.5f;
    [SerializeField] private float fadeDuration = 1.2f;

    private void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"Part {name} üzerinde SpriteRenderer bulunamadı.");
            yield break;
        }

        // Bekleme
        yield return new WaitForSeconds(delayBeforeFade);

        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}