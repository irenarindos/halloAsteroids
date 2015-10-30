using UnityEngine;
using System.Collections;

public class Asteroid : Wrappable {
	private Vector3 direction; 
	private float speed = 2f;

	// Use this for initialization
	void Start () {
		setBounds ();
		direction = generateRandomDirection ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, 0f);
		checkBounds ();
	//	setScale (.9f);
	}

	//Scale asteroid
	void setScale(float factor){
		transform.localScale = new Vector3 (transform.localScale.x * factor, transform.localScale.y * factor, transform.localScale.z * factor);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "shot") {
			Debug.Log ("It's a hit!");		
			
			//Destroy the shot and this asteroid
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
		if (other.gameObject.tag == "Player") {
			Debug.Log ("We hit the player!");
		}
	}

	//Generate a normalized random direction
	public Vector3 generateRandomDirection(){
		Vector3 dir= Vector3.zero;

		while(dir == Vector3.zero){
			dir= Random.onUnitSphere;
			//Discard z component
			dir.z = 0;
		}

		dir.Normalize ();
		return dir;
	}
}
