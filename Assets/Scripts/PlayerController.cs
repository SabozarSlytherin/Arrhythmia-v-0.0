using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 6f;
	public float runSpeed = 15f;
	public float gravity = -25f;
	public float speedSmoothTime = 0.1f;
	public float jumpHeight = 5f;
	[Range(0,1)]
	public float airControlPercent;

	//moje

	Animator Animator;
	float speedPercent;
	bool uSkoku;
	bool skocio;

	public float ledgeGrabDistance = 1f;
	bool ispredivice = false;
	bool naivici = false;
	//kraj mojeg

	public float velocityY;
	float speedSmoothVelocity;
	float currentSpeed;

	public float turnSmoothTime = 0.2f;
	public float turnSmoothVelocity;

	Transform cameraT;
	CharacterController controller;

	// Use this for initialization
	void Start () {
		cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController> ();

		Animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 inputDir = input.normalized;
		bool running = Input.GetKey (KeyCode.LeftShift);

		Move (inputDir, running);

		//moje
		if (controller.isGrounded) {
			uSkoku = false;
		} else {
			uSkoku = true;
		}
			
		if (velocityY == 0 && Input.GetKey (KeyCode.Space)) {
			skocio = true;
		} else if(controller.isGrounded){
			skocio = false;
		}

		if (skocio) {
			Animator.Play ("jump1");
		}

		Animator.SetBool ("skocio", skocio);
		Animator.SetBool ("skok", uSkoku);

		float animSpeed = ((running) ? 1 : .5f) * inputDir.magnitude;
		Animator.SetFloat ("speedPercent", animSpeed);

		//kraj mojeg

		if (Input.GetKeyDown (KeyCode.Space)) {
			Jump();
		}

		/*if (Animator.GetBool ("skok")) {
			Animator.Play ("jump1");
		}*/

		LedgeScan ();

		if (Input.GetKey (KeyCode.K)) {
			controller.transform.position = Vector3.zero;
		}
	}

	//moje
	/*void LegdeScan(){

		Vector3 bla = controller.transform.position;
		Vector3 bla2 = new Vector3 (bla.x, bla.y + 2, bla.z + 1);
		Ray r = new Ray (bla, bla2);

		Vector3 preIvice = new Vector3 (0, -0.5f, -0.5f);
		Vector3 nakonIvice = new Vector3 (0, 1, 0);
		Quaternion NormalnaRotacija = controller.transform.rotation;
		Quaternion RotacijaNaIvici = Quaternion.Euler (bla.x-20, bla.y, bla.z);

		float pom = velocityY;

		RaycastHit hit;

		if (Physics.Raycast(r, out hit,ledgeGrabDistance)) {
			if (hit.collider.tag == "Ledge") {
				Debug.Log ("Ispred tebe se nalazi ivica!");
				ispredivice = true;
			}
			if (ispredivice && Input.GetKey (KeyCode.E)) {
				Debug.Log ("Drzis se za ivicu");
				naivici = true;
				ispredivice = false;

				if (naivici) {
					velocityY = 0;
					controller.transform.SetPositionAndRotation (hit.transform.position + new Vector3(bla.x, -1, -1), RotacijaNaIvici);
					controller.transform.gameObject.transform.position = hit.transform.position + preIvice;
					if (Input.GetKey (KeyCode.R)) {
						controller.transform.position = hit.collider.transform.position + nakonIvice;
						controller.transform.rotation = NormalnaRotacija;
					}
				} else {
					controller.transform.rotation = NormalnaRotacija;
					velocityY = pom;
				}
				// /*velocityY = 0;
				controller.transform.SetPositionAndRotation (bla, RotacijaNaIvici);
				// controller.transform.position = hit.collider.transform.position + preIvice;
			}else{
				naivici = false;
				controller.transform.rotation = NormalnaRotacija;
			}
		}
	}*/
	//kraj mojeg

	void LedgeScan(){

		/*Vector3 bla = controller.transform.position;
		Vector3 pocetakGornjegRaya = new Vector3 (bla.x, bla.y + 1.75f, bla.z);
		Vector3 krajGornjegRaya = new Vector3 (bla.x, bla.y + 3.05f, bla.z + 1);
		Ray gornjiRay = new Ray (pocetakGornjegRaya, new Vector3 (bla.x, bla.y + 1.3f, bla.z + 1f));*/

		Vector3 bla = controller.transform.position;
		Vector3 v1Kraj = new Vector3 (bla.x, bla.y + 1.3f, bla.z + 1f);
		Quaternion v1q = Quaternion.Euler (v1Kraj);
		Ray v1 = new Ray (bla, v1Kraj);
		v1.direction = v1q.eulerAngles;
		Debug.DrawRay (bla, v1Kraj, Color.magenta);

		Vector3 v2Kraj = new Vector3 (bla.x + bla.x * 2, bla.y + 2*bla.y, bla.z + 3f + bla.z *2);
		Ray v2 = new Ray (bla, v2Kraj);
		Debug.DrawRay (bla, v2Kraj, Color.red);

		//Vector3 pocetakDonjegRaya = new Vector3(bla.x, bla.y + 1.75f, bla.z)
	}

	void drawVector(Ray r){
		
	}

	void Move(Vector2 inputDir, bool running){
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		}

		velocityY += Time.deltaTime * gravity;

		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
		controller.Move (velocity * Time.deltaTime);
		currentSpeed = new Vector2 (controller.velocity.x, controller.velocity.z).magnitude;

		if (controller.isGrounded) {
			velocityY = 0;
		}
	}

	void Jump(){
		if (controller.isGrounded) {
			Debug.Log ("Sada si u skoku.");
			float jumpVelocity = Mathf.Sqrt (-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	float GetModifiedSmoothTime(float smoothTime){
		if (controller.isGrounded) {
			return smoothTime;
		}
		if (airControlPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;
	}
}
