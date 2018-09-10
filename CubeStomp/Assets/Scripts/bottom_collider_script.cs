using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bottom_collider_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("Bottom Script");
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	//=== neither of these are working===//
	void OnTriggerEnter2D(Collider2D col){
		Debug.Log("Bottom Collider Colliding");
	}

	void OnColliderEnter2D(Collider2D col){
		Debug.Log("Bottom Collider 2");
	}
}
