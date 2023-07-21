using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text pointsText; // TMP_Text for displaying the score

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = "SCORE: " + score.ToString(); // Display the score on the Game Over screen
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Reload the current scene
    }
    public void CharacterSelect()
    {
        SceneManager.LoadScene("CharSelect"); // Reload the current scene
    }
    public void Play()
    {
        SceneManager.LoadScene("SnakeGame"); // Reload the current scene
    }
    public void Instruction()
    {
        SceneManager.LoadScene("Instruction"); // Reload the current scene
    }

    public void ExitButton()
    {
        Application.Quit(); // Quit the game (for standalone builds)
    }
}
