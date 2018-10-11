using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class menu_controller : MonoBehaviour {
    [SerializeField]
    TextMeshProUGUI sliderValue;
    [SerializeField]
    Slider slider;

    private void Start()
    {
        slider.value = game_controller_script.GAME_CONTROLLER.scoreToWin;
        sliderValue.SetText(slider.value.ToString("0"));

    }
    public void startGame()
    {
        game_controller_script.GAME_CONTROLLER.startGame();
    }
    public void adjustHealth(float newHealth)
    {
        game_controller_script.GAME_CONTROLLER.setHealth(newHealth);
    }
    public void adjustScore()
    {
        sliderValue.SetText(slider.value.ToString("0"));
        game_controller_script.GAME_CONTROLLER.scoreToWin = (int)slider.value;
    }
}
