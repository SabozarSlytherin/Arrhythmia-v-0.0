using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

	float yaw;
	float pitch;

	public bool lockCursor;

	Vector2 pitchminmax = new Vector2 (-40, 85);

	public float mouseSensitivity = 5f;
	public Transform target;
	public float distanceFromTarget; // = 6f;

	public PlayerController pc;

	public float rotationSmoothTime = 0.12f;
	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;

	void LateUpdate () {
		yaw += Input.GetAxis ("Mouse X") * mouseSensitivity;
		pitch -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
		pitch = Mathf.Clamp (pitch, pitchminmax.x, pitchminmax.y);

		currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
		transform.eulerAngles = currentRotation;

		transform.position = target.position + new Vector3(0, 5, 0) - transform.forward * distanceFromTarget;
	}

	void DynamicCameraSpeed(PlayerController pc){
		if (!Input.GetKey (KeyCode.LeftShift)) {
			distanceFromTarget = 10f;
		} else {
			distanceFromTarget = 15f;
		}
	}

	void Update(){
		DynamicCameraSpeed (pc);
	}

	void Start(){
		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
