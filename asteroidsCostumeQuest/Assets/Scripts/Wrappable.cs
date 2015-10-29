using UnityEngine;
using System.Collections;

//Class to handle screen wrapping of objects

public class Wrappable : MonoBehaviour {

	protected float yBound, xBound;

	protected void setBounds () {
		//Calculate bounds from the camera
		yBound = Camera.main.orthographicSize;  
		xBound = yBound * Screen.width / Screen.height;

		//Adjust bounds according to mesh dimensions
		Renderer renderer= GetComponent<Renderer>();
		
		Vector3 extents = renderer.bounds.extents;
		yBound += extents.y;
		xBound += extents.x;
	}
	
	//Ensure position wraps if it breaches a boundary
	protected void checkBounds(){
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
