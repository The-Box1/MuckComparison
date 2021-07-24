using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(Vector3.forward, 22f * Time.deltaTime);
		float num = 1f + Mathf.PingPong(Time.time * 0.25f, 0.3f) - 0.15f;
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one * num, Time.deltaTime * 2f);
	}
}
