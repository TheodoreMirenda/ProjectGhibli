using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TJ
{
[CustomEditor (typeof (MeshManager))]
public class MeshManagerEditor : Editor {

	public override void OnInspectorGUI() {
		MeshManager meshManager = (MeshManager)target;
		base.OnInspectorGUI();

		if (GUILayout.Button ("Generate Prototypes")) {
			meshManager.GeneratePrototypes();
		}
        // if (GUILayout.Button ("Display Prototypes")) {
		// 	meshManager.DisplayPrototypes();
		// }
		// if (GUILayout.Button ("Clear Prototypes")) {
		// 	meshManager.ClearPrototypes();
		// }
	}
}
}