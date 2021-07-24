using UnityEngine;

public class GronkSwordProjectile : MonoBehaviour
{
	private void Start()
	{
		_ = base.transform.forward;
		Vector3 euler = base.transform.rotation.eulerAngles + new Vector3(0f, 0f, -90f);
		base.transform.rotation = Quaternion.Euler(euler);
		Rigidbody component = GetComponent<Rigidbody>();
		component.maxAngularVelocity = 9999f;
		component.AddRelativeTorque(component.angularVelocity * 2000f);
		component.angularVelocity = Vector3.zero;
	}
}
