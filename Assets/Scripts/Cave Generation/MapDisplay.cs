using UnityEngine;
using System.Collections;

namespace TJ
{
public class MapDisplay : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData, Texture2D texture) {
        // Debug.Log($"DrawMesh: meshData.vertices.Length = {meshData.vertices.Length}");
		meshFilter.sharedMesh = meshData.CreateMesh ();
		// meshRenderer.sharedMaterial.mainTexture = texture;
	}

}
}