using System.Reflection;
using UnityEditor;

namespace DevionGames.Graphs
{
    public static class EditorExtensions 
    {
		public static bool IsDocked(this EditorWindow window)
		{
			BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
			MethodInfo isDockedMethod = typeof(EditorWindow).GetProperty("docked", fullBinding).GetGetMethod(true);
			return (bool)isDockedMethod.Invoke(window, null);
		}
	}
}