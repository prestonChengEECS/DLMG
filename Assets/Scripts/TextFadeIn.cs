using UnityEngine;
using System.Collections;
using TMPro;

public class TextFadeIn : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float defaultFadeDuration = 0.1f;
    public float fastFadeDuration = 0.05f;
    public float slowFadeDuration = 0.3f;
    public float periodPause = 0.8f;
    public float commaPause = 0.3f;
    public float exclamationPause = 1f;
    public float questionPause = 0.8f;
    public float colonPause = 0.8f;
    public float waitTime = 0f;
    public float fadeOutDuration = 0.5f;
    public float fadeOutDelay = 1f;
    public AudioSource audioSource;
    public AudioClip keyPressSound;
    public AudioClip spacePressSound;
    public Color textColor;

    void Start()
    {
        StartCoroutine(FadeInText(textComponent));
    }

    IEnumerator FadeInText(TextMeshProUGUI textComponent)
    {
        textComponent.ForceMeshUpdate();
        int totalChars = textComponent.textInfo.characterCount;
        string text = textComponent.text;

        yield return new WaitForSeconds(waitTime);

        Color32 baseColor32 = textColor;

        for (int i = 0; i < totalChars; i++)
        {
            // handle space sound before skipping invisible characters
            if (i < text.Length && text[i] == ' ')
            {
                if (audioSource != null && spacePressSound != null)
                    audioSource.PlayOneShot(spacePressSound);
                continue;
            }

            TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // play key sound after character fades in
            if (audioSource != null && keyPressSound != null)
                audioSource.PlayOneShot(keyPressSound);

            float elapsed = 0f;
            float fadeDuration = defaultFadeDuration;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 255, elapsed / fadeDuration);

                Color32[] colors = textComponent.textInfo.meshInfo[meshIndex].colors32;
                colors[vertexIndex] = new Color32(baseColor32.r, baseColor32.g, baseColor32.b, (byte)alpha);
                colors[vertexIndex + 1] = new Color32(baseColor32.r, baseColor32.g, baseColor32.b, (byte)alpha);
                colors[vertexIndex + 2] = new Color32(baseColor32.r, baseColor32.g, baseColor32.b, (byte)alpha);
                colors[vertexIndex + 3] = new Color32(baseColor32.r, baseColor32.g, baseColor32.b, (byte)alpha);

                textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }

            // pause after punctuation
            if (i < text.Length)
            {
                if (text[i] == '.') yield return new WaitForSeconds(periodPause);
                if (text[i] == ',') yield return new WaitForSeconds(commaPause);
                if (text[i] == '?') yield return new WaitForSeconds(questionPause);
                if (text[i] == ':') yield return new WaitForSeconds(colonPause);
                if (text[i] == '!') yield return new WaitForSeconds(exclamationPause);
            }
        }

        // fade out after all characters done
        yield return new WaitForSeconds(fadeOutDelay);

        float outElapsed = 0f;
        while (outElapsed < fadeOutDuration)
        {
            outElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, outElapsed / fadeOutDuration);
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }
    }
}