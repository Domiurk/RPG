using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DevionGames
{
    public static class Utility
    {
        private static readonly Assembly[] m_AssembliesLookup;
        private static readonly Dictionary<string, Type> m_TypeLookup;
        private static readonly Dictionary<Type, FieldInfo[]> m_SerializedFieldInfoLookup;
        private static readonly Dictionary<Type, MethodInfo[]> m_MethodInfoLookup;
        private static readonly Dictionary<MemberInfo, object[]> m_MemberAttributeLookup;

        static Utility()
        {
            m_AssembliesLookup = GetLoadedAssemblies();
            m_TypeLookup = new Dictionary<string, Type>();
            m_SerializedFieldInfoLookup = new Dictionary<Type, FieldInfo[]>();
            m_MethodInfoLookup = new Dictionary<Type, MethodInfo[]>();
            m_MemberAttributeLookup = new Dictionary<MemberInfo, object[]>();
        }

        /// <summary>
        /// Gets the Type with the specified name, performing a case-sensitive search.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>The type with the specified name, if found; otherwise, null.</returns>
        public static Type GetType(string typeName)
        {
            if(string.IsNullOrEmpty(typeName)){
                Debug.LogWarning("Type name should not be null or empty!");
                return null;
            }

            if(m_TypeLookup.TryGetValue(typeName, out Type type)){
                return type;
            }

            type = Type.GetType(typeName);

            if(type == null){
                int num = 0;

                while(num < m_AssembliesLookup.Length){
                    type = Type.GetType(string.Concat(typeName, ",", m_AssembliesLookup[num].FullName));

                    if(type == null){
                        num++;
                    }
                    else{
                        break;
                    }
                }
            }

            if(type == null){
                foreach(Assembly a in m_AssembliesLookup){
                    Type[] assemblyTypes = a.GetTypes();

                    foreach(Type assemblyType in assemblyTypes){
                        if(assemblyType.Name == typeName){
                            type = assemblyType;
                            break;
                        }
                    }
                }
            }

            if(type != null){
                m_TypeLookup.Add(typeName, type);
            }

            return type;
        }

        public static Type GetElementType(Type type)
        {
            Type[] interfaces = type.GetInterfaces();

            return (from i in interfaces
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select i.GetGenericArguments()[0]).FirstOrDefault();
        }

        public static MethodInfo[] GetAllMethods(this Type type)
        {
            MethodInfo[] methods = Array.Empty<MethodInfo>();

            if(type != null && !m_MethodInfoLookup.TryGetValue(type, out methods)){
                methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                              .Concat(GetAllMethods(type.GetBaseType()))
                              .ToArray();
                m_MethodInfoLookup.Add(type, methods);
            }

            return methods;
        }

        public static FieldInfo GetSerializedField(this Type type, string name)
        {
            return type.GetAllSerializedFields().FirstOrDefault(x => x.Name == name);
        }

        public static FieldInfo[] GetAllSerializedFields(this Type type)
        {
            if(type == null){
                return Array.Empty<FieldInfo>();
            }

            FieldInfo[] fields = GetSerializedFields(type).Concat(GetAllSerializedFields(type.BaseType)).ToArray();
            fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
            return fields;
        }

        public static FieldInfo[] GetSerializedFields(this Type type)
        {
            if(!m_SerializedFieldInfoLookup.TryGetValue(type, out FieldInfo[] fields)){
                fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(x => x.IsPublic && !x.HasAttribute(typeof(NonSerializedAttribute)) ||
                                         x.HasAttribute(typeof(SerializeField)) ||
                                         x.HasAttribute(typeof(SerializeReference)))
                             .ToArray();
                fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
                m_SerializedFieldInfoLookup.Add(type, fields);
            }

            return fields;
        }

        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
        {
            while(type != null){
                yield return type;
                type = type.BaseType;
            }
        }

        public static IEnumerable<Type> BaseTypes(this Type type)
        {
            while(type != null){
                type = type.BaseType;
                yield return type;
            }
        }

        public static object[] GetCustomAttributes(MemberInfo memberInfo, bool inherit)
        {
            if(!m_MemberAttributeLookup.TryGetValue(memberInfo, out object[] customAttributes)){
                customAttributes = memberInfo.GetCustomAttributes(inherit);
                m_MemberAttributeLookup.Add(memberInfo, customAttributes);
            }

            return customAttributes;
        }

        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo)
        {
            object[] objArray = GetCustomAttributes(memberInfo, true);
            return objArray.Where(t => t.GetType() == typeof(T) || t.GetType().IsSubclassOf(typeof(T)))
                           .Cast<T>()
                           .ToArray();
        }

        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
        {
            object[] objArray = GetCustomAttributes(memberInfo, true);

            foreach(object objElement in objArray){
                if(objElement.GetType() == typeof(T) || objElement.GetType().IsSubclassOf(typeof(T))){
                    return (T)objElement;
                }
            }

            return default(T);
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo)
        {
            return memberInfo.HasAttribute(typeof(T));
        }

        public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            object[] objArray = GetCustomAttributes(memberInfo, true);
            return objArray.Any(t => t.GetType() == attributeType || t.GetType().IsSubclassOf(attributeType));
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        private static Assembly[] GetLoadedAssemblies()
        {
#if NETFX_CORE
			var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
			
			List<Assembly> loadedAssemblies = new List<Assembly>();
			
			var folderFilesAsync = folder.GetFilesAsync();
			folderFilesAsync.AsTask().Wait();
			
			foreach (var file in folderFilesAsync.GetResults())
			{
				if (file.FileType == ".dll" || file.FileType == ".exe")
				{
					try
					{
						var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
						AssemblyName name = new AssemblyName { Name = filename };
						Assembly asm = Assembly.Load(name);
						loadedAssemblies.Add(asm);
					}
					catch (BadImageFormatException)
					{
						// Thrown reflecting on C++ executable files for which the C++ compiler stripped the relocation addresses (such as Unity dlls): http://msdn.microsoft.com/en-us/library/x4cw969y(v=vs.110).aspx
					}
				}
			}
			
			return loadedAssemblies.ToArray();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }
}