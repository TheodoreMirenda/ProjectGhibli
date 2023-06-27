using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TJ
{
[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		MapGenerator mapGen = (MapGenerator)target;

		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				mapGen.GenerateMap ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.GenerateMap ();
		}
		if (GUILayout.Button ("Smooth")) {
			mapGen.Smooth ();
		}
	}
}
}