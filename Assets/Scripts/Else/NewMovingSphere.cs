using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.InputSystem;


public enum StickDir
{
	INVALID,
	LEFT,
	RIGHT,
	FRONT,
	BACK
}

public class NewMovingSphere : MonoBehaviour {

	[SerializeField]
	private Transform playerInputSpace = default;

	[SerializeField, Range(0f, 100f)]
	private float maxSpeed = 10f, maxClimbSpeed = 4f;

	[SerializeField, Range(0f, 100f)]
	private float
		maxAcceleration = 10f,
		maxAirAcceleration = 1f,
		maxClimbAcceleration = 40f;

	[SerializeField, Range(0f, 10f)]
	private float jumpHeight = 2f;

	[SerializeField, Range(0, 5)]
	private int maxAirJumps = 0;

	[SerializeField, Range(0, 90)]
	private float maxGroundAngle = 25f, maxStairsAngle = 50f;

	[SerializeField, Range(90, 170)]
	private float maxClimbAngle = 140f;

	[SerializeField, Range(0f, 100f)]
	private float maxSnapSpeed = 100f;

	[SerializeField, Min(0f)]
	private float probeDistance = 1f;

	[SerializeField]
	private LayerMask probeMask = -1, stairsMask = -1, climbMask = -1;

	[SerializeField]
	public Material normalMaterial = default, climbingMaterial = default;

	[HideInInspector]
	public Rigidbody body, connectedBody, previousConnectedBody;

	private Vector2 playerInput;

	private Vector3 velocity, connectionVelocity;

	private Vector3 connectionWorldPosition, connectionLocalPosition;
	
	private Vector3 upAxis, rightAxis, forwardAxis;

	private bool desiredJump, desiresClimbing;

	private Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

	private int groundContactCount, steepContactCount, climbContactCount;

	private bool OnGround => groundContactCount > 0;

	private bool OnSteep => steepContactCount > 0;

	private bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

	private int jumpPhase;

	private float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

	private int stepsSinceLastGrounded, stepsSinceLastJump;

	private MeshRenderer meshRenderer;

	public TerrainGenerator terrainGenerator;

	private PlayerStateMachine stm;

    public int playerIndex = 0;
	public Vector3 startPosition;

	public ParticleSystem starParticles;
	public GameObject collisionParticles;

	private Vector3 lastPosition = Vector3.zero;
	[SerializeField]
	private GameObject visualObj;

	private OrbitCamera cam;
	private Vector3 lastRelativeCameraPosition;

	public Vector3 hitPosition;
	public GameObject fallingRockPrefab;

	private float lastShootTime = 0;
	private float shootInterval = 1;

