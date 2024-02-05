using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevionGames
{
    [Serializable]
    public class NamedVariable : INameable
    {
        [SerializeField]
        private string m_Name = "New Variable";
        public string Name { get => m_Name; set => m_Name = value; }

		[TextArea]
		[SerializeField]
		private string m_Description=string.Empty;

		public string Description { get => m_Description; set => m_Description = value; }

		[SerializeField]
		private NamedVariableType m_VariableType = 0;

		public NamedVariableType VariableType
		{
			get => m_VariableType;
			set => m_VariableType = value;
		}

		public Type ValueType {
			get{
				return VariableType switch{
					NamedVariableType.Bool => typeof(bool),
					NamedVariableType.Color => typeof(Color),
					NamedVariableType.Float => typeof(float),
					NamedVariableType.Int => typeof(int),
					NamedVariableType.Object => typeof(Object),
					NamedVariableType.String => typeof(string),
					NamedVariableType.Vector2 => typeof(Vector2),
					NamedVariableType.Vector3 => typeof(Vector3),
					_ => null
				};
			}
		}

		public string[] VariableTypeNames {
			get {
				return new[] {"Bool","Color","Float","Int", "Object", "String","Vector2", "Vector3"};
			}
		}

		public string stringValue = string.Empty;
		public int intValue;
		public float floatValue;
		public Color colorValue = Color.white;
		public bool boolValue;
		public Object objectReferenceValue;
		public Vector2 vector2Value = Vector2.zero;
		public Vector3 vector3Value = Vector3.zero;

		public object GetValue()
		{
			return VariableType switch{
				NamedVariableType.Bool => boolValue,
				NamedVariableType.Color => colorValue,
				NamedVariableType.Float => floatValue,
				NamedVariableType.Int => intValue,
				NamedVariableType.Object => objectReferenceValue,
				NamedVariableType.String => stringValue,
				NamedVariableType.Vector2 => vector2Value,
				NamedVariableType.Vector3 => vector3Value,
				_ => null
			};
		}

		public void SetValue(object value)
		{
			switch(value){
				case string s:
					m_VariableType = NamedVariableType.String;
					stringValue = s;
					break;
				case bool b:
					m_VariableType = NamedVariableType.Bool;
					boolValue = b;
					break;
				case Color color:
					m_VariableType = NamedVariableType.Color;
					colorValue = color;
					break;
				case float or double:
					m_VariableType = NamedVariableType.Float;
					floatValue = Convert.ToSingle(value);
					break;
				case null or Object:
					m_VariableType = NamedVariableType.Object;
					objectReferenceValue = (Object)value;
					break;
				case int or uint or long or sbyte or byte or short or ushort or ulong:
					m_VariableType = NamedVariableType.Int;
					intValue = Convert.ToInt32(value);
					break;
				case Vector2 vector2:
					m_VariableType = NamedVariableType.Vector2;
					vector2Value = vector2;
					break;
				case Vector3 vector3:
					m_VariableType = NamedVariableType.Vector3;
					vector3Value = vector3;
					break;
			}
		}

		public string PropertyPath
		{
			get{
				return m_VariableType switch{
					NamedVariableType.Bool => "boolValue",
					NamedVariableType.Color => "colorValue",
					NamedVariableType.Float => "floatValue",
					NamedVariableType.Int => "intValue",
					NamedVariableType.Object => "objectReferenceValue",
					NamedVariableType.String => "stringValue",
					NamedVariableType.Vector2 => "vector2Value",
					NamedVariableType.Vector3 => "vector3Value",
					_ => string.Empty
				};
			}
		}
	}

	public enum NamedVariableType
	{
		String = 0,
		Bool = 2,
		Color = 3,
		Float = 4,
		Object = 5,
		Int = 6,
		Vector2 = 7,
		Vector3 = 8
	}
}