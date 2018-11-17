using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine;

namespace AirDebugger
{
	public static class AirInspectorHelper
	{
		private static readonly Dictionary<Type, AirInspectorInfo> inspectorInfos =
			new Dictionary<Type, AirInspectorInfo>();

		public static bool Collect(IEnumerable<MonoBehaviour> monoBehaviours,
			Dictionary<Type, AirInspector> airInspectors)
		{
			var foundInspectors = false;

			const BindingFlags bindingFlags = BindingFlags.Static
			                                  | BindingFlags.Instance
			                                  | BindingFlags.Public
			                                  | BindingFlags.NonPublic;

			// collect all new types not added to inspector infos
			var allNewInfos = monoBehaviours
				.Select(monoBehaviour => monoBehaviour.GetType())
				.Distinct()
				.Where(type => !inspectorInfos.ContainsKey(type))
				.Select(type => new AirInspectorInfo(
					type,
					ReflectionHelper.GetFieldsWithAttr<AirInspectorAttribute>(type, bindingFlags),
					ReflectionHelper.GetPropertiesWithAttr<AirInspectorAttribute>(type, bindingFlags),
					ReflectionHelper.GetMethodsWithAttr<AirInspectorAttribute>(type, bindingFlags)
				))
				.Where(info => info.valid);

			foreach (var info in allNewInfos)
			{
				inspectorInfos.Add(info.type, info);
			}

			// bind air inspectors
			foreach (var monoBehaviour in monoBehaviours)
			{
				var type = monoBehaviour.GetType();
				AirInspectorInfo info;
				if (inspectorInfos.TryGetValue(type, out info))
				{
					AirInspector inspector;
					if (airInspectors.TryGetValue(type, out inspector))
					{
						inspector.Register(monoBehaviour);
					}
					else
					{
						inspector = new AirInspector(info);
						inspector.Register(monoBehaviour);
						airInspectors.Add(type, inspector);
					}

					foundInspectors = true;
				}
			}

			return foundInspectors;
		}
	}
}
