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
        float duration, float delay, Action<float> onUpdate, Action onPlay = null, Action onComplete = null,
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
        var startPosition = rect.anchoredPosition;
        return Animate(duration, delay,
            t => rect.anchoredPosition = Vector2.Lerp(startPosition, toMove, t),
            onPlay, onComplete, Easing.EaseOutQuad);
    }

    private static IEnumerator FadeIn(Graphic graphic, float duration, float delay, Color color, Action onPlay,
        Action onComplete)
    {
        return Animate(duration, delay,
            t => graphic.color = new Color(color.r, color.g, color.b, t),
            onPlay, onComplete, Easing.EaseOutQuad);
    }
    
    private static IEnumerator FadeOut(Graphic graphic, float duration, float delay, Color color, Action onPlay,
        Action onComplete)
    {
        return Animate(duration, delay,
            t => graphic.color = new Color(color.r, color.g, color.b, 1 - t),
            onPlay, onComplete, Easing.EaseOutQuad);
    }

    private static IEnumerator ImageSlide(Image image, float duration, float delay, float start, float end,
        int fillOrigin, Action onPlay, Action onComplete)
    {
        image.fillOrigin = fillOrigin;
        
        return Animate(duration, delay,
            t => image.fillAmount = Mathf.Lerp(start, end, t),
            onPlay, onComplete, Easing.EaseOutQuad);
    }

    private static IEnumerator ScaleX(GameObject obj, float from, float to, float duration, float delay,
        Action onPlay, Action onComplete)
    {
        return Animate(duration, delay,
            t => obj.transform.localScale = new Vector3(Mathf.Lerp(from, to, t), obj.transform.localScale.y, obj.transform.localScale.z),
            onPlay, onComplete);
    }

    private static IEnumerator ScaleY(GameObject obj, float fromSize, float toSize, float duration, float delay,
        Action onPlay, Action onComplete)
    {
        return Animate(duration, delay,
            t => obj.transform.localScale = new Vector3(obj.transform.localScale.x, Mathf.Lerp(fromSize, toSize, t), obj.transform.localScale.z),
            onPlay, onComplete);
    }
    #endregion

    #region Public Animation API
    /// <summary>
    /// 특정 애니메이션 정지
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="rect">오브젝트</param>
    /// <typeparam name="T">데이터 형식</typeparam>
    public static void StopAnimation<T>(MonoBehaviour runner, T rect)
    {
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
    /// <param name="rect">오브젝트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="toMove">이동할 위치</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public static void MoveAnimation(MonoBehaviour runner, RectTransform rect, float duration, float delay, Vector2 toMove,
        Action onPlay = null, Action onComplete = null)
    {
        StartManagedCoroutine(runner, rect, Move(rect, duration, delay, toMove, onPlay, onComplete));
    }

    /// <summary>
    /// 페이드 인 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="graphic">이미지 또는 텍스트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="color">색상</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public static void FadeInAnimation(MonoBehaviour runner, Graphic graphic, float duration, float delay, Color color,
        Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, graphic, FadeIn(graphic, duration, delay, color, onPlay, onComplete));
    }

    /// <summary>
    /// 페이드 아웃 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="graphic">이미지 또는 텍스트</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="color">색상</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public static void FadeOutAnimation(MonoBehaviour runner, Graphic graphic, float duration, float delay, Color color,
        Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, graphic, FadeOut(graphic, duration, delay, color, onPlay, onComplete));
    }

    /// <summary>
    /// 슬라이드 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="image">이미지</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="fillOrigin">이동 방향</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    /// <param name="start">시작 값</param>
    /// <param name="end">끝 값</param>
    public static void SlideAnimation(MonoBehaviour runner, Image image, float duration, float delay, float start,
        float end, int fillOrigin, Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, image, ImageSlide(image, duration, delay, start, end, fillOrigin, onPlay, onComplete));
    }

    /// <summary>
    /// 스케일X 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="obj">오브젝트</param>
    /// <param name="fromSize">몇에서</param>
    /// <param name="toSize">몇으로</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public static void ScaleXAnimation(MonoBehaviour runner, GameObject obj, float fromSize, float toSize,
        float duration, float delay, Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, obj, ScaleX(obj, fromSize, toSize, duration, delay, onPlay, onComplete));
    }

    /// <summary>
    /// 스케일Y 애니메이션
    /// </summary>
    /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
    /// <param name="obj">오브젝트</param>
    /// <param name="fromSize">몇에서</param>
    /// <param name="toSize">몇으로</param>
    /// <param name="duration">지속시간</param>
    /// <param name="delay">지연시간</param>
    /// <param name="onPlay">시작했을 때 실행할 행동</param>
    /// <param name="onComplete">끝났을 때 실행할 행동</param>
    public static void ScaleYAnimation(MonoBehaviour runner, GameObject obj, float fromSize, float toSize,
        float duration, float delay, Action onPlay, Action onComplete)
    {
        StartManagedCoroutine(runner, obj, ScaleY(obj, fromSize, toSize, duration, delay, onPlay, onComplete));
    }
    #endregion
}