using UnityEngine;
using System;
using System.Collections.Generic;

namespace AirDebugger
{
	public delegate ECMBase DeserializeDelegate(byte[] data);

	public static class EditorConnectionMessageHelper
	{
		private static Dictionary<string, DeserializeDelegate> deserializers
			= new Dictionary<string, DeserializeDelegate>();

		public static void RegisterDeserializer(string ID, DeserializeDelegate deserializer)
		{
			deserializers.Add(ID, deserializer);
		}

		public static ECMBase Deserialize(byte[] data)
		{
			var ecmBase = data.Deserialize<ECMBase>();
			DeserializeDelegate d;
			if (deserializers.TryGetValue(ecmBase.ID, out d))
			{
				return d(data);
			}
			Debug.LogErrorFormat("Unknown message ID {0}", ecmBase.ID);
			return null;
		}
	}
}
