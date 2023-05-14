using System;
using UnityEngine;

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

        public static Vector3 AddVectors(this Vector3 self, Vector3[] vectors)
        {
            foreach(Vector3 vector in vectors){
                self.Set(self.x + vector.x,self.y + vector.y,self.z + vector.z);
            }

            return self;
        }
        public static Vector3 AddVector(this Vector3 self, Vector3 vector)
            => self.AddVectors(new []{vector});
    }
}