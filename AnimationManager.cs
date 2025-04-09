//System
using System;
using System.Collections;

//UnityEngine
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dotween의 기능을 개발
/// </summary>
public class AnimationManager : MonoBehaviour
{
    #region Animations
    /// <summary>
    /// 이동 애니메이션
    /// </summary>
    /// <param name="rect">오브젝트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="toMove">이동할 위치</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public void MoveAnimation(RectTransform rect, float duration, float delay, Vector2 toMove, Action onPlay = null, Action onComplete = null)
    {
        StartCoroutine(Move(rect, duration, delay, toMove, onPlay, onComplete));
    }
    
    /// <summary>
    /// 페이드 인 애니메이션
    /// </summary>
    /// <param name="graphic">이미지 또는 텍스트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="color">색상</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public void FadeInAnimation(Graphic graphic, float duration, float delay, Color color, Action onPlay, Action onComplete)
    {
        StartCoroutine(FadeIn(graphic, duration, delay, color, onPlay, onComplete));
    }
    
    /// <summary>
    /// 페이드 아웃 애니메이션
    /// </summary>
    /// <param name="graphic">이미지 또는 텍스트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="color">색상</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public void FadeOutAnimation(Graphic graphic, float duration, float delay, Color color, Action onPlay, Action onComplete)
    {
        StartCoroutine(FadeOut(graphic, duration, delay, color, onPlay, onComplete));
    }
    
    /// <summary>
    /// 슬라이드 애니메이션
    /// </summary>
    /// <param name="image">이미지</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public void SlideAnimation(Image image, float duration, float delay, Action onPlay, Action onComplete)
    {
        StartCoroutine(ImageSlide(image, duration, delay, onPlay, onComplete));    
    }
    
    /// <summary>
    /// 스케일 애니메이션
    /// </summary>
    /// <param name="obj">오브젝트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public void ScaleAnimation(GameObject obj, float duration, float delay, Action onPlay, Action onComplete)
    {
        StartCoroutine(ScaleY(obj, duration, delay, onPlay, onComplete));
    }
    #endregion
    
    private static IEnumerator Move(RectTransform rect, float duration, float delay, Vector2 toMove, Action onPlay = null, Action onComplete = null)
    {
        var startPosition = rect.anchoredPosition;
        yield return new WaitForSeconds(delay);
        onPlay?.Invoke();
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += 0.016f;
            yield return new WaitForSeconds(0.016f);
            
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = 1 - Mathf.Pow(1 - t, 2);
            
            rect.anchoredPosition = Vector2.Lerp(startPosition, toMove, easedT);
        }
        onComplete?.Invoke();
    }
    private static IEnumerator FadeIn(Graphic graphic, float duration, float delay, Color color, Action onPlay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onPlay?.Invoke();
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += 0.016f;
            yield return new WaitForSeconds(0.016f);
            
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = 1 - Mathf.Pow(1 - t, 2);

            graphic.color = new Color(color.r, color.g, color.b, easedT);
        }
        onComplete?.Invoke();
    }
    private static IEnumerator FadeOut(Graphic graphic, float duration, float delay, Color color, Action onPlay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onPlay?.Invoke();
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += 0.016f;
            yield return new WaitForSeconds(0.016f);
            
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = 1 - Mathf.Pow(1 - t, 2);

            graphic.color = new Color(color.r, color.g, color.b, 1 - easedT);
        }
        onComplete?.Invoke();
    }
    private static IEnumerator ImageSlide(Image image, float duration, float delay, Action onPlay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onPlay?.Invoke();
        float slideValue = 1;
        while (slideValue > 0)
        {
            slideValue -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            image.fillAmount = slideValue;
        }
        onComplete?.Invoke();
    }
    private static IEnumerator ScaleY(GameObject obj, float duration, float delay, Action onPlay, Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onPlay?.Invoke();
        float objectScaleY = obj.transform.localScale.y;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += 0.016f;
            yield return new WaitForSeconds(0.016f);
            float t = Mathf.Clamp01(elapsedTime / duration);
            float newScaleY = Mathf.Lerp(objectScaleY, 0, t);
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, newScaleY, obj.transform.localScale.z);
        }
        onComplete?.Invoke();
    }
}
