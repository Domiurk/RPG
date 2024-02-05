#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

#if UNITY_EDITOR
    using Editor;
    using Utilities;
    using System.Collections;
#endif

    [Serializable]
    public class StatList
    {
        [SerializeField]
        [ValueDropdown("CustomAddStatsButton", IsUniqueList = true, DrawDropdownForListElements = false,
                          DropdownTitle = "Modify Stats")]
        [ListDrawerSettings(DraggableItems = false, ShowFoldout = true)]
        private List<StatValue> stats = new();

        public StatValue this[int index]
        {
            get => stats[index];
            set => stats[index] = value;
        }

        public int Count => stats.Count;

        public float this[StatType type]
        {
            get{ return stats.Where(statValue => statValue.Type == type).Select(statValue => statValue.Value).FirstOrDefault(); }
            set{
                for(int i = 0; i < stats.Count; i++){
                    if(stats[i].Type == type){
                        StatValue val = stats[i];
                        val.Value = value;
                        stats[i] = val;
                        return;
                    }
                }

                stats.Add(new StatValue(type, value));
            }
        }

#if UNITY_EDITOR
        private IEnumerable CustomAddStatsButton()
        {
            return Enum.GetValues(typeof(StatType))
                       .Cast<StatType>()
                       .Except(stats.Select(x => x.Type))
                       .Select(x => new StatValue(x))
                       .AppendWith(stats)
                       .Select(x => new ValueDropdownItem(x.Type.ToString(), x));
        }
#endif
    }

#if UNITY_EDITOR

    internal class StatListValueDrawer : OdinValueDrawer<StatList>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }

#endif
}
#endif