using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TJ
{
[CustomEditor (typeof (LandGenerator))]
public class LandGeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		LandGenerator landGen = (LandGenerator)target;

		if (DrawDefaultInspector ()) {
			// if (mapGen.autoUpdate) {
			// 	landGen.GenerateMap ();
			// }
		}
		if (GUILayout.Button ("Create Land")) {
			landGen.OfflineMapData();
		}
		// if (GUILayout.Button ("Spawn Land")) {
		// 	landGen.SpawnLand();
		// }
		// if (GUILayout.Button ("Clear Land")) {
		// 	landGen.ClearLand();
		// }
	}
}
}