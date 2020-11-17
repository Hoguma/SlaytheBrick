using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIevent : MonoBehaviour
{
    private bool pauseOn = false;
    public GameObject pausePanel;
    

    public void ActivePauseBotton()
    {
        if (!pauseOn)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1.0f;
            pausePanel.SetActive(false);
        }


        pauseOn = !pauseOn;
    }
    
    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
    

    public void ExitButton()
    {
        Debug.Log("게임을 종료합니다");
        Application.Quit();
    }
}