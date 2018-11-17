using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AirDebugger.Editor
{
	public static class AirDebuggerTools
	{
		[MenuItem("Air Debugger/Connect to Player")]
		private static void OpenConnectToPlayerWindow()
		{
			var wnd = EditorWindow.GetWindow<ConnectToPlayer>("Connect To Player", focus: true);
			wnd.ShowUtility();
		}

	}
}
