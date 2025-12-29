using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonFader : MonoBehaviour
{
    public Graphic[] graphicsToFade;
    public Image fadePanel;
    public float fadeDuration = 1f;

    public void FadeAndTriggerScene()
    {
        StartCoroutine(FadeBoth());
    }

    IEnumerator FadeBoth()
    {
        float t = 0f;
        Color[] originalColors = new Color[graphicsToFade.Length];
        for (int i = 0; i < graphicsToFade.Length; i++)
            originalColors[i] = graphicsToFade[i].color;

        Color panelColor = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float pct = Mathf.Clamp01(t / fadeDuration);

            for (int i = 0; i < graphicsToFade.Length; i++)
            {
                Color c = originalColors[i];
                c.a = c.a * (1f - pct);
                graphicsToFade[i].color = c;
            }

            panelColor.a = pct;
            fadePanel.color = panelColor;

            yield return null;
        }

        for (int i = 0; i < graphicsToFade.Length; i++)
        {
            Color c = originalColors[i];
            c.a = 0f;
            graphicsToFade[i].color = c;
        }

        panelColor.a = 1f;
        fadePanel.color = panelColor;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}
