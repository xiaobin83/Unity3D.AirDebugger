using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace AirDebugger
{
	public enum AirInspectorType
	{
		Default,
		NotificationOnChanged
	}

	public class AirInspector
	{
		private List<MonoBehaviour> instances = new List<MonoBehaviour>();
		private AirInspectorInfo info;

		public AirInspector(AirInspectorInfo info)
		{
			this.info = info;
		}

		public void Register(MonoBehaviour instance)
		{
			instances.Add(instance);
		}

		public void Unregister(MonoBehaviour instance)
		{
			instances.Remove(instance);
		}

	}

	public class AirInspectorInfo
	{
		public Type type { get; private set; }
		private IEnumerable<PropertyInfo> properties;
		private IEnumerable<FieldInfo> fields;
		private IEnumerable<MethodInfo> methods;

		public bool valid { get; private set; }

		public AirInspectorInfo(
			Type type,
			IEnumerable<FieldInfo> fields, IEnumerable<PropertyInfo> properties, IEnumerable<MethodInfo> methods)
		{
			this.type = type;
			this.fields = fields;
			this.properties = properties;
			this.methods = methods;

			valid = fields.FirstOrDefault() != null
			        || properties.FirstOrDefault() != null
			        || methods.FirstOrDefault() != null;
		}
	}
}
