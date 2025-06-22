// System
using System;
using System.Collections;
using System.Collections.Generic;

// UnityEngine
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Animation.Helper
{
    public static class UITween
    {
        public static readonly Dictionary<object, Coroutine> RunningCoroutines = new();

        #region UI Tween Helpers

        public static void StartManagedCoroutine(MonoBehaviour runner, object key, IEnumerator coroutine)
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

        public static IEnumerator WrapCoroutine(object key, IEnumerator coroutine)
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
                yield return new WaitForSecondsRealtime(delay);

            onPlay?.Invoke();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = easingFunc?.Invoke(t) ?? t;
                onUpdate?.Invoke(easedT);
                yield return null;
            }

            onUpdate?.Invoke(1f);
            onComplete?.Invoke();
        }

        public static IEnumerator Move(RectTransform rect, float duration, float delay, Vector2 toMove,
            Action onPlay, Action onComplete)
        {
            if (rect == null) yield break;
            if (!rect.gameObject.activeInHierarchy) rect.gameObject.SetActive(true);

            Vector2 startPosition = rect.anchoredPosition;
            yield return Animate(duration, delay,
                t => rect.anchoredPosition = Vector2.Lerp(startPosition, toMove, t),
                onPlay, onComplete, Easing.EaseOutQuad);
        }

        public static IEnumerator Fade(Graphic graphic, Color color, float fromAlpha, float toAlpha, float duration,
            float delay, Action onPlay,
            Action onComplete)
        {
            if (graphic == null) yield break;
            if (!graphic.gameObject.activeInHierarchy) graphic.gameObject.SetActive(true);

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

        public static IEnumerator ImageSlide(Image image, float duration, float delay, float start, float end,
            int fillOrigin, Action onPlay, Action onComplete)
        {
            if (image == null) yield break;
            if (!image.gameObject.activeInHierarchy) image.gameObject.SetActive(true);

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
            if (obj == null) yield break;
            if (!obj.activeInHierarchy) obj.gameObject.SetActive(true);

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
            if (obj == null) yield break;
            if (!obj.activeInHierarchy) obj.gameObject.SetActive(true);

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

        public static IEnumerator ScaleAxis(GameObject obj, float from, float to, float duration, float delay,
            Action onPlay, Action onComplete, bool isXAxis)
        {

            return isXAxis
                ? ScaleX(obj, from, to, duration, delay, onPlay, onComplete)
                : ScaleY(obj, from, to, duration, delay, onPlay, onComplete);
        }

        #endregion
    }
}
