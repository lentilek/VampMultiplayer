using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject howToPlay;
    [SerializeField] private GameObject credits;
    private void Awake()
    {
        howToPlay.SetActive(false);
        credits.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void HowToPlay()
    {
        howToPlay.SetActive(true);
        credits.SetActive(false);
    }
    public void Credits()
    {
        howToPlay.SetActive(false);
        credits.SetActive(true);
    }
    public void CloseUI()
    {
        howToPlay.SetActive(false);
        credits.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
