using UnityEngine;
using System.Collections;

public class EnemyFadeAway : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] AudioClip fadeSound;

    private bool isEFading = false;
    private void OnTriggerEnter(Collider other)
    {
        if(!isEFading && other.CompareTag("Player"))
        {
            isEFading = true;
            AudioSource.PlayClipAtPoint(fadeSound, transform.position);
            StartCoroutine(FadeOutCoroutine());
        }
    }
    private IEnumerator FadeOutCoroutine()
    {
        float startAlpha = 1f;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            material.SetFloat("_Alpha", alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
