using UnityEngine;
using System.Collections;

public class AsteroidManager : MonoBehaviour {

	public GameObject asteroid;
	private float startTime;
	private float asteroidGenDelta = 1f;

	void Start () {
		startTime = Time.time;	
	}

	// Update is called once per frame
	void Update () {
		//Periodically generate asteroids with random pos and dir
		if (Time.time - startTime > asteroidGenDelta) {
			startTime= Time.time;
			generateAsteroid(1f);
		}	
	}

	//Generate an asteroid at a desired size
	void generateAsteroid(float scaleFactor){
		GameObject shot = Instantiate(asteroid, generateRandomPosition(), Quaternion.identity) as GameObject;
		shot.transform.localScale=new Vector3 (shot.transform.localScale.x * scaleFactor, shot.transform.localScale.y 
               * scaleFactor, shot.transform.localScale.z * scaleFactor);
		Asteroid script = shot.GetComponent<Asteroid>();
		script.generateRandomDirection();
	}

	Vector3 generateRandomPosition(){
		//Calculate bounds from the camera
		float yBound = Camera.main.orthographicSize;  
		float xBound = yBound * Screen.width / Screen.height;

		//Generate a random position within these bounds
		float yPos = Random.Range (-yBound, yBound);
		float xPos = Random.Range (-xBound, xBound);
		Vector3 randPos = new Vector3 (xPos, yPos, 1f);
		return randPos;
	}
}
