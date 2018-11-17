using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace AirDebugger.Editor
{
	public enum ConnectionStatus
	{
		Connected,
		Disconnected
	}

	public delegate void ConnectDelegate(ConnectionStatus status, ConnectToPlayer connectToPlayer);

	public class PlayerConnectorAttribute : Attribute
	{
		private static IEnumerable<ConnectDelegate> GetAllConnectors()
		{
			var allTypes = ReflectionHelper.GetAllTypes(fromEditor: true);
			const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return allTypes
				.SelectMany(type => ReflectionHelper.GetMethodsWithAttr<PlayerConnectorAttribute>(type, bindingFlags))
				.Select(m => (ConnectDelegate) Delegate.CreateDelegate(typeof(ConnectDelegate), m));
		}

		public static void Connect(ConnectToPlayer ctp)
		{
			var ccs = GetAllConnectors();
			foreach (var c in ccs)
			{
				c(ConnectionStatus.Connected, ctp);
			}
		}

		public static void Disconnect(ConnectToPlayer ctp)
		{
			var ccs = GetAllConnectors();
			foreach (var c in ccs)
			{
				c(ConnectionStatus.Disconnected, ctp);
			}
		}
	}

	public class ConnectToPlayer : EditorWindow
	{
		private EditorConnection editorConnection;
		private int currentPlayerID = -1;


		private void OnEnable()
		{
			editorConnection = EditorConnection.instance;
			editorConnection.Initialize();
			editorConnection.RegisterConnection(OnPlayerConnected);
			editorConnection.RegisterDisconnection(OnPlayerDisconnected);
			editorConnection.Register(EditorConnectionMessageID.Player, OnPlayerMessageReceived);
			PlayerConnectorAttribute.Connect(this);
		}

		private void OnDisable()
		{
			PlayerConnectorAttribute.Disconnect(this);
			editorConnection.Unregister(EditorConnectionMessageID.Player, OnPlayerMessageReceived);
			editorConnection.DisconnectAll();
			editorConnection = null;
		}

		private void OnPlayerConnected(int playerID)
		{
			Debug.LogFormat("OnPlayerConnected {0}", playerID);
			currentPlayerID = playerID;
		}

		private void OnPlayerDisconnected(int playerID)
		{
			if (currentPlayerID == playerID)
				currentPlayerID = -1;
		}


		public event Action onGUI;

		public void RegisterMessageHandler(string ID, MessageHandler handler)
		{
			MessageHandler mh;
			if (onPlayerMessageReceived.TryGetValue(ID, out mh))
			{
				mh += handler;
			}
			else
			{
				onPlayerMessageReceived.Add(ID, handler);
			}
		}

		public void UnregisterMessageHandler(string ID, MessageHandler handler)
		{
			MessageHandler mh;
			if (onPlayerMessageReceived.TryGetValue(ID, out mh))
			{
				mh -= handler;
			}
		}

		private readonly Dictionary<string, MessageHandler> onPlayerMessageReceived
			= new Dictionary<string, MessageHandler>();

		private void OnGUI()
		{
			if (onGUI != null)
			{
				onGUI();
			}
		}

		private void OnPlayerMessageReceived(MessageEventArgs args)
		{
			if (args.playerId != currentPlayerID)
			{
				return;
			}
			var msgReceived = args.data.Deserialize<ECMBase>();
			MessageHandler mh;
			var msgRespondingList = new List<ECMBase>();
			if (onPlayerMessageReceived.TryGetValue(msgReceived.ID, out mh))
			{
				mh(args.data, msgRespondingList);
			}
			foreach (var r in msgRespondingList)
			{
				Send(r);
			}
		}

		public void Send(ECMBase message)
		{
			editorConnection.Send(EditorConnectionMessageID.Editor, message.SerializeToByteArray());
		}
	}
}
