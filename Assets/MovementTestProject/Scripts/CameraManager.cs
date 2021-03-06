using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	InputManager inputManager;
	StateManager stateManager;

	public Transform targetTransform; // camera follow object
	public Transform cameraPivot; // object that camera pivots
	public Transform cameraTransform; // the transform of the camera object
	public LayerMask collisionLayers; // layers that the camera will collide with
	private float defaultPosition;
	private Vector3 cameraFollowVelocity = Vector3.zero;
	private Vector3 cameraVectorPosition;

	public float cameraCollisionOffset = 0.2f; // how much the camera will jump off of objects at collision
	public float minimumCollisionOffset = 0.2f;
	public float cameraCollisionRadius = 0.2f;
	public float cameraFollowSpeed = 0.2f;
	public float cameraLookSpeed = 2;
	public float cameraPivotSpeed = 2;

	public float lookAngle; // camera up and down
	public float pivotAngle; // camera left and right
	public float minimumPivotAngle = -35;
	public float maximumPivotAngle = 35;

	private void Awake()
	{
		inputManager = FindObjectOfType<InputManager>();
		stateManager = FindObjectOfType<StateManager>();
		targetTransform = FindObjectOfType<StateManager>().transform;
		cameraTransform = Camera.main.transform;
		defaultPosition = cameraTransform.localPosition.z;
	}

	public void HandleALlCameraMovement()
	{
		FollowTarget();
		RotateCamera();
		HandleCameraCollisions();
	}

	private void FollowTarget()
	{
		Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

		transform.position = targetPosition;
	}

	private void RotateCamera()
	{
		Vector3 rotation;
		Quaternion targetRotation;

		pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed); // vertical

		if (!stateManager.isFlying) // grounded camera controls
		{
			lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed); // horizontal
			pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);
		}
		else if (stateManager.isFlying) 
		{
			//if (Vector3.Dot(cameraTransform.up, Vector3.down) > 0) // invert left right when camera is upside down
			//{
			//	lookAngle = lookAngle + (-inputManager.cameraInputX * cameraLookSpeed);
			//}
			//else
			//{
			//	lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
			//}
			lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
		}

		rotation = Vector3.zero;
		rotation.y = lookAngle;
		targetRotation = Quaternion.Euler(rotation);
		transform.rotation = targetRotation;

		rotation = Vector3.zero;
		rotation.x = pivotAngle;
		targetRotation = Quaternion.Euler(rotation);
		cameraPivot.localRotation = targetRotation;
	}

	private void HandleCameraCollisions()
	{
		float targetPosition = defaultPosition;
		RaycastHit hit;
		Vector3 direction = cameraTransform.position - cameraPivot.position;
		direction.Normalize();

		if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers)) 
		{
			float distance = Vector3.Distance(cameraPivot.position, hit.point);
			targetPosition =- (distance - cameraCollisionOffset);
		}

		if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
		{
			targetPosition = targetPosition - minimumCollisionOffset;
		}

		cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
		cameraTransform.localPosition = cameraVectorPosition;
	}
}
