using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Text_Renaming.Scripts.Runtime
{
    public class RenameObjects : MonoBehaviour
    {
        public GameObject Object;
        public string Prefix = "mixamorig:";
        public string Change = string.Empty;

        public void Rename()
        {
            if (Prefix == string.Empty)
                throw new MissingFieldException($"Please Write Prefix {gameObject}");

            if (Object == null)
            {
                Debug.Log($"Start renaming children on this:{this.name}");
                Object = this.gameObject;
            }

            int count = 0;
            List<Transform> transformsChild = new List<Transform>()
                .With(list => list.AddRange(Object.GetComponentsInChildren<Transform>()
                    .Where(t => t.name.Contains(Prefix))));
            if (transformsChild.Count == 0)
            {
                Debug.Log($"GameObject have not childrens with prefix:\"{Prefix}\"");
                return;
            }

            foreach (Transform obj in transformsChild)
            {
                string newName = obj.name.Replace(Prefix, Change);
                obj.name = newName;
                count++;
            }

            Debug.Log($"You successful renamed {count} childrens.");
        }
    }

    public static class Extensions
    {
        public static T With<T>(this T self, Action<T> action)
        {
            action.Invoke(self);
            return self;
        }
    }
}