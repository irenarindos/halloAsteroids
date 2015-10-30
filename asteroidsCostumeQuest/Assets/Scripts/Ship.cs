using UnityEngine;
using System.Collections.Generic;

public class Ship : Wrappable {
	private  CharacterController controller;
	private float speed = 10f;
	float rotationSpeed = 200f;

	public Rigidbody projectile;
	private float projectileSpeed = 500f;

	void Start(){
		controller = GetComponent<CharacterController>();
		setBounds();
	}
	
	void Update() {
		Vector3 velocity = Vector3.zero;

		//Rotate around x-axis
		transform.Rotate(Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0.0f, 0.0f);

		//Calculate velocity along local ship's y-axis
		velocity = transform.up * Input.GetAxis("Vertical") * speed;
		controller.Move (velocity * Time.deltaTime);   
		checkBounds ();

		if(Input.GetButtonUp("Shoot"))
		{
			Rigidbody shot = Instantiate(projectile, transform.position+transform.up, transform.rotation) as Rigidbody;
			shot.AddForce(transform.up * projectileSpeed);
		}
	}


	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "asteroid") {
			//Debug.Log ("It's a hit!");		
			
			//Deal with asteroid and ship collisions
		}
	}

	
}
