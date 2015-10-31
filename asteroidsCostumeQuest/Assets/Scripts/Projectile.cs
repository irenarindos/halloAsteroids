using UnityEngine;
using System.Collections;

public class Projectile : Wrappable {
	private float instantiateTime;
	private float lifespan = 1f;

	void Start(){
		setBounds ();
		instantiateTime = Time.time;
	}
	
	void Update () {	
		//Destroy shot after lifespan is up
		if (Time.time - instantiateTime > lifespan)
			Destroy (gameObject);

		checkBounds ();
	}
}
