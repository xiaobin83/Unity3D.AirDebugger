using System.Collections.Generic;

namespace AirDebugger.Editor
{
	public class AirInspectorWindow
	{
		[PlayerConnector]
		private static void Connect(ConnectionStatus status, ConnectToPlayer ctp)
		{
			if (status == ConnectionStatus.Connected)
			{
				ctp.onGUI += OnGUI;
				ctp.RegisterMessageHandler(ECMID.Inspector, OnMessageReceived);
			}
			else if (status == ConnectionStatus.Disconnected)
			{
				ctp.onGUI -= OnGUI;
				ctp.UnregisterMessageHandler(ECMID.Inspector, OnMessageReceived);
			}
		}

		private static void OnGUI()
		{

		}

		private static void OnMessageReceived(byte[] msg, List<ECMBase> responseToPlayer)
		{

		}
	}
}
