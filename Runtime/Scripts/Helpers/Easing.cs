// UnityEngine
using UnityEngine;

namespace Utils.Animation.Helper
{
    public static class Easing
    {
        public static float EaseOutQuad(float t)
        {
            return 1 - Mathf.Pow(1 - t, 2);
        }
    }
}
