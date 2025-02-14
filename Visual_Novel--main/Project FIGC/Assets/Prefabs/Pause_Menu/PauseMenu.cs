using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public TextMeshProUGUI bot1, bot2;
    public Color txtColor;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (GameIsPaused)
        //    {
        //        Resume();
        //    }
          
        //}
       

    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause()
    {
        bot1.color = txtColor;
        bot1.color = txtColor;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(0);
        Debug.Log("Load Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quiting the game");
        Application.Quit();
    }

    public void CambiarColor()
    {
        bot1.color = Color.white;
        
    }
    public void ColorOriginal()
    {
        bot1.color = txtColor;
        
    }
    public void CambiarColor2()
    {
       
        bot2.color = Color.white;
    }
    public void ColorOriginal2()
    {
       
        bot2.color = txtColor;
    }


}
