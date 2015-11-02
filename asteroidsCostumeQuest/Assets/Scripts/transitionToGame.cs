using UnityEngine;
using System.Collections;

public class transitionToGame : MonoBehaviour {

	void Update () {
		if(Input.GetButtonUp("Shoot")){
			Application.LoadLevel("asteroidsGameplay");
		}	
	}
}
