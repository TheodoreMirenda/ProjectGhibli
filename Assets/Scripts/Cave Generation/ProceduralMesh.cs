using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour {
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();

	static MeshJobScheduleDelegate[] jobs = {
		MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
		MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel,
		MeshJob<SharedTriangleGrid, SingleStream>.ScheduleParallel,
		MeshJob<FlatHexagonGrid, SingleStream>.ScheduleParallel,
		MeshJob<PointyHexagonGrid, SingleStream>.ScheduleParallel
	};

	public enum MeshType {
		SquareGrid, SharedSquareGrid, SharedTriangleGrid,
		FlatHexagonGrid, PointyHexagonGrid
	};

	[SerializeField]
	MeshType meshType;

	[SerializeField, Range(1, 50)]
	int resolution = 1;

	Mesh mesh;

	void Awake () {
		mesh = new Mesh {
			name = "Procedural Mesh"
		};
		GetComponent<MeshFilter>().mesh = mesh;
	}

	void OnValidate () => enabled = true;

	void Update () {
		GenerateMesh();
		enabled = false;
	}

	void GenerateMesh () {
		Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
		Mesh.MeshData meshData = meshDataArray[0];

		jobs[(int)meshType](mesh, meshData, resolution, default).Complete();

		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
	}
}