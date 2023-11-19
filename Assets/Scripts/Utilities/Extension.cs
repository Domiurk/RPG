using System;

namespace Utilities
{
    public static class Extension
    {
        public static T With<T>(this T self, Action<T> action)
        {
            action.Invoke(self);
            return self;
        }
        
        public static T With<T>(this T self, Action<T> action, bool condition)
        {
            if(condition)
                action.Invoke(self);
            return self;
        }
        
        public static T With<T>(this T self, Action<T> action, Func<bool> condition)
        {
            if(condition())
                action.Invoke(self);
            return self;
        }
    }
}