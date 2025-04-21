using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenDeathUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup SuddenDeathCanvasGroup;
    [SerializeField]
    private int FlashCount = 5;
    [SerializeField]
    private float FlashInterval = 0.2f;
    private void OnEnable()
    {
        TurnStateEvents.OnSuddenDeath += DoSuddenDeath;
    }
    private void OnDisable()
    {
        TurnStateEvents.OnSuddenDeath -= DoSuddenDeath;
    }
    private void DoSuddenDeath()
    {
        StartCoroutine(FlashFadeCoroutine());
    }
    private IEnumerator FlashFadeCoroutine()
    {
        for (int i = 0; i < FlashCount; i++)
        {
            yield return FadeTo(1f, FlashInterval); // Fade in
            yield return FadeTo(0f, FlashInterval); // Fade out
        }

        yield return FadeTo(0f, FlashInterval); // Fade out for last time
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = SuddenDeathCanvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            SuddenDeathCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        SuddenDeathCanvasGroup.alpha = targetAlpha;
    }
}
