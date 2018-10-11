using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons_script : MonoBehaviour {
    public void startGame()
    {
        game_controller_script.GAME_CONTROLLER.startGame();
    }
    public void menu()
    {
        SceneManager.LoadScene("menu");
    }


}
