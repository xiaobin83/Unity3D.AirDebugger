using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Collections.Generic;

namespace AirDebugger
{
	public delegate void MessageHandler(byte[] messageReceived, List<ECMBase> messagesToResponse);

	public class ConnectToEditor : MonoBehaviour
	{

		static ConnectToEditor instance_;
		static ConnectToEditor instance
		{
			get
			{
				if (instance_ == null)
				{
					var go = new GameObject("_ConnectToEditor");
					instance_ = go.AddComponent<ConnectToEditor>();
					DontDestroyOnLoad(go);
				}
				return instance_;
			}
		}

		public static void RegisterMessageHandler(string ID, MessageHandler handler)
		{
			MessageHandler mh;
			if (onEditorMessageReceived.TryGetValue(ID, out mh))
			{
				mh += handler;
			}
			else
			{
				onEditorMessageReceived.Add(ID, handler);
			}
		}

		public static void UnregisterMessageHandler(string ID, MessageHandler handler)
		{
			MessageHandler mh;
			if (onEditorMessageReceived.TryGetValue(ID, out mh))
			{
				mh -= handler;
			}
		}

		private PlayerConnection playerConnection;

		private void OnEnable()
		{
			playerConnection = PlayerConnection.instance;
			playerConnection.Register(EditorConnectionMessageID.Editor, OnEditorMessageReceived);
		}

		private void OnDisable()
		{
			playerConnection.Unregister(EditorConnectionMessageID.Editor, OnEditorMessageReceived);
			playerConnection.DisconnectAll();
		}

		private static readonly Dictionary<string, MessageHandler> onEditorMessageReceived
			= new Dictionary<string, MessageHandler>();

		private void OnEditorMessageReceived(MessageEventArgs args)
		{
			var msgReceived = args.data.Deserialize<ECMBase>();
			var msgRespondingList = new List<ECMBase>();
			MessageHandler mh;
			if (onEditorMessageReceived.TryGetValue(msgReceived.ID, out mh))
			{
				mh(args.data, msgRespondingList);
			}
			if (playerConnection.isConnected)
			{
				foreach (var r in msgRespondingList)
				{
					playerConnection.Send(EditorConnectionMessageID.Player, r.SerializeToByteArray());
				}
			}
		}

	}
}
