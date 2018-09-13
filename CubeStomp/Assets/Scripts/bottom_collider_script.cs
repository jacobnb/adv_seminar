using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bottom_collider_script : MonoBehaviour {
    player_move playerScript;
	// Use this for initialization
	void Start () {
        playerScript = transform.parent.gameObject.GetComponent<player_move>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player")){
			Debug.Log("Hit Player");
		}
        playerScript.canJump = true;
        playerScript.doubleJump = true;
    }
}
