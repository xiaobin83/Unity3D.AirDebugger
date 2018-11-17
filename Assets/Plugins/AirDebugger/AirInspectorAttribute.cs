using System;
using System.Threading;

namespace AirDebugger
{
	public class AirInspectorAttribute : Attribute
	{
		private AirInspectorType type;

		public AirInspectorAttribute(AirInspectorType type = AirInspectorType.Default)
		{
			this.type = type;
		}
	}
}
