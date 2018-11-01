using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This controls the little off screen icons (that aren't needed currently)
public class pointer_script : MonoBehaviour {
	public float topOfScreen = 3f;
	public Transform player;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		stayAtTopOfScreen();
		enableIfPlayerOffScreen();
	}
	void enableIfPlayerOffScreen(){
		if (player.position.y > topOfScreen+0.5){
			gameObject.GetComponent<SpriteRenderer>().enabled = true;
		}
		else{
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
		}
	}
	void stayAtTopOfScreen(){
		transform.position = new Vector3(player.position.x, topOfScreen, transform.position.z);
	}
}
