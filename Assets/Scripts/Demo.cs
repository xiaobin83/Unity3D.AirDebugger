using UnityEngine;
using AirDebugger;

public class Demo : MonoBehaviour
{
	[SerializeField, AirInspector] private string text;

	[AirInspector(AirInspectorType.NotificationOnChanged)]
	void AirInspector_OnChanged()
	{

	}
}
