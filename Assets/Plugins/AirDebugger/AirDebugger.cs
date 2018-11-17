using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AirDebugger
{
	public class AirDebugger : MonoBehaviour
	{
		private static AirDebugger TryGetInstance()
		{
			return FindObjectsOfType<AirDebugger>().FirstOrDefault(a => !a.invalid);
		}

		private readonly Dictionary<Type, AirInspector> airInspectors = new Dictionary<Type, AirInspector>();

		private bool invalid = false;

		private void Awake()
		{
			var airDebugger = TryGetInstance();
			if (airDebugger != this)
			{
				Debug.LogWarning("AirDebugger already instantiated.");
				invalid = true;
				return;
			}

			DontDestroyOnLoad(gameObject);

			ConnectToEditor.RegisterMessageHandler(
				ECMID.Inspector, OnEditorInspectorMessageReceived);

			SceneManager.activeSceneChanged += OnSceneChanged;
		}

		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= OnSceneChanged;
			ConnectToEditor.UnregisterMessageHandler(
				ECMID.Inspector, OnEditorInspectorMessageReceived);
		}

		private void CollectAirInspectorsInScene(Scene scene)
		{
			var rootObjects = scene.GetRootGameObjects();
			var monoBehaviours = new List<MonoBehaviour>();
			foreach (var go in rootObjects)
			{
				go.GetComponentsInChildren(monoBehaviours);
				if (AirInspectorHelper.Collect(monoBehaviours, airInspectors))
				{
					go.AddComponent<AirInspectorWatchDog>();
				}
			}
		}

		private void OnEditorInspectorMessageReceived(byte[] data, List<ECMBase> res)
		{
			var ecmInspector = data.Deserialize<ECMInspector>();
			Debug.LogFormat("[Inspector]{0}", ecmInspector.Op);
		}

		private void OnSceneChanged(Scene from, Scene to)
		{
			Debug.LogFormat("OnSceneChanged {0} -> {1}", from.name, to.name);
			if (invalid)
			{
				return;
			}

			airInspectors.Clear();
			CollectAirInspectorsInScene(to);
		}
	}
}
