using UnityEngine;
using System.Collections;

public class ship : MonoBehaviour {
	private  CharacterController controller;
	float speed = 10f;
	float rotationSpeed = 200f;
	private float yBound, xBound;

	void Start(){
		controller = GetComponent<CharacterController>();

		//Calculate bounds from the camera
		yBound = Camera.main.orthographicSize;  
		xBound = yBound * Screen.width / Screen.height;

		//Adjust bounds according to ship dimensions
		Renderer rend = GetComponent<Renderer>();
		Vector3 extents = rend.bounds.extents;
		yBound += extents.y;
		xBound += extents.x;
		//Debug.Log ("vert: " + yBound + " horz: " + xBound);
	}
	
	void Update() {
		Vector3 velocity = Vector3.zero;

		//Rotate around x-axis
		transform.Rotate(Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0.0f, 0.0f);

		//Calculate velocity along local ship's y-axis
		velocity = transform.up * Input.GetAxis("Vertical") * speed;
		controller.Move (velocity * Time.deltaTime);   
		checkBounds ();
	}

	//Ensure ship position wraps if it breaches a boundary
	void checkBounds(){
		if (transform.position.x > xBound)
			transform.position=new Vector3(-xBound, transform.position.y, 1f);
		if (transform.position.y > yBound)
			transform.position=new Vector3(transform.position.x, -yBound, 1f);
		if (transform.position.x < -xBound)
			transform.position=new Vector3(xBound, transform.position.y, 1f);
		if (transform.position.y < -yBound)
			transform.position=new Vector3(transform.position.x, yBound, 1f);
	}
	
}
