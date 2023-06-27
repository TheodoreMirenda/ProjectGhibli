using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (PrototypeGenerator))]
public class PrototypeEditor : Editor {

	public override void OnInspectorGUI() {
		PrototypeGenerator prototypeGenerator = (PrototypeGenerator)target;
		base.OnInspectorGUI();

		if (GUILayout.Button ("Generate Prototypes")) {
			prototypeGenerator.GeneratePrototypes();
		}
        if (GUILayout.Button ("Display Prototypes")) {
			prototypeGenerator.DisplayPrototypes();
		}
		if (GUILayout.Button ("Clear Prototypes")) {
			prototypeGenerator.ClearPrototypes();
		}
	}
}