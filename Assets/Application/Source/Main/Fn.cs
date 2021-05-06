using UnityEngine;

namespace Yak
{
    // 便利関数群
    // 20210302
    public class Fn
    {
        public static float Limit(float value, float max, float min)//値、最大、最小
        {
            value = Mathf.Max(Mathf.Min(value, max), min);
            return value;
        }

        public static int Limit(int value, int max, int min)//値、最大、最小
        {
            value = Mathf.Max(Mathf.Min(value, max), min);
            return value;
        }

        public static int TurnNumber(int a, int max, int min)
        {
            if (a > max)
            {
                a = min;
            }else if (a < min)
            {
                a = max;
            }
            return a;
        }

        public static float angleNormal(float value)
        {
            return ((value + 180) % 360 - 360) % 360 + 180;
        }
    }
}