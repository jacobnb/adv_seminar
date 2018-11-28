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
    [Header("Dev Vars")]
    [SerializeField] //for dev
    private int firstLevelToLoad;
    [Tooltip("singleton variable")]
    public static game_controller_script GAME_CONTROLLER;
    int player1Score, player2Score;
    public int scoreToWin = 3; //public to be accessed in menu
    int currentScene;
    int nextSceneToLoad;

    [Header("References")]
    TextMeshProUGUI player1Text, player2Text;
    [SerializeField]
    Canvas uiCanvas;
    [SerializeField]
    //Disables cinemachine on load (start screen)
    private disable_cinemachine cinemachine_script;
    GameObject loadingScreen;
    GameObject winScreen;
    player_move player1, player2; //replace with messaging system?
    Texture2D player1Winning, player2Winning;

    void Awake() {
        if (!GAME_CONTROLLER) {
            player1Score = 0;
            player2Score = 0;
            GAME_CONTROLLER = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (GAME_CONTROLLER != this) {
            Destroy(gameObject);
            GAME_CONTROLLER.Start();
        }
    }
    void Start() {

        uiCanvas.enabled = true;
        player1Text = GameObject.Find("Score 1").GetComponent<TextMeshProUGUI>();
        player2Text = GameObject.Find("Score 2").GetComponent<TextMeshProUGUI>();
        loadingScreen = GameObject.Find("Loading Screen");
        winScreen = GameObject.Find("Win Screen");
        Debug.Assert(player1Text && player2Text);
        updateScore();
        showUI(false); 
        showLoadingScreen(false);
        showWinScreen(false);
        nextSceneToLoad = (int)Scenes.LEVEL_ONE;
        currentScene = (int)Scenes.START;

        //if dev.
        currentScene = firstLevelToLoad;
        showUI(true);
        //cinemachine_script.enableCinemachine(true);

        //end if dev

        SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
        
        //replace with messaging system
        player1 = GameObject.Find("Player1").GetComponent<player_move>();
        player2 = GameObject.Find("Player2").GetComponent<player_move>();
    }
    void updateScore() {
        player1Text.SetText(player1Score.ToString());
        player2Text.SetText(player2Score.ToString());
    }

    void showUI(bool shouldShow)
    {
        uiCanvas.enabled = shouldShow;
    }
    void showLoadingScreen(bool shouldShow)
    {
        if (loadingScreen)
            loadingScreen.SetActive(shouldShow);
    }


    public void setHealth(float newHealth)
    {
        player1.setHealth(newHealth);
        player2.setHealth(newHealth);
    }
    
    public void playerLost(int playerNum) {
        if (playerNum == 2) {
            player1Score++;
            setLoadingScreen(1);
        }
        else if (playerNum == 1) {
            player2Score++;
            setLoadingScreen(2);
        }
        else {
            Debug.LogError("Unknown Player Number");
        }
        if (player1Score >= scoreToWin)
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
    void playerWon(int playerNum)
    {
        setWinScreen(playerNum);
        showWinScreen(true, playerNum.ToString());
    }
    void showWinScreen(bool shouldShow, string playerNumber = "ERROR")
    {
        if (shouldShow)
        {
            winScreen.SetActive(true);
            winScreen.GetComponentInChildren<TextMeshProUGUI>().SetText("Player " + playerNumber + " Wins!");
        }
        else
        {
            winScreen.SetActive(false);
        }
    }

    void setWinScreen(int playerNum)
    {
        Sprite sprite = null;
        if(playerNum == 1)
        {
            var rect = new Rect(0, 0, player1Winning.width, player1Winning.height);
            var pivot = Vector2.zero;//new Vector2(player1Winning.width / 2.0f, player1Winning.height / 2.0f);
            sprite = Sprite.Create(player1Winning, rect, pivot);
        }
        else if (playerNum == 2)
        {
            var rect = new Rect(0, 0, player1Winning.width, player1Winning.height);
            var pivot = new Vector2(player1Winning.width / 2.0f, player1Winning.height / 2.0f);
            sprite = Sprite.Create(player1Winning, rect, pivot);
        }
        winScreen.GetComponent<Image>().sprite = sprite;
    }

    void setLoadingScreen(int playerNum)
    {
        Sprite sprite = null;
        if (playerNum == 1)
        {
            var rect = new Rect(0, 0, player1Winning.width, player1Winning.height);
            var pivot = new Vector2(player1Winning.width / 2.0f, player1Winning.height / 2.0f);
            sprite = Sprite.Create(player1Winning, rect, pivot);
        }
        else if (playerNum == 2)
        {
            var rect = new Rect(0, 0, player1Winning.width, player1Winning.height);
            var pivot = new Vector2(player1Winning.width / 2.0f, player1Winning.height / 2.0f);
            sprite = Sprite.Create(player1Winning, rect, pivot);
        }
        loadingScreen.GetComponent<Image>().sprite = sprite;
    }
    public void captureScreen(int playerNum)
    {
        if(playerNum == 1)
        {
            player1Winning = ScreenCapture.CaptureScreenshotAsTexture();
        }
        else if(playerNum == 2)
        {
            player2Winning = ScreenCapture.CaptureScreenshotAsTexture();
        }
    }

    IEnumerator loadNextScene(float delay = 0f) {
        //if scene not already loaded.
        if (currentScene != nextSceneToLoad)
        {
            yield return new WaitForSeconds(delay);
            showLoadingScreen(true);
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentScene); 
            yield return unload; //wait for scene to be unloaded. 
            //https://stackoverflow.com/questions/50502394/how-can-i-wait-for-a-scene-to-unload
            SceneManager.LoadScene(nextSceneToLoad, LoadSceneMode.Additive);
            if (currentScene == (int)Scenes.START || currentScene == (int)Scenes.MENU)
            {
                levelLoaded();
            }
            if(nextSceneToLoad == (int)Scenes.START || nextSceneToLoad == (int)Scenes.MENU)
            {
                cinemachine_script.enableCinemachine(false);
            }
            else
            {
                cinemachine_script.enableCinemachine(true);
            }
            currentScene = nextSceneToLoad;
        }
        else// if (currentScene != (int)Scenes.START || currentScene != (int)Scenes.MENU)
        {
            showLoadingScreen(true);
        }
    }

    public void continueGame(){ //Continue from loading screen
        //set == in loadNextScene.
        if(currentScene == nextSceneToLoad)
        {
            levelLoaded();
        }
    }

    void levelLoaded()
    {
        player1.GameStarted();
        player2.GameStarted();
        showLoadingScreen(false);
    }
    void nextScene()
    {
        nextSceneToLoad++;
        nextSceneToLoad -= (int) Scenes.LEVEL_ONE;
        nextSceneToLoad = nextSceneToLoad % (SceneManager.sceneCountInBuildSettings - (int) Scenes.LEVEL_ONE);
        nextSceneToLoad += (int)Scenes.LEVEL_ONE;

        StartCoroutine(loadNextScene());
    }

    public void loadMenu()
    {
        nextSceneToLoad = (int)Scenes.MENU;
        StartCoroutine(loadNextScene());
    }

    public void startGame()
    {

        nextSceneToLoad = (int)Scenes.LEVEL_ONE;
        showUI(true);
        StartCoroutine(loadNextScene());

        //reset
        player1Score = 0;
        player2Score = 0;
        updateScore();
        showUI(true);
        showWinScreen(false);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
