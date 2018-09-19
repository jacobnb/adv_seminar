﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class game_controller_script : MonoBehaviour {
	public static game_controller_script GAME_CONTROLLER;
	public int player1Score, player2Score;
	Text player1Text, player2Text;
	void Awake(){
		if(!GAME_CONTROLLER){
			player1Score=0;
			player2Score=0;
			GAME_CONTROLLER = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (GAME_CONTROLLER != this){
			Destroy(gameObject);
			GAME_CONTROLLER.Start();
		}
	}
	// Use this for initialization
	void Start () {
		player1Text = GameObject.Find("Score 1").GetComponent<Text>();
		player2Text = GameObject.Find("Score 2").GetComponent<Text>();
		Debug.Assert(player1Text && player2Text);
		updateScore();
	}
	void updateScore(){
		player1Text.text = player1Score.ToString();
		player2Text.text = player2Score.ToString();
	}

	public void playerWon(int playerNum){
		if(playerNum == 1){
			player1Score++;
		}
		else if(playerNum ==2){
			player2Score++;
		}
		else{
			Debug.LogError("Unknown Player Number");
		}
		StartCoroutine("loadScene1", 2f);
		updateScore();
	}

	IEnumerator loadScene1(float delay){
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }
}
