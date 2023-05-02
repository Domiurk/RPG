using System;

namespace Utilities.Runtime
{
    public static class Extensions
    {
        public static T With<T>(this T self, Action<T> action)
        {
            action.Invoke(self);
            return self;
        }
        
        public static T With<T>(this T self, Action<T> action, bool when)
        {
            if(when)
                With(self, action);
            return self;
        }
        
        public static T With<T>(this T self, Action<T> action, Func<bool> when)
        {
            if(when())
                With(self, action);
            return self;
        }
    }
}