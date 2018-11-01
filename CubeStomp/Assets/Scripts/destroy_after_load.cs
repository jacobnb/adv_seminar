using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Probably used to destroy a particle system that's instantiated?
public class destroy_after_load : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 1f);
	}

}