	private void OnValidate ()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
		minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
	}

	private void Awake ()
	{
		cam = playerInputSpace.gameObject.GetComponent<OrbitCamera>();

		body = GetComponent<Rigidbody>();
		body.useGravity = false;
		meshRenderer = GetComponent<MeshRenderer>();
		OnValidate();

		terrainGenerator = FindObjectOfType<TerrainGenerator>();
		startPosition = transform.position;
		stm = new PlayerStateMachine(this);
		stm.Init();
		DisableSelf();
	}

	public void EnableSelf()
    {
		stm.SwitchState("Reset");
    }

	public void DisableCamera()
    {
		cam.cameraMode = CameraMode.TRACKING;
		lastRelativeCameraPosition = cam.transform.position - transform.position;
    }

	public void EnableCamera()
    {
		cam.cameraMode = CameraMode.CONNECTED;
		cam.transform.position = transform.position + lastRelativeCameraPosition;
	}

	public void OnMove(InputAction.CallbackContext ctx) => playerInput = ctx.ReadValue<Vector2>();

	public void OnJump(InputAction.CallbackContext ctx) => desiredJump |= true;

	public void OnRelease(InputAction.CallbackContext ctx)
	{
		if (stm != null)
        {
			if (stm.IsCurrentState("Dizzy")) desiresClimbing = false;
			else desiresClimbing = ctx.performed;
		}
		else
        {
			desiresClimbing = ctx.performed;
		}
		
	}

	public void OnShoot(InputAction.CallbackContext ctx)
    {
		if (Time.time > lastShootTime + shootInterval)
        {
			lastShootTime = Time.time;
			var rock = Instantiate(fallingRockPrefab).GetComponent<FallingRock>();
			rock.shouldRespawn = false;
			rock.transform.position = transform.position + Vector3.up * 2;
			rock.transform.localScale = Vector3.one * 0.5f;
			var rb = rock.gameObject.GetComponent<Rigidbody>();
			rb.AddForce(playerInputSpace.forward * 50, ForceMode.Impulse);
		}
    }

	private void Update ()
	{
		if (stm.IsCurrentState("OutOfBounds")) return;

		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		if (playerInputSpace)
		{
			rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
		}
		else
		{
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}

		//meshRenderer.material = Climbing ? climbingMaterial : normalMaterial;
	}

	public void DisableSelf()
    {
		stm.SwitchState("Disabled");
    }

	public void ResetPosition()
    {
		transform.position = startPosition;
		body.velocity = Vector3.zero;
	}

	private void FixedUpdate ()
	{
		stm.Tick();
		if (stm.IsCurrentState("Disabled")) return;

		Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
		
		UpdateState();
		AdjustVelocity();

		if (desiredJump)
		{
			desiredJump = false;
			Jump(gravity);
		}

		if (Climbing)
		{
			velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
		}
		else if (OnGround && velocity.sqrMagnitude < 0.01f)
		{
			velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
		}
		else if (desiresClimbing && OnGround)
		{
			velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
		}
		else
		{
			velocity += gravity * Time.deltaTime;
		}

		body.velocity = velocity;
		ClearState();
		UpdateVisuals();
	}

	private bool CheckDirection(Vector3 direction)
    {
		var leftObjs = Physics.RaycastAll(transform.position, direction, .51f);
		for (int i = 0; i < leftObjs.Length; i++)
		{
			if (leftObjs[i].transform.gameObject.layer == 14)
			{
				return true;
			}
		}

		return false;
    }

	private void UpdateVisuals()
    {
		StickDir dir = StickDir.INVALID;
		Vector3 movementDir = (transform.position - lastPosition) * 100;

		var xRotDir = -Vector3.forward;
		var yRotDir = Vector3.zero;
		var zRotDir = Vector3.right;

		if (CheckDirection(Vector3.left)) dir = StickDir.LEFT;
		else if (CheckDirection(Vector3.right)) dir = StickDir.RIGHT;
		else if (CheckDirection(Vector3.forward)) dir = StickDir.FRONT;
		else if (CheckDirection(Vector3.back)) dir = StickDir.BACK;

		switch(dir)
        {
			default:
				break;
			case StickDir.LEFT:
				xRotDir = Vector3.zero;
				yRotDir = Vector3.forward;
				zRotDir = -Vector3.up;
				break;
			case StickDir.RIGHT:
				xRotDir = Vector3.zero;
				yRotDir = -Vector3.forward;
				zRotDir = Vector3.up;
				break;
			case StickDir.FRONT:
				xRotDir = -Vector3.up;
				yRotDir = Vector3.right;
				zRotDir = Vector3.zero;
				break;
			case StickDir.BACK:
				xRotDir = Vector3.up;
				yRotDir = -Vector3.right;
				zRotDir = Vector3.zero;
				break;
        }

		if (xRotDir != Vector3.zero) visualObj.transform.RotateAround(transform.position, xRotDir, movementDir.x);
		if (yRotDir != Vector3.zero) visualObj.transform.RotateAround(transform.position, yRotDir, movementDir.y);
		if (zRotDir != Vector3.zero) visualObj.transform.RotateAround(transform.position, zRotDir, movementDir.z);

		lastPosition = transform.position;
	}

	public void GetHitAt(Vector3 _hitPosition)
    {
		hitPosition = _hitPosition;
		stm.SwitchState("Dizzy");
		//collisionParticles.SetActive(true);
		//collisionParticles.transform.position = hitPosition;
		//await Task.Delay(2000);
		//collisionParticles.SetActive(false);
	}

	public async void DisableClimbing(float duration)
    {
		float timer = Time.realtimeSinceStartup;
		meshRenderer.material = normalMaterial;

		float currentTime = 0;

		float emitInterval = .2f;
		float lastEmitTime = 0;

		while (currentTime < duration)
        {
			currentTime = Time.realtimeSinceStartup - timer;

			if (currentTime - lastEmitTime > emitInterval)
            {
				starParticles.Emit(1);
				lastEmitTime = currentTime;
            }

			desiresClimbing = false;
			await Task.Yield();
        }

		meshRenderer.material =  climbingMaterial;
		//await Task.Yield();
	}

	private void ClearState ()
	{
		groundContactCount = steepContactCount = climbContactCount = 0;
		contactNormal = steepNormal = climbNormal = Vector3.zero;
		connectionVelocity = Vector3.zero;
		previousConnectedBody = connectedBody;
		connectedBody = null;
	}

	private void UpdateState ()
	{
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;
		if (CheckClimbing() || OnGround || SnapToGround() || CheckSteepContacts())
		{
			stepsSinceLastGrounded = 0;
			if (stepsSinceLastJump > 1)
			{
				jumpPhase = 0;
			}
			if (groundContactCount > 1)
			{
				contactNormal.Normalize();
			}
		}
		else
		{
			contactNormal = upAxis;
		}
		
		if (connectedBody)
		{
			if (connectedBody.isKinematic || connectedBody.mass >= body.mass) {
				UpdateConnectionState();
			}
		}
	}

	private void UpdateConnectionState()
	{
		if (connectedBody == previousConnectedBody)
		{
			Vector3 connectionMovement =
				connectedBody.transform.TransformPoint(connectionLocalPosition) -
				connectionWorldPosition;
			connectionVelocity = connectionMovement / Time.deltaTime;
		}
		connectionWorldPosition = body.position;
		connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
			connectionWorldPosition
		);
	}

	private bool CheckClimbing()
	{
		if (Climbing)
		{
			if (climbContactCount > 1)
			{
				climbNormal.Normalize();
				float upDot = Vector3.Dot(upAxis, climbNormal);
				if (upDot >= minGroundDotProduct)
				{
					climbNormal = lastClimbNormal;
				}
			}
			groundContactCount = 1;
			contactNormal = climbNormal;
			return true;
		}
		return false;
	}

	private bool SnapToGround()
	{
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
		{
			return false;
		}
		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed)
		{
			return false;
		}
		if (!Physics.Raycast(
			body.position, -upAxis, out RaycastHit hit,
			probeDistance, probeMask
		))
		{
			return false;
		}

		float upDot = Vector3.Dot(upAxis, hit.normal);
		if (upDot < GetMinDot(hit.collider.gameObject.layer))
		{
			return false;
		}

		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f)
		{
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}
		connectedBody = hit.rigidbody;
		return true;
	}

	private bool CheckSteepContacts()
	{
		if (steepContactCount > 1)
		{
			steepNormal.Normalize();
			float upDot = Vector3.Dot(upAxis, steepNormal);
			if (upDot >= minGroundDotProduct)
			{
				steepContactCount = 0;
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	private void AdjustVelocity()
	{
		float acceleration, speed;
		Vector3 xAxis, zAxis;
		if (Climbing)
		{
			acceleration = maxClimbAcceleration;
			speed = maxClimbSpeed;
			xAxis = Vector3.Cross(contactNormal, upAxis);
			zAxis = upAxis;
		}
		else
		{
			acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
			speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed;
			xAxis = rightAxis;
			zAxis = forwardAxis;
		}

		xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
		zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);
		
		Vector3 relativeVelocity = velocity - connectionVelocity;
		float currentX = Vector3.Dot(relativeVelocity, xAxis);
		float currentZ = Vector3.Dot(relativeVelocity, zAxis);

		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX = Mathf.MoveTowards(currentX, playerInput.x * speed, maxSpeedChange);
		float newZ = Mathf.MoveTowards(currentZ, playerInput.y * speed, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}

	private void Jump (Vector3 gravity)
	{
		Vector3 jumpDirection;
		if (OnGround)
		{
			jumpDirection = contactNormal;
		}
		else if (OnSteep)
		{
			jumpDirection = steepNormal;
			jumpPhase = 0;
		}
		else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)
		{
			if (jumpPhase == 0)
			{
				jumpPhase = 1;
			}
			jumpDirection = contactNormal;
		}
		else
		{
			return;
		}

		stepsSinceLastJump = 0;
		jumpPhase += 1;
		float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
		jumpDirection = (jumpDirection + upAxis).normalized;
		float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
		if (alignedSpeed > 0f)
		{
			jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
		}
		velocity += jumpDirection * jumpSpeed;
	}

	private void OnCollisionEnter (Collision collision)
	{
		EvaluateCollision(collision);
	}

	private void OnCollisionStay (Collision collision)
	{
		EvaluateCollision(collision);
	}

	private void EvaluateCollision (Collision collision)
	{
		int layer = collision.gameObject.layer;
		float minDot = GetMinDot(layer);
		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			float upDot = Vector3.Dot(upAxis, normal);
			if (upDot >= minDot)
			{
				groundContactCount += 1;
				contactNormal += normal;
				connectedBody = collision.rigidbody;
			}
			else {
				if (upDot > -0.01f)
				{
					steepContactCount += 1;
					steepNormal += normal;
					if (groundContactCount == 0)
					{
						connectedBody = collision.rigidbody;
					}
				}
				if (
					desiresClimbing && upDot >= minClimbDotProduct &&
					(climbMask & (1 << layer)) != 0
				)
				{
					climbContactCount += 1;
					climbNormal += normal;
					lastClimbNormal = normal;
					connectedBody = collision.rigidbody;
				}
			}
		}
	}

	private Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}

	private float GetMinDot (int layer)
	{
		return (stairsMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
	}
}
