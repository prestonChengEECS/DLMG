using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class TextFadeIn : MonoBehaviour
{
    [Header("Text & Timings")]
    public TextMeshProUGUI textComponent;
    public float defaultFadeDuration = 0.05f;
    public float waitTime = 0f;
    public float backspaceSpeed = 0.05f;
    public float hesitationPause = 0.6f;

    [Header("Punctuation Pauses")]
    public float periodPause = 0.8f;
    public float commaPause = 0.3f;
    public float exclamationPause = 1f;
    public float questionPause = 0.8f;
    public float colonPause = 0.8f;

    [Header("Fade Out")]
    public float fadeOutDuration = 0.5f;
    public float fadeOutDelay = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip keyPressSound;
    public AudioClip spacePressSound;
    public AudioClip backspaceSound;

    [Header("Styling")]
    public Color textColor = Color.white;

    private enum ActionType { TypeChar, Backspace, Pause }
    private struct TextAction
    {
        public ActionType type;
        public char character;
        public int count;
        public float duration;
    }

    void Start()
    {
        if (textComponent != null)
        {
            StartCoroutine(FadeInText(textComponent));
        }
    }

    IEnumerator FadeInText(TextMeshProUGUI textComponent)
    {
        string rawText = textComponent.text;

        // Parse custom tags (<bs=N>, <pause=N>)
        List<TextAction> actions = ParseTextActions(rawText);

        // Calculate the exact final text to measure its dimensions
        string finalCleanText = CalculateFinalString(actions);

        // Set alignment to Left so typing doesn't push the left edge around
        textComponent.alignment = TextAlignmentOptions.TopLeft;
        textComponent.color = textColor;

        // Force TMP to update geometry for the final string to measure width/height
        textComponent.text = finalCleanText;
        textComponent.ForceMeshUpdate();

        // Calculate offset to place the top-left origin dead-center
        Vector2 boundsSize = textComponent.GetRenderedValues(false);
        RectTransform rectTransform = textComponent.rectTransform;

        // Center the whole bounding box on screen
        rectTransform.anchoredPosition = new Vector2(-boundsSize.x / 2f, boundsSize.y / 2f);

        // Reset text to empty and begin typing
        textComponent.text = "";

        yield return new WaitForSeconds(waitTime);

        System.Text.StringBuilder visibleString = new System.Text.StringBuilder();

        foreach (TextAction action in actions)
        {
            switch (action.type)
            {
                case ActionType.TypeChar:
                    if (action.character == ' ')
                    {
                        if (audioSource != null && spacePressSound != null)
                            audioSource.PlayOneShot(spacePressSound);
                    }
                    else
                    {
                        if (audioSource != null && keyPressSound != null)
                            audioSource.PlayOneShot(keyPressSound);
                    }

                    visibleString.Append(action.character);
                    textComponent.text = visibleString.ToString();
                    textComponent.ForceMeshUpdate();

                    yield return StartCoroutine(FadeInLastCharacter());
                    yield return HandlePunctuationPause(action.character);
                    break;

                case ActionType.Pause:
                    yield return new WaitForSeconds(action.duration);
                    break;

                case ActionType.Backspace:
                    yield return new WaitForSeconds(hesitationPause);

                    int eraseCount = Mathf.Min(action.count, visibleString.Length);
                    for (int i = 0; i < eraseCount; i++)
                    {
                        if (visibleString.Length > 0)
                        {
                            visibleString.Remove(visibleString.Length - 1, 1);
                            textComponent.text = visibleString.ToString();
                            textComponent.ForceMeshUpdate();

                            if (audioSource != null && backspaceSound != null)
                                audioSource.PlayOneShot(backspaceSound);

                            yield return new WaitForSeconds(backspaceSpeed);
                        }
                    }

                    yield return new WaitForSeconds(hesitationPause);
                    break;
            }
        }

        // Fade Out Sequence
        yield return new WaitForSeconds(fadeOutDelay);
        float outElapsed = 0f;
        while (outElapsed < fadeOutDuration)
        {
            outElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, outElapsed / fadeOutDuration);
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeInLastCharacter()
    {
        int totalChars = textComponent.textInfo.characterCount;
        if (totalChars == 0) yield break;

        int lastIndex = totalChars - 1;
        TMP_CharacterInfo charInfo = textComponent.textInfo.characterInfo[lastIndex];

        if (!charInfo.isVisible) yield break;

        int meshIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        float elapsed = 0f;
        while (elapsed < defaultFadeDuration)
        {
            elapsed += Time.deltaTime;
            byte alpha = (byte)Mathf.Lerp(0, 255, elapsed / defaultFadeDuration);

            Color32[] colors = textComponent.textInfo.meshInfo[meshIndex].colors32;
            Color32 c = textColor;
            c.a = alpha;

            colors[vertexIndex] = c;
            colors[vertexIndex + 1] = c;
            colors[vertexIndex + 2] = c;
            colors[vertexIndex + 3] = c;

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }
    }

    IEnumerator HandlePunctuationPause(char c)
    {
        if (c == '.') yield return new WaitForSeconds(periodPause);
        else if (c == ',') yield return new WaitForSeconds(commaPause);
        else if (c == '?') yield return new WaitForSeconds(questionPause);
        else if (c == ':') yield return new WaitForSeconds(colonPause);
        else if (c == '!') yield return new WaitForSeconds(exclamationPause);
    }

    string CalculateFinalString(List<TextAction> actions)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var action in actions)
        {
            if (action.type == ActionType.TypeChar)
            {
                sb.Append(action.character);
            }
            else if (action.type == ActionType.Backspace)
            {
                int removeCount = Mathf.Min(action.count, sb.Length);
                if (removeCount > 0)
                {
                    sb.Remove(sb.Length - removeCount, removeCount);
                }
            }
        }
        return sb.ToString();
    }

    List<TextAction> ParseTextActions(string input)
    {
        List<TextAction> actions = new List<TextAction>();
        Regex tagRegex = new Regex(@"<(bs|pause)=([\d\.]+)>");
        int lastIndex = 0;

        foreach (Match match in tagRegex.Matches(input))
        {
            string leadingText = input.Substring(lastIndex, match.Index - lastIndex);
            foreach (char c in leadingText)
            {
                actions.Add(new TextAction { type = ActionType.TypeChar, character = c });
            }

            string command = match.Groups[1].Value;
            float val = float.Parse(match.Groups[2].Value);

            if (command == "bs")
                actions.Add(new TextAction { type = ActionType.Backspace, count = (int)val });
            else if (command == "pause")
                actions.Add(new TextAction { type = ActionType.Pause, duration = val });

            lastIndex = match.Index + match.Length;
        }

        string remainingText = input.Substring(lastIndex);
        foreach (char c in remainingText)
        {
            actions.Add(new TextAction { type = ActionType.TypeChar, character = c });
        }

        return actions;
    }
}