using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void startGame()
    {
        game_controller_script.GAME_CONTROLLER.startGame();
    }
    public void menu()
    {
        SceneManager.LoadScene("menu");
    }
}
