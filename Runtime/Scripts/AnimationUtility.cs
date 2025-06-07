// System
using System;

// UnityEngine
using UnityEngine;
using UnityEngine.UI;
using Utils.Animation.Helper;

namespace Utils.Animation
{
    public static class AnimationUtility
    {
        #region Public Animation API

        /// <summary>
        /// 특정 애니메이션 정지
        /// </summary>
        /// <typeparam name="T">데이터 형식</typeparam>
        /// <param name="runner">MonoBehaviour를 상속한 클래스</param>
        /// <param name="rect">애니메이션 대상 오브젝트</param>
        public static void StopAnimation<T>(MonoBehaviour runner, T rect)
        {
            if (runner == null) return;
            if (UITween.RunningCoroutines.TryGetValue(rect, out var coroutine))
            {
                runner.StopCoroutine(coroutine);
                UITween.RunningCoroutines.Remove(rect);
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
        public static void MoveAnimation(MonoBehaviour runner, RectTransform rect, float duration, float delay,
            Vector2 toMove,
            Action onPlay = null, Action onComplete = null)
        {
            UITween.StartManagedCoroutine(runner, rect,
                UITween.Move(rect, duration, delay, toMove, onPlay, onComplete));
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
        public static void FadeAnimation(MonoBehaviour runner, Graphic graphic, Color color, float fromAlpha,
            float toAlpha, float duration, float delay,
            Action onPlay, Action onComplete)
        {
            UITween.StartManagedCoroutine(runner, graphic,
                UITween.Fade(graphic, color, fromAlpha, toAlpha, duration, delay, onPlay, onComplete));
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
            UITween.StartManagedCoroutine(runner, image,
                UITween.ImageSlide(image, duration, delay, start, end, fillOrigin, onPlay, onComplete));
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
            UITween.StartManagedCoroutine(runner, obj,
                UITween.ScaleAxis(obj, from, to, duration, delay, onPlay, onComplete, isXAxis));
        }

        #endregion
    }
}