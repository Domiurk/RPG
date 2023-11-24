#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using Editor;
    using Sirenix.Utilities.Editor;
    using OdinInspector;
    using Utilities;

    public class OverrideGetTargetsExampleWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Odin/Demos/Odin Editor Window Demos/Draw Any Target")]
        private static void OpenWindow()
        {
            GetWindow<OverrideGetTargetsExampleWindow>()
                .position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [HideLabel]
        [Multiline(6)]
        [SuffixLabel("This is drawn", true)]
        public string Test;

        protected override IEnumerable<object> GetTargets()
        {
            yield return this;

            yield return GUI.skin.settings;

            yield return GUI.skin;
        }

        protected override void DrawEditor(int index)
        {
            object currentDrawingEditor = CurrentDrawingTargets[index];

            SirenixEditorGUI.Title(
                title: currentDrawingEditor.ToString(),
                subtitle: currentDrawingEditor.GetType().GetNiceFullName(),
                textAlignment: TextAlignment.Left,
                horizontalLine: true
            );

            base.DrawEditor(index);

            if (index != CurrentDrawingTargets.Count - 1)
            {
                SirenixEditorGUI.DrawThickHorizontalSeparator(15, 15);
            }
        }
    }
}
#endif
