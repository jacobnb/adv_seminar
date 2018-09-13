using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class side_colliders_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

		void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Player")){
            col.GetComponent<player_move>().touching_enemySide = true;
		}
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<player_move>().touching_enemySide = false;
        }
    }
}
