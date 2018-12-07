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
    [SerializeField]
    private float timeToSuddenDeath = 30;
    [SerializeField]
    private float suddenDeathTimer;
    private float prevPlayerHP; //to reset after sudden death.
    private bool inSuddenDeath = false;
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
    PolygonCollider2D cameraBounds;

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
        cameraBounds = GameObject.Find("CameraBounds").GetComponent<PolygonCollider2D>();
        Debug.Assert(player1Text && player2Text && cameraBounds);
        updateScore();
        showUI(false); 
        showLoadingScreen(false);
        showWinScreen(false);
        nextSceneToLoad = (int)Scenes.LEVEL_ONE;
        currentScene = (int)Scenes.START;
        suddenDeathTimer = timeToSuddenDeath;
        //if dev.
        //currentScene = firstLevelToLoad;
        //showUI(true);
        ////cinemachine_script.enableCinemachine(true);

        //end if dev

        SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
        
        //replace with messaging system
        player1 = GameObject.Find("Player1").GetComponent<player_move>();
        player2 = GameObject.Find("Player2").GetComponent<player_move>();
    }

    private void Update()
    {
        checkForSuddenDeath();
    }
    private void checkForSuddenDeath()
    {
        suddenDeathTimer -= Time.deltaTime;
        if(suddenDeathTimer <= 0 && !inSuddenDeath)
        {
            suddenDeath();
        }
    }
    private void suddenDeath()
    {
        inSuddenDeath = true;
        player1.suddenDeathDrain();
        player2.suddenDeathDrain();
    }
    private void TRUEsuddenDeath() //sudden death implementation that sets player health to 1.
    {
        inSuddenDeath = true;
        setHealth(1f); //caches health in prevPlayerHP
        player1.GameStarted();
        player2.GameStarted();
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
        prevPlayerHP = player1.setHealth(newHealth);
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
        updateScore();
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
    void updateCameraBounds()
    {
        if (camera_bounds_script.BOUNDS_SCRIPT)
        {
            var points = camera_bounds_script.BOUNDS_SCRIPT.getBoundsPoints();
            if (points != null)
            {
                cameraBounds.points = points;
            }
        }
        else
        {
            Debug.LogError("Failed Camera Update");
        }
    }
    void setWinScreen(int winPlayer)
    {
        Sprite sprite = null;
        if(winPlayer == 1)
        {
            var rect = new Rect(0, 0, player1Winning.width, player1Winning.height);
            var pivot = Vector2.zero;//new Vector2(player1Winning.width / 2.0f, player1Winning.height / 2.0f);
            sprite = Sprite.Create(player1Winning, rect, pivot);
        }
        else if (winPlayer == 2)
        {
            var rect = new Rect(0, 0, player2Winning.width, player2Winning.height);
            var pivot = new Vector2(player2Winning.width / 2.0f, player2Winning.height / 2.0f);
            sprite = Sprite.Create(player2Winning, rect, pivot);
        }
        winScreen.GetComponent<Image>().sprite = sprite;
    }

    void setLoadingScreen(int winPlayer)
    {
        Sprite sprite = null;
        
        if (winPlayer == 1)
        {
            if (!player1Winning)
            {
                //set a default picture here.
                return;
            }
            var posit = new Vector2(0, 0);
            var size = new Vector2(player1Winning.width, player1Winning.height); //935x526
            var rect = new Rect(posit, size);
            //pixel size problems?
            var pivot = Vector2.zero;//new Vector2(player1Winning.width / 2.0f, player1Winning.height / 2.0f);
            sprite = Sprite.Create(player1Winning, rect, pivot);
        }
        else if (winPlayer == 2)
        {
            if (!player2Winning)
            {
                //set a default picture here.
                return;
            }
            var rect = new Rect(0, 0, player2Winning.width, player2Winning.height);
            var pivot = Vector2.zero;//new Vector2(player2Winning.width / 2.0f, player2Winning.height / 2.0f);
            sprite = Sprite.Create(player2Winning, rect, pivot);
        }
        loadingScreen.GetComponent<Image>().sprite = sprite;
    }
    public void captureScreen(int losingPlayer)
    {
        StartCoroutine(getTexture(losingPlayer));
    }
    IEnumerator getTexture(int losingPlayer)
    {
        yield return null;
        showUI(false);
        yield return new WaitForEndOfFrame();
        if (losingPlayer == 1)
        {
            player2Winning = ScreenCapture.CaptureScreenshotAsTexture();
        }
        else if (losingPlayer == 2)
        {
            player1Winning = ScreenCapture.CaptureScreenshotAsTexture();
        }
        showUI(true);
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
                yield return new WaitForSeconds(0.1f);
                updateCameraBounds();
                cinemachine_script.enableCinemachine(true);
            }
            currentScene = nextSceneToLoad;
        }
        else
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
        suddenDeathTimer = timeToSuddenDeath;
        inSuddenDeath = false;
        //enable the following if using TRUEsuddenDeath;
        //if (inSuddenDeath)
        //{
        //    setHealth(prevPlayerHP);
        //    inSuddenDeath = false;
        //}
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
