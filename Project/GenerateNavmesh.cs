using UnityEngine;
using UnityEngine.AI;

public class GenerateNavmesh : MonoBehaviour
{
	public NavMeshSurface surface;

	public void GenerateNavMesh()
	{
		surface.BuildNavMesh();
	}
}
