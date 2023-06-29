using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TJ
{
[CustomEditor (typeof (GameBoard))]
public class GameBoardEditor : Editor {
	public override void OnInspectorGUI() {
		GameBoard gameboard = (GameBoard)target;

		
		// if (GUILayout.Button ("Load Map Data")) {
		// 	gameboard.LoadData();
		// }
		// if (GUILayout.Button ("Create Clickable Tiles")) {
		// 	gameboard.CreateClickableTiles();
		// }
		if (GUILayout.Button ("Create Initial Chunk")) {
			gameboard.CreateInitialChunk();
		}
		if (GUILayout.Button ("Create Neighbors")) {
			gameboard.GenerateNeighbors();
		}
		if (GUILayout.Button ("Clear All Chunks")) {
			gameboard.ClearAllChunks();
		}
		if (DrawDefaultInspector ()) {
			// if (mapGen.autoUpdate) {
			// 	landGen.GenerateMap ();
			// }
		}
	}
}
}