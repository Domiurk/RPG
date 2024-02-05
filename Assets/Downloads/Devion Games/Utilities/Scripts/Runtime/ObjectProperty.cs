using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevionGames
{
    [Serializable]
    public class ObjectProperty : INameable
    {
        [SerializeField]
        private string name = string.Empty;

        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField]
        private int typeIndex;

        public Type SerializedType => SupportedTypes[typeIndex];

        public string stringValue;
        public int intValue;

        public float floatValue;
        public Color colorValue;
        public bool boolValue;
        public Object objectReferenceValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public bool show;
        public Color color = Color.white;

        public object GetValue()
        {
            Type mType = SerializedType;

            if(mType == null){
                return null;
            }

            if(mType == typeof(string)){
                return stringValue;
            }

            if(mType == typeof(bool)){
                return boolValue;
            }

            if(mType == typeof(Color)){
                return colorValue;
            }

            if(mType == typeof(float)){
                return floatValue;
            }

            if(typeof(Object).IsAssignableFrom(mType)){
                return objectReferenceValue;
            }

            if(mType == typeof(int)){
                return intValue;
            }

            if(mType == typeof(Vector2)){
                return vector2Value;
            }

            if(mType == typeof(Vector3)){
                return vector3Value;
            }

            return null;
        }

        public void SetValue(object value)
        {
            switch(value){
                case string s:
                    typeIndex = 0;
                    stringValue = s;
                    break;
                case bool b:
                    typeIndex = 1;
                    boolValue = b;
                    break;
                case Color color1:
                    typeIndex = 2;
                    colorValue = color1;
                    break;
                case float or double:
                    typeIndex = 3;
                    floatValue = Convert.ToSingle(value);
                    break;
                case Object obj:
                    typeIndex = 4;
                    objectReferenceValue = obj;
                    break;
                case int or uint or long or sbyte or byte or short or ushort or ulong:
                    typeIndex = 5;
                    intValue = Convert.ToInt32(value);
                    break;
                case Vector2 vector2:
                    typeIndex = 6;
                    vector2Value = vector2;
                    break;
                case Vector3 vector3:
                    typeIndex = 7;
                    vector3Value = vector3;
                    break;
                default:
                    Debug.LogWarning("Type is not supported " + value);
                    break;
            }
        }

        public static string GetPropertyName(Type mType)
        {
            if(mType == typeof(string)){
                return "stringValue";
            }

            if(mType == typeof(bool)){
                return "boolValue";
            }

            if(mType == typeof(Color)){
                return "colorValue";
            }

            if(mType == typeof(float)){
                return "floatValue";
            }

            if(typeof(Object).IsAssignableFrom(mType)){
                return "objectReferenceValue";
            }

            if(mType == typeof(int)){
                return "intValue";
            }

            if(mType == typeof(Vector2)){
                return "vector2Value";
            }

            if(mType == typeof(Vector3)){
                return "vector3Value";
            }

            return string.Empty;
        }

        public string ToString(string format)
        {
            return SerializedType == typeof(float) ? floatValue.ToString(format) : GetValue().ToString();
        }

        public static Type[] SupportedTypes
        {
            get{
                return new[]{
                    typeof(string),
                    typeof(bool),
                    typeof(Color),
                    typeof(float),
                    typeof(Object),
                    typeof(int),
                    typeof(Vector2),
                    typeof(Vector3),
                };
            }
        }

        public static string[] DisplayNames
        {
            get{
                return new[]{
                    "String",
                    "Bool",
                    "Color",
                    "Float",
                    "Object",
                    "Int",
                    "Vector2",
                    "Vector3",
                };
            }
        }
    }
}