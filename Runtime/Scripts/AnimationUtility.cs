//System
using System;
using System.Collections;
using System.Collections.Generic;

//UnityEngine
using UnityEngine;
using UnityEngine.UI;

public static class Easing
{
    public static float EaseOutQuad(float t)
    {
        return 1 - Mathf.Pow(1 - t, 2);
    }
}

public static class AnimationUtility
{
    private static readonly Dictionary<object, Coroutine> RunningCoroutines = new();

    #region UI Tween Helpers
    private static void StartManagedCoroutine(MonoBehaviour runner, object key, IEnumerator coroutine)
    {
        if (runner == null) return;

        if (key == null) return;
        
        if (RunningCoroutines.TryGetValue(key, out var existing))
        {
            runner.StopCoroutine(existing);
        }

        var newCoroutine = runner.StartCoroutine(WrapCoroutine(key, coroutine));
        RunningCoroutines[key] = newCoroutine;
    }

    private static IEnumerator WrapCoroutine(object key, IEnumerator coroutine)
    {
        yield return coroutine;
        RunningCoroutines.Remove(key);
    }
    
    private static IEnumerator Animate(
        float duration, 
        float delay, 
        Action<float> onUpdate = null,
        Action onPlay = null, 
        Action onComplete = null,
        Func<float, float> easingFunc = null)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        onPlay?.Invoke();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = easingFunc?.Invoke(t) ?? t;
            onUpdate?.Invoke(easedT);
            yield return null;
        }

        onUpdate?.Invoke(1f);
        onComplete?.Invoke();
    }
    
    private static IEnumerator Move(RectTransform rect, float duration, float delay, Vector2 toMove,
        Action onPlay, Action onComplete)
    {
        if (rect == null) yield break;
        if(!rect.gameObject.activeInHierarchy) rect.gameObject.SetActive(true);
        
        Vector2 startPosition = rect.anchoredPosition;
        yield return Animate(duration, delay,
            t => rect.anchoredPosition = Vector2.Lerp(startPosition, toMove, t),
            onPlay, onComplete, Easing.EaseOutQuad);
    }

    private static IEnumerator Fade(Graphic graphic, Color color, float fromAlpha, float toAlpha, float duration, float delay, Action onPlay,
        Action onComplete)
    {
        if(graphic == null) yield break;
        if(!graphic.gameObject.activeInHierarchy) graphic.gameObject.SetActive(true);

        Color tempColor = color;
        tempColor.a = fromAlpha;
        yield return Animate(duration, delay,
            t =>
            {
                float currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                graphic.color = new Color(tempColor.r, tempColor.g, tempColor.b, currentAlpha);
            },
            onPlay, 
            onComplete, 
            Easing.EaseOutQuad);
    }

    private static IEnumerator ImageSlide(Image image, float duration, float delay, float start, float end,
        int fillOrigin, Action onPlay, Action onComplete)
    {
        if(image == null) yield break;
        if(!image.gameObject.activeInHierarchy) image.gameObject.SetActive(true);
        
        image.fillOrigin = fillOrigin;
        yield return Animate(duration, delay,
            t => image.fillAmount = Mathf.Lerp(start, end, t),
            onPlay, 
            onComplete, 
            Easing.EaseOutQuad);
    }

    private static IEnumerator ScaleX(GameObject obj, float from, float to, float duration, float delay,
        Action onPlay, Action onComplete)
    {
        if(obj == null) yield break;
        if(!obj.activeInHierarchy) obj.gameObject.SetActive(true);

        yield return Animate(duration, delay,
            t =>
            {
                Vector3 currentScale = obj.transform.localScale;
                currentScale.x = Mathf.Lerp(from, to, t);
                obj.transform.localScale = currentScale;
            },
            onPlay, 
            onComplete,
            Easing.EaseOutQuad);
    }
    
    private static IEnumerator ScaleY(GameObject obj, float from, float to, float duration, float delay,
        Action onPlay, Action onComplete)
    {
        if(obj == null) yield break;
        if(!obj.activeInHierarchy) obj.gameObject.SetActive(true);

        yield return Animate(duration, delay,
            t =>
            {
                Vector3 currentScale = obj.transform.localScale;
                currentScale.y = Mathf.Lerp(from, to, t);
                obj.transform.localScale = currentScale;
            },
            onPlay, 
            onComplete,
            Easing.EaseOutQuad);
    }

    private static IEnumerator ScaleAxis(GameObject obj, float from, float to, float duration, float delay,
        Action onPlay, Action onComplete, bool isXAxis)
    {
        
        return isXAxis ? 
            ScaleX(obj, from, to, duration, delay, onPlay, onComplete) :
            ScaleY(obj, from, to, duration, delay, onPlay, onComplete);
    }
    #endregion

    #region Public Animation API
    /// <summary>
    /// 특정 애니메이션 정지
    /// </summary>
    /// <typeparam name="T">데이터 형식</typeparam>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="rect">애니메이션 대상 오브젝트</param>
    public static void StopAnimation<T>(MonoBehaviour runner, T rect)
    {
        if(runner == null) return;
        if (RunningCoroutines.TryGetValue(rect, out var coroutine))
        {
            runner.StopCoroutine(coroutine);
            RunningCoroutines.Remove(rect);
        }
    }
    /// <summary>
    /// 이동 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehavior를 상속한 클래스</param>
    /// <param name="rect">대상 RectTransform</param>
    /// <param name="duration">애니메이션 지속시간</param>
    /// <param name="delay">애니메이션 지연시간</param>
    /// <param name="toMove">이동할 최종 위치</param>
    /// <param name="onPlay">애니메이션 시작 시 실행할 행동</param>
    /// <param name="onComplete">애니메이션 완료 시 실행할 행동</param>
    public static void MoveAnimation(MonoBehaviour runner, RectTransform rect, float duration, float delay, Vector2 toMove,
        Action onPlay = null, Action onComplete = null)
    {
        StartManagedCoroutine(runner, rect, Move(rect, duration, delay, toMove, onPlay, onComplete));
    }

    /// <summary>
    /// 페이드 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="graphic">대상 이미지 또는 텍스트</param>
    /// <param name="color">기본 색상</param>
    /// <param name="fromAlpha">시작 알파 값</param>
    /// <param name="toAlpha">종료 알파 값</param>
    /// <param name="duration">애니메이션 지속시간</param>
    /// <param name="delay">애니메이션 지연시간</param>
    /// <param name="onPlay">애니메이션 시작 시 실행할 행동</param>
    /// <param name="onComplete">애니메이션 완료 시 실행할 행동</param>
    public static void FadeAnimation(MonoBehaviour runner, Graphic graphic, Color color, float fromAlpha, float toAlpha, float duration, float delay,
        Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, graphic, Fade(graphic, color, fromAlpha, toAlpha, duration, delay, onPlay, onComplete));
    }

    /// <summary>
    /// 슬라이드 애니메이션 (fillAmount 조절)
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="image">대상 이미지</param>
    /// <param name="duration">애니메이션 지속시간</param>
    /// <param name="delay">애니메이션 지연시간</param>
    /// <param name="start">시작 fillAmount 값</param>
    /// <param name="end">끝 fillAmount 값</param>
    /// <param name="fillOrigin">이미지 채우기 시작 방향</param>
    /// <param name="onPlay">애니메이션 시작 시 실행할 행동</param>
    /// <param name="onComplete">애니메이션 완료 시 실행할 행동</param>
    public static void SlideAnimation(MonoBehaviour runner, Image image, float duration, float delay, float start,
        float end, int fillOrigin, Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, image, ImageSlide(image, duration, delay, start, end, fillOrigin, onPlay, onComplete));
    }

    /// <summary>
    /// 스케일 애니메이션 (X축 또는 Y축)
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="obj">대상 GameObject</param>
    /// <param name="from">시작 scale 값</param>
    /// <param name="to">종료 scale 값</param>
    /// <param name="duration">애니메이션 지속시간</param>
    /// <param name="delay">애니메이션 지연시간</param>
    /// <param name="onPlay">애니메이션 시작 시 실행할 행동</param>
    /// <param name="onComplete">애니메이션 완료 시 실행할 행동</param>
    /// <param name="isXAxis">true이면 X축, 아니면 Y축 애니메이션 수행</param>
    public static void ScaleAxisAnimation(MonoBehaviour runner, GameObject obj, float from, float to,
        float duration, float delay, Action onPlay, Action onComplete, bool isXAxis = false)
    {
        StartManagedCoroutine(runner, obj, ScaleAxis(obj, from, to, duration, delay, onPlay, onComplete, isXAxis));
    }
    #endregion
}