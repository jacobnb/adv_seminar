using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

enum Scenes
{
    GAME = 0,
    START = 1,
    MENU = 2,
    LEVEL_ONE = 3,
}

public class game_controller_script : MonoBehaviour {
	public static game_controller_script GAME_CONTROLLER;
	public int player1Score, player2Score;
    public int scoreToWin = 3; //public to be accessed in menu
    int currentScene;
    int nextSceneToLoad;
	Text player1Text, player2Text;
    Canvas uiCanvas;
    GameObject loadingScreen;
    GameObject winScreen;
    player_move player1, player2; //replace with messaging system?

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
        uiCanvas = GameObject.Find("UI").GetComponent<Canvas>();
        loadingScreen = GameObject.Find("Loading Screen");
        winScreen = GameObject.Find("Win Screen");
		Debug.Assert(player1Text && player2Text);
		updateScore();
        showUI(false);
        showLoadingScreen(false);
        showWinScreen(false);
        nextSceneToLoad = (int)Scenes.LEVEL_ONE;
        currentScene = (int)Scenes.START;

        //replace with messaging system
        player1 = GameObject.Find("Player1").GetComponent<player_move>();
        player2 = GameObject.Find("Player2").GetComponent<player_move>();
    }
    void updateScore(){
		player1Text.text = player1Score.ToString();
		player2Text.text = player2Score.ToString();
	}

    void showUI(bool shouldShow)
    {
        uiCanvas.enabled = shouldShow;
    }
    void showLoadingScreen(bool shouldShow)
    {
        if(loadingScreen)
            loadingScreen.SetActive(shouldShow);
    }

    void showWinScreen(bool shouldShow, string playerNumber = "ERROR")
    {
        if (shouldShow)
        {
            winScreen.SetActive(true);
            winScreen.GetComponentInChildren<TextMeshProUGUI>().SetText("Player " + playerNumber + "Wins!");
        }
        else
        {
            winScreen.SetActive(false);
        }
    }

    void playerWon(int playerNum)
    {
        showWinScreen(true, playerNum.ToString());
    }
	public void playerLost(int playerNum){
		if(playerNum == 2){
			player1Score++;
		}
		else if(playerNum ==1){
			player2Score++;
		}
		else{
			Debug.LogError("Unknown Player Number");
		}
		if(player1Score >= scoreToWin)
        {
            playerWon(1);   
        }
        else if (player2Score >= scoreToWin)
        {
            playerWon(2);
        }
        else
        {
            updateScore();
            nextScene();
        }
    }


	IEnumerator loadNextScene(float delay = 0f){
        if (currentScene != nextSceneToLoad)
        {
            yield return new WaitForSeconds(delay);
            showLoadingScreen(true);
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene); //wait for this to finish and add a loading screen;
            Debug.Log("Unloaded Scene" + currentScene);
            yield return unload; //wait for scene to be unloaded. 
                                 //https://stackoverflow.com/questions/50502394/how-can-i-wait-for-a-scene-to-unload
            SceneManager.LoadScene(nextSceneToLoad, LoadSceneMode.Additive);
            showLoadingScreen(false);
            currentScene = nextSceneToLoad;
        }
        levelLoaded();
    }

    void levelLoaded()
    {
        player1.GameStarted();
        player2.GameStarted();
    }
    void nextScene()
    {
        nextSceneToLoad++;
        nextSceneToLoad -= (int) Scenes.LEVEL_ONE;
        nextSceneToLoad = nextSceneToLoad % (SceneManager.sceneCountInBuildSettings - (int) Scenes.LEVEL_ONE);
        nextSceneToLoad += (int)Scenes.LEVEL_ONE;

        Debug.Log(nextSceneToLoad);
        StartCoroutine(loadNextScene());
    }

    //un-load old scene
    //load new scene.

    public void startGame()
    {
        showUI(true);
        StartCoroutine(loadNextScene());
    }

}
