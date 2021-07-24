using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	public enum CameraState
	{
		Player,
		PlayerDeath,
		Spectate
	}

	public Transform player;

	public Vector3 offset;

	public Vector3 desyncOffset;

	public Vector3 vaultOffset;

	private Camera cam;

	private Rigidbody rb;

	public PlayerInput playerInput;

	public bool cinematic;

	private float desiredTilt;

	private float tilt;

	private Vector3 desiredDeathPos;

	private Transform target;

	private Vector3 desiredSpectateRotation;

	private Transform playerTarget;

	public LayerMask whatIsGround;

	private int spectatingId;

	private Vector3 desiredBob;

	private Vector3 bobOffset;

	private float bobSpeed = 15f;

	private float bobMultiplier = 1f;

	private readonly float bobConstant = 0.2f;

	public Camera mainCam;

	public Camera gunCamera;

	public static MoveCamera Instance { get; private set; }

	public CameraState state { get; set; }

	private void Start()
	{
		Instance = this;
		cam = base.transform.GetChild(0).GetComponent<Camera>();
		rb = PlayerMovement.Instance.GetRb();
		UpdateFov(CurrentSettings.Instance.fov);
		Debug.LogError("updating fov: " + CurrentSettings.Instance.fov);
	}

	private void LateUpdate()
	{
		if (state == CameraState.Player)
		{
			PlayerCamera();
		}
		else if (state == CameraState.PlayerDeath)
		{
			PlayerDeathCamera();
		}
		else if (state == CameraState.Spectate)
		{
			SpectateCamera();
		}
	}

	public void PlayerRespawn(Vector3 pos)
	{
		base.transform.position = pos;
		state = CameraState.Player;
		base.transform.parent = null;
		CancelInvoke("SpectateCamera");
	}

	public void PlayerDied(Transform ragdoll)
	{
		target = ragdoll;
		state = CameraState.PlayerDeath;
		desiredDeathPos = base.transform.position + Vector3.up * 3f;
		if (GameManager.state != GameManager.GameState.GameOver)
		{
			Invoke("StartSpectating", 4f);
		}
	}

	private void StartSpectating()
	{
		if (GameManager.state != GameManager.GameState.GameOver && PlayerStatus.Instance.IsPlayerDead())
		{
			target = null;
			state = CameraState.Spectate;
			PPController.Instance.Reset();
		}
	}

	private void SpectateCamera()
	{
		if (!target)
		{
			foreach (PlayerManager value2 in GameManager.players.Values)
			{
				if (!(value2 == null) && !value2.dead)
				{
					target = new GameObject("cameraOrbit").transform;
					playerTarget = value2.transform;
					base.transform.parent = target;
					base.transform.localRotation = Quaternion.identity;
					base.transform.localPosition = new Vector3(0f, 0f, -10f);
					spectatingId = value2.id;
				}
			}
			if (!target)
			{
				return;
			}
		}
		Vector2 vector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		desiredSpectateRotation += new Vector3(0f - vector.y, vector.x, 0f) * 1.5f;
		if (Input.GetKeyDown(InputManager.rightClick))
		{
			SpectateToggle(1);
		}
		else if (Input.GetKeyDown(InputManager.leftClick))
		{
			SpectateToggle(-1);
		}
		target.position = playerTarget.position;
		target.rotation = Quaternion.Lerp(target.rotation, Quaternion.Euler(desiredSpectateRotation), Time.deltaTime * 10f);
		Vector3 direction = base.transform.position - target.position;
		float value;
		if (Physics.Raycast(target.position, direction, out var hitInfo, 10f, whatIsGround))
		{
			Debug.DrawLine(target.position, hitInfo.point);
			value = 10f - hitInfo.distance + 0.8f;
			value = Mathf.Clamp(value, 0f, 10f);
		}
		else
		{
			value = 0f;
		}
		base.transform.localPosition = new Vector3(0f, 0f, -10f + value);
	}

	private void SpectateToggle(int dir)
	{
		int num = spectatingId;
		for (int i = 0; i < GameManager.players.Count; i++)
		{
			if (!GameManager.players.ContainsKey(i) || GameManager.players[i] == null)
			{
				continue;
			}
			PlayerManager playerManager = GameManager.players[i];
			if (!(playerManager == null) && !playerManager.dead)
			{
				if (dir > 0 && playerManager.id > num)
				{
					spectatingId = playerManager.id;
					playerTarget = playerManager.transform;
					break;
				}
				if (dir < 0 && playerManager.id < num)
				{
					spectatingId = playerManager.id;
					playerTarget = playerManager.transform;
					break;
				}
			}
		}
	}

	private void PlayerDeathCamera()
	{
		if (!(target == null))
		{
			base.transform.position = Vector3.Lerp(base.transform.position, desiredDeathPos, Time.deltaTime * 1f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(target.position - base.transform.position), Time.deltaTime);
		}
	}

	private void PlayerCamera()
	{
		UpdateBob();
		MoveGun();
		base.transform.position = player.transform.position + bobOffset + desyncOffset + vaultOffset + offset;
		if (!cinematic)
		{
			Vector3 cameraRot = playerInput.cameraRot;
			cameraRot.x = Mathf.Clamp(cameraRot.x, -90f, 90f);
			base.transform.rotation = Quaternion.Euler(cameraRot);
			desyncOffset = Vector3.Lerp(desyncOffset, Vector3.zero, Time.deltaTime * 15f);
			vaultOffset = Vector3.Slerp(vaultOffset, Vector3.zero, Time.deltaTime * 7f);
			if (PlayerMovement.Instance.IsCrouching())
			{
				desiredTilt = 6f;
			}
			else
			{
				desiredTilt = 0f;
			}
			tilt = Mathf.Lerp(tilt, desiredTilt, Time.deltaTime * 8f);
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.z = tilt;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
		}
	}

	private void MoveGun()
	{
		if ((bool)rb && !(Mathf.Abs(rb.velocity.magnitude) < 4f) && PlayerMovement.Instance.grounded)
		{
			PlayerMovement.Instance.IsCrouching();
		}
	}

	public void UpdateFov(float f)
	{
		mainCam.fieldOfView = f;
		gunCamera.fieldOfView = f;
	}

	public void BobOnce(Vector3 bobDirection)
	{
		Vector3 vector = ClampVector(bobDirection * 0.15f, -3f, 3f);
		desiredBob = vector * bobMultiplier;
	}

	private void UpdateBob()
	{
		desiredBob = Vector3.Lerp(desiredBob, Vector3.zero, Time.deltaTime * bobSpeed * 0.5f);
		bobOffset = Vector3.Lerp(bobOffset, desiredBob, Time.deltaTime * bobSpeed);
	}

	private Vector3 ClampVector(Vector3 vec, float min, float max)
	{
		return new Vector3(Mathf.Clamp(vec.x, min, max), Mathf.Clamp(vec.y, min, max), Mathf.Clamp(vec.z, min, max));
	}
}
