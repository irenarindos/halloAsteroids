using UnityEngine;
using System.Collections;

public class Asteroid : Wrappable {
	private Vector3 direction; 
	private float speed = 2f;
	public static AsteroidManager manager;
	public bool isDebris = false;

	// Use this for initialization
	void Start () {
		setBounds ();
		direction = generateRandomDirection ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, 0f);
		checkBounds ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "shot") {
			if(!isDebris){
				manager.generateDebris(transform.position);
			}

			//Destroy the shot and this asteroid
			Destroy(other.gameObject);
			Destroy(gameObject);
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
