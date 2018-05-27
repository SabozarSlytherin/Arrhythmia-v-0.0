using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//public float walkSpeed = 6f;
	//public float runSpeed = 15f;
	public float gravity = -25f;
	public float speedSmoothTime = 0.1f;
	public float jumpHeight = 5f;
	[Range(0,1)]
	public float airControlPercent;
	float targetSpeed;
	float animSpeed;

	public float velocityY;
	float speedSmoothVelocity;
	static float currentSpeed;

	//moje

	bool ledgeHor = false;
	bool ledgeKosi = false;
	Transform ledgeTransform;

	[Range(0, 35)]
	float zaFormulu;

	Animator Animator;
	float speedPercent;
	float speedPercent2;
	bool uSkoku;
	bool skocio;
    public bool isGrounded;

	public float ledgeGrabDistanceHor = 3.5f;
	public float ledgeGrabDistanceUkoso = 3f;
	bool ispredivice = false;
	bool naivici = false;

	//otkucaji za srce
	public float heartBeats = 500f;
	public float jedan = 1;

	//kraj mojeg

	public float turnSmoothTime = 0.2f;
	public float turnSmoothVelocity;

	Transform cameraT;
	CharacterController controller;

	public float getCurrentSpeed(){
		return currentSpeed;
	}

	// Use this for initialization
	void Start () {
		cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController> ();

		//beat (heartBeats);

		/*System.Timers.Timer srceTimer = new System.Timers.Timer ();
		srceTimer.Elapsed += new System.Timers.ElapsedEventHandler ();
		srceTimer.Interval=1000;
		srceTimer.Enabled=true;*/

		Animator = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 inputDir = input.normalized;
		bool running = Input.GetKey (KeyCode.LeftShift);

		Move (inputDir, running);

		//moje
		if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
			uSkoku = true;
		} else {
			uSkoku = false;
		}

		//"SKOCIO" SE TRIGGERUJE KAD PRITISNES SPACE(I CONTROLLER.ISGROUNDED JE TRUE), A "USKOKU" DOK SI U VAZDUHU(TJ DOK JE CONTROLLER.ISGROUNDED = FALSE)

		if (velocityY == 0 && Input.GetKey (KeyCode.Space)) {
			skocio = true;
		} else if (velocityY > 0) {
			skocio = false;
		} else {
			skocio = false;
		}

		if (skocio) {
			Animator.Play ("jump1");
		}

		Animator.SetBool ("skocio", skocio);
		Animator.SetBool ("skok", uSkoku);

		animSpeed = ((running) ? 35f : 6f) * inputDir.magnitude;
		Animator.SetFloat ("speedPercent", animSpeed);

		//kraj mojeg

		if (Input.GetKeyDown (KeyCode.Space)) {
			Jump();
		}

		/*if (Animator.GetBool ("skok")) {
			Animator.Play ("jump1");
		}*/

		if (Input.GetKey (KeyCode.E)) {
			LedgeScan ();
		}

		if (Input.GetKey (KeyCode.K)) {
			controller.transform.position = Vector3.zero;
		}

		Debug.Log ("Beats remaining: " + heartBeats);

		

        if (groundTouch())
        {
            
            isGrounded = true;
            Animator.SetBool("uSkoku", !isGrounded);
        }
        else
        {
            isGrounded = false;
            Animator.SetBool("uSkoku", !isGrounded);
        }
        
    }
    public Ray drawRayDown()
    {
        Vector3 bla = controller.transform.position;
        Vector3 pelvisRay1 = new Vector3(bla.x, bla.y + 1f, bla.z);
        Vector3 pelvisRay2 = Vector3.down + new Vector3(0, -0.5f, 0);
        Debug.DrawRay(pelvisRay1, pelvisRay2, Color.magenta);
        Ray rayDown = new Ray(pelvisRay1, pelvisRay2);
        return rayDown;

    }

    public bool groundTouch()
    {
        Ray rayDown = drawRayDown();
        RaycastHit hitGround;
        if (Physics.Raycast(rayDown, out hitGround) && hitGround.collider.tag == "Ground" && hitGround.distance < 2.75f)
        {
                return true;
        }
        else { return false; }


    }


    void FixedUpdate (){
		brojacZaJenduSekundu ();
	}

	//SRCE \/    \/    \/    \/    \/

	float pom = 1f;
	public void brojacZaJenduSekundu(){
		if (pom >= 100f) {
			if (getRunSpeed() == 0f) {
				beatMirovanje ();
				pom = 1f;
			} else if (getRunSpeed() <= 0.5f) {
				beatHodanje ();
				pom = 1f;
			} else if (getRunSpeed() > 0.5f) {
				beatSprint ();
				pom = 1f;
			}
		} else {
			pom += 1f;
		}
	}

	public void beatMirovanje(){
		heartBeats--;
	}

	public void beatHodanje(){
		heartBeats -= 2f;
	}

	public void beatSprint(){
		heartBeats -= 3f;
	}

	static float getRunSpeed(){
		if (currentSpeed == 0f) {
			return 0;
		} else if(currentSpeed <= 6f){
			return 0.5f;
		}else{
			return 1f;
		}
	}

	//SRCE /\    /\    /\    /\    /\

	//moje
	public void LedgeScan(){

		//Vector iz kojeg krecu rayovi za skeniranje za litice/stvari za koje mozes da se uhvatis
		Vector3 trenutniTransform = controller.transform.position;
		Vector3 origin = new Vector3 (trenutniTransform.x, trenutniTransform.y + 6, trenutniTransform.z);

		//"donji" ray
		Ray donjiRay = new Ray (origin, transform.forward);
		Debug.DrawRay (origin, transform.forward, Color.red);

		//"gornji ray"
		Ray gornjiRay = new Ray (origin, transform.forward + transform.up);
		Debug.DrawRay (origin, transform.forward + transform.up, Color.blue);


		Vector3 preIvice = new Vector3 (0, -2f, -2f);
		Vector3 nakonIvice = new Vector3 (0, 1, 0);
		Quaternion NormalnaRotacija = controller.transform.rotation;
		Quaternion RotacijaNaIvici = Quaternion.Euler (origin.x - 20, origin.y, origin.z);
   
		//float pom = velocityY;
		RaycastHit hitDonji;
		if (Physics.Raycast (donjiRay, out hitDonji, ledgeGrabDistanceHor)) {
			if (hitDonji.collider.tag == "Ledge") {
				Debug.Log ("Ispred tebe se nalazi ivica!(DONJI ray)");
				ispredivice = true;
				ledgeHor = true;
				ledgeKosi = false;
			} else {
				ispredivice = false;
			}
		}

		RaycastHit hitGornji;
		if (Physics.Raycast (gornjiRay, out hitGornji, ledgeGrabDistanceUkoso)) {
			if (hitGornji.collider.tag == "Ledge") {
				Debug.Log ("Ispred tebe se nalazi ivica!(GORNJI Ray)");
				ispredivice = true;
				ledgeKosi = true;
				ledgeHor = false;
			} else {
				ispredivice = false;
			}
		}

		if (Input.GetKey (KeyCode.E) && (ledgeHor && !(ledgeKosi))) {
			//ledgeTransform = new Vector3 (hitDonji.collider.gameObject.transform.position.x, hitDonji.collider.gameObject.transform.position.y, hitDonji.collider.gameObject.transform.position.z);
			Debug.Log ("vracam DONJI");
			ledgeTransform = hitDonji.transform;
			ledgeHor = false;
			ledgeKosi = false;
		}

		if (Input.GetKey (KeyCode.E) && (ledgeKosi && !(ledgeHor))) {
			Debug.Log ("vracam GORNJI");
			ledgeTransform = hitGornji.transform;
			ledgeHor = false;
			ledgeKosi = false;
		}

		if (ispredivice && Input.GetKey (KeyCode.E)) {
			Debug.Log ("Drzis se za ivicu");
			naivici = true;
			ispredivice = false;
			if (naivici && Input.GetKey (KeyCode.E)) {
				velocityY = 0;
				controller.transform.position = ledgeTransform.position + new Vector3 (0, -1, 0);
				Animator.Play ("hang1");
				bool visi = Animator.GetBool ("visi");
				Animator.SetBool ("visi", visi);
				/*if (Input.GetKey (KeyCode.R)) {
					controller.transform.position = ledgeTransform.GetComponent<Collider>().transform.position + nakonIvice;
					controller.transform.rotation = NormalnaRotacija;
				}
			} else {
				controller.transform.rotation = NormalnaRotacija;
			}
			velocityY = 0;
			controller.transform.SetPositionAndRotation (origin, RotacijaNaIvici);
			// controller.transform.position = hit.collider.transform.position + preIvice;*/
			} else {
				naivici = false;
			}
		}
	}

	float formula(float brzina){
		float rez; 
		float rez2;
		if (brzina <= 7f) {
			rez = 0.08333f * brzina;
			if (brzina >= 5.9f && brzina < 7f) {
				return 0.5f;
			}
			return rez;
		} else{
			rez2 = 0.01724f * brzina + 0.5f;
			if (rez2 > 1f) {
				return 1f;
			}
			return rez2;
		}
	}

	//kraj mojeg

	/*void LedgeScan(){
	 * 
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
	}*/

	float Walk(float wspeed){
		if (wspeed < 6f) {
			return wspeed + 0.2f;
		}
		else if ( wspeed >= 6f)
		{
			return 6f ;
		}

		else {
			return 0f;
		}
	}

	void Move(Vector2 inputDir, bool running){
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		}

		velocityY += 3 * (Time.deltaTime * gravity);
		//float pom;
		//float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		targetSpeed = ((running) ? ((targetSpeed > 35f) ? 35f : targetSpeed +0.8f)  :((targetSpeed > 6f)? ((targetSpeed <=7.3f)?6f:targetSpeed - 1.2f): Walk(targetSpeed))) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
		controller.Move (velocity * Time.deltaTime);
		currentSpeed = new Vector2 (controller.velocity.x, controller.velocity.z).magnitude;

		if (controller.isGrounded) {
			velocityY = 0;
		}

		Animator.SetFloat ("speedPercent2", formula(targetSpeed));
	}

	void Jump(){
		if (isGrounded) {
			Debug.Log ("Sada si u skoku.");
			float jumpVelocity = 2.0f * (Mathf.Sqrt (-2 * gravity * jumpHeight));
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