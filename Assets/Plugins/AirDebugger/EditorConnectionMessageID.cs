using System;
using UnityEngine.Experimental.UIElements;

namespace AirDebugger
{
	public static class EditorConnectionMessageID
	{
		public static readonly Guid Editor = new Guid("94D35D8F-99A2-4956-B72F-F13897EA4F1F");
		public static readonly Guid Player = new Guid("016BF130-5A54-4279-A23E-09036E0132FB");
	}

	public static class ECMID
	{
		public const string Inspector = "Inspector";
		public const string PutFile = "PutFile";
		public const string String = "string";
	}

	public class ECMBase
	{
		public string ID { get; protected set; }
	}


	public enum ECMInspectorOp
	{
		GetValue,
		SetValue,
		CallMethod
	}

	public class ECMInspector : ECMBase
	{
		public ECMInspector()
		{
			ID = ECMID.Inspector;
		}

		public ECMInspectorOp Op;
	}

	public class ECMPutFile : ECMBase
	{
		public ECMPutFile()
		{
			ID = ECMID.PutFile;
		}

		public string Path;
		public string Content;
	}

	public class ECMString : ECMBase
	{
		public ECMString()
		{
			ID = ECMID.String;
		}

		public string String;
	}
}
