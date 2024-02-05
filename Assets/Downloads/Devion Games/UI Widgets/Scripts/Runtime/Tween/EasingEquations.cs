using UnityEngine;

namespace DevionGames.UIWidgets
{
    public static class EasingEquations
    {
        public enum EaseType
        {
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            Linear,
            Spring,
            EaseInBounce,
            EaseOutBounce,
            EaseInOutBounce,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
            EaseInElastic,
            EaseOutElastic,
            EaseInOutElastic,
            Punch,
        }

        public static float GetValue(EaseType easeType, float start, float end, float value)
        {
            return easeType switch{
                EaseType.EaseInQuad => EaseInQuad(start, end, value),
                EaseType.EaseOutQuad => EaseOutQuad(start, end, value),
                EaseType.EaseInOutQuad => EaseInOutQuad(start, end, value),
                EaseType.EaseInCubic => EaseInCubic(start, end, value),
                EaseType.EaseOutCubic => EaseOutCubic(start, end, value),
                EaseType.EaseInOutCubic => EaseInOutCubic(start, end, value),
                EaseType.EaseInQuart => EaseInQuart(start, end, value),
                EaseType.EaseOutQuart => EaseOutQuart(start, end, value),
                EaseType.EaseInOutQuart => EaseInOutQuart(start, end, value),
                EaseType.EaseInQuint => EaseInQuint(start, end, value),
                EaseType.EaseOutQuint => EaseOutQuint(start, end, value),
                EaseType.EaseInOutQuint => EaseInOutQuint(start, end, value),
                EaseType.EaseInSine => EaseInSine(start, end, value),
                EaseType.EaseOutSine => EaseOutSine(start, end, value),
                EaseType.EaseInOutSine => EaseInOutSine(start, end, value),
                EaseType.EaseInExpo => EaseInExpo(start, end, value),
                EaseType.EaseOutExpo => EaseOutExpo(start, end, value),
                EaseType.EaseInOutExpo => EaseInOutExpo(start, end, value),
                EaseType.EaseInCirc => EaseInCirc(start, end, value),
                EaseType.EaseOutCirc => EaseOutCirc(start, end, value),
                EaseType.EaseInOutCirc => EaseInOutCirc(start, end, value),
                EaseType.Spring => Spring(start, end, value),
                EaseType.EaseInBounce => EaseInBounce(start, end, value),
                EaseType.EaseOutBounce => EaseOutBounce(start, end, value),
                EaseType.EaseInOutBounce => EaseInOutBounce(start, end, value),
                EaseType.EaseInBack => EaseInBack(start, end, value),
                EaseType.EaseOutBack => EaseOutBack(start, end, value),
                EaseType.EaseInOutBack => EaseInOutBack(start, end, value),
                EaseType.EaseInElastic => EaseInElastic(start, end, value),
                EaseType.EaseOutElastic => EaseOutElastic(start, end, value),
                EaseType.EaseInOutElastic => EaseInOutElastic(start, end, value),
                _ => Linear(start, end, value)
            };
        }

        public static float Linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        public static float CLerp(float start, float end, float value)
        {
            const float min = 0.0f;
            const float max = 360.0f;
            float half = Mathf.Abs((max - min) * 0.5f);
            float retval = 0.0f;
            float diff = 0.0f;

            if((end - start) < -half){
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if((end - start) > half){
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else
                retval = start + (end - start) * value;

            return retval;
        }

        public static float Spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) +
                     value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if(value < 1)
                return end * 0.5f * value * value + start;
            value--;
            return -end * 0.5f * (value * (value - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if(value < 1)
                return end * 0.5f * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if(value < 1)
                return end * 0.5f * value * value * value * value + start;
            value -= 2;
            return -end * 0.5f * (value * value * value * value - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if(value < 1)
                return end * 0.5f * value * value * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value * value * value + 2) + start;
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value - 1)) + start;
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if(value < 1)
                return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if(value < 1)
                return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;

            switch(value){
                case < 1 / 2.75f:
                    return end * (7.5625f * value * value) + start;
                case < 2 / 2.75f:
                    value -= (1.5f / 2.75f);
                    return end * (7.5625f * (value) * value + .75f) + start;
                case < 2.5f / 2.75f:
                    value -= (2.25f / 2.75f);
                    return end * (7.5625f * (value) * value + .9375f) + start;
            }

            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }

        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if(value < d * 0.5f)
                return EaseInBounce(0, end, value * 2) * 0.5f + start;
            return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;

            if((value) < 1){
                s *= (1.525f);
                return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
            }

            value -= 2;
            s *= (1.525f);
            return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static float Punch(float amplitude, float value)
        {
            if(value is 0 or 1){
                return 0;
            }

            const float period = 1 * 0.3f;
            float s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
        }

        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if(value == 0)
                return start;

            if((value /= d) == 1)
                return start + end;

            if(a == 0f || a < Mathf.Abs(end)){
                a = end;
                s = p / 4;
            }
            else{
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        public static float EaseOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if(value == 0)
                return start;

            if((value /= d) == 1)
                return start + end;

            if(a == 0f || a < Mathf.Abs(end)){
                a = end;
                s = p * 0.25f;
            }
            else{
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if(value == 0)
                return start;

            if((value /= d * 0.5f) == 2)
                return start + end;

            if(a == 0f || a < Mathf.Abs(end)){
                a = end;
                s = p / 4;
            }
            else{
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if(value < 1)
                return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) +
                       start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end +
                   start;
        }
    }
}