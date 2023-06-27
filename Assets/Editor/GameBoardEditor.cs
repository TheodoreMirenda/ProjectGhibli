using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TJ
{
[CustomEditor (typeof (GameBoard))]
public class GameBoardEditor : Editor {
	public override void OnInspectorGUI() {
		GameBoard gameboard = (GameBoard)target;

		if (DrawDefaultInspector ()) {
			// if (mapGen.autoUpdate) {
			// 	landGen.GenerateMap ();
			// }
		}
		if (GUILayout.Button ("Load Map Data")) {
			gameboard.LoadData();
		}
		if (GUILayout.Button ("Create Clickable Tiles")) {
			gameboard.CreateClickableTiles();
		}
	}
}
}