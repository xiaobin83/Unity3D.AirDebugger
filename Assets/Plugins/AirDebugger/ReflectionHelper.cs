using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace AirDebugger
{
	public static class ReflectionHelper
	{
		public static IEnumerable<Type> GetAllTypes(bool fromEditor)
		{
			IEnumerable<Type> allTypes = null;
			try
			{
				if (fromEditor)
					allTypes = Assembly.Load("Assembly-CSharp-Editor").GetTypes().AsEnumerable();
				else
					allTypes = Assembly.Load("Assembly-CSharp").GetTypes().AsEnumerable();
			}
			catch
			{
			}

			try
			{
				IEnumerable<Type> typesInPlugins;
				if (fromEditor)
					typesInPlugins = Assembly.Load("Assembly-CSharp-Editor-firstpass").GetTypes();
				else
					typesInPlugins = Assembly.Load("Assembly-CSharp-firstpass").GetTypes();
				if (allTypes != null)
					allTypes = allTypes.Union(typesInPlugins);
				else
					allTypes = typesInPlugins;
			}
			catch
			{
			}

			return allTypes;
		}

		public static IEnumerable<PropertyInfo> GetPropertiesWithAttr<TAttr>(Type type, BindingFlags bindingFlags)
		{
			return type
				.GetProperties(bindingFlags)
				.Where(m => m.GetCustomAttributes(typeof(TAttr), true).Length > 0);
		}

		public static IEnumerable<FieldInfo> GetFieldsWithAttr<TAttr>(Type type, BindingFlags bindingFlags)
		{
			return type
				.GetFields(bindingFlags)
				.Where(m => m.GetCustomAttributes(typeof(TAttr), true).Length > 0);
		}

		public static IEnumerable<MethodInfo> GetMethodsWithAttr<TAttr>(Type type, BindingFlags bindingFlags)
		{
			return type
				.GetMethods(bindingFlags)
				.Where(m => m.GetCustomAttributes(typeof(TAttr), true).Length > 0);
		}
	}
}
