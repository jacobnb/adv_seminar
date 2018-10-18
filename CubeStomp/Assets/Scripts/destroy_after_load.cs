using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_after_load : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 1f);
	}

}
