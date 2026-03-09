using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FadeOnOverlap : MonoBehaviour
{
    [SerializeField] private float fadeAlpha = 0.3f; // 透明时的Alpha值
    [SerializeField] private float fadeDuration = 0.2f; // 渐变时间

    private SpriteRenderer spriteRenderer;
    private List<GameObject> overlappingObjects = new List<GameObject>(); // 记录重叠对象

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            overlappingObjects.Add(other.gameObject);
            StartCoroutine(FadeSprite(fadeAlpha));
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            overlappingObjects.Remove(other.gameObject);
            if (overlappingObjects.Count == 0)
            {
                StartCoroutine(FadeSprite(1f)); // 恢复不透明
            }
        }
    }
    private IEnumerator FadeSprite(float targetAlpha)
    {
        Color initialColor = spriteRenderer.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            spriteRenderer.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }
}