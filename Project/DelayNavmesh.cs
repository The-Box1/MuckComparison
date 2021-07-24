using UnityEngine;
using UnityEngine.AI;

public class DelayNavmesh : MonoBehaviour
{
	private void Awake()
	{
		Invoke("ResetObstacle", Random.Range(5f, 15f));
	}

	private void ResetObstacle()
	{
		NavMeshObstacle component = GetComponent<NavMeshObstacle>();
		component.enabled = false;
		component.enabled = true;
	}
}
