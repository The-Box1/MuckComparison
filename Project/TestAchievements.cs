using UnityEngine;

public class TestAchievements : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			GameManager.instance.GameOver(-3);
		}
	}
}
