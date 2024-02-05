#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Editor;
    using Sirenix.Utilities.Editor;
    using Utilities;
    using UnityEditor;

#endif

    [TypeInfoBox("We may have gone overboard with this example")]
    public class CustomGroupExample : SerializedMonoBehaviour
    {
        [PartyGroup(3f, 20f)]
        public int MyInt;

        [PartyGroup]
        public float MyFloat { get; set; }

        [PartyGroup]
        public void StateTruth()
        {
            Debug.Log("Odin Inspector is awesome.");
        }

        [PartyGroup("Group Two", 10f, 8f)]
        public Vector3 AVector3;

        [PartyGroup("Group Two")]
        public int AnotherInt;

        [InfoBox("Of course, all the controls are still usable. If you can catch them at least.")]
        [PartyGroup("Group Three", 0.8f, 250f)]
        public Quaternion AllTheWayAroundAndBack;

        [PartyGroup("Group Four", 1f, 12f)]
        public Thingy ThingyField;

        public class Thingy
        {
            [PartyGroup(1f, 12f)]
            public Thingy ThingyField;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PartyGroupAttribute : PropertyGroupAttribute
    {
        public float Speed { get; private set; }
        public float Range { get; private set; }

        public PartyGroupAttribute(float speed = 0f, float range = 0f, int order = 0) : base("_DefaultGroup", order)
        {
            Speed = speed;
            Range = range;
        }

        public PartyGroupAttribute(string groupId, float speed = 0f, float range = 0f, int order = 0) : base(groupId, order)
        {
            Speed = speed;
            Range = range;
        }

        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var party = (PartyGroupAttribute)other;
            if (Speed == 0f)
            {
                Speed = party.Speed;
            }

            if (Range == 0f)
            {
                Range = party.Range;
            }
        }
    }

#if UNITY_EDITOR

    public class PartyGroupAttributeDrawer : OdinGroupDrawer<PartyGroupAttribute>
    {
        private Color start;
        private Color target;

        protected override void Initialize()
        {
            start = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f);
            target = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUILayout.Space(8f);

            if (Event.current.rawType != EventType.Layout)
            {
                Vector3 offset = Property.LastDrawnValueRect.position + new Vector2(Property.LastDrawnValueRect.width, Property.LastDrawnValueRect.height) * 0.5f;
                Matrix4x4 matrix =
                    Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one) *
                    Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(Mathf.Sin((float)EditorApplication.timeSinceStartup * Attribute.Speed) * Attribute.Range, Vector3.forward), Vector3.one * (1f + MathUtilities.BounceEaseInFastOut(Mathf.Sin((float)EditorApplication.timeSinceStartup * 2f)) * 0.1f)) *
                    Matrix4x4.TRS(-offset + new Vector3(Mathf.Sin((float)EditorApplication.timeSinceStartup * 2f), 0f, 0f) * 100f, Quaternion.identity, Vector3.one) *
                    GUI.matrix;
                GUIHelper.PushMatrix(matrix);
            }

            if (Event.current.rawType == EventType.Repaint)
            {
                float t = MathUtilities.Bounce(Mathf.Sin((float)EditorApplication.timeSinceStartup * 2f));
                if (t <= 0f)
                {
                    start = target;
                    target = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f);
                }

                GUIHelper.PushColor(Color.Lerp(start, target, t));
            }

            SirenixEditorGUI.BeginBox();
            for (int i = 0; i < Property.Children.Count; i++)
            {
                InspectorProperty child = Property.Children[i];
                child.Draw(child.Label);
            }
            SirenixEditorGUI.EndBox();

            if (Event.current.rawType == EventType.Repaint)
            {
                GUIHelper.PopColor();
            }
            if (Event.current.rawType != EventType.Layout)
            {
                GUIHelper.PopMatrix();
            }

            GUIHelper.RequestRepaint();
            GUILayout.Space(8f);
        }
    }

#endif
}
#endif
