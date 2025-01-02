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
        AudioManager.Instance.PlaySound("click");
        SceneManager.LoadScene(1);
    }
    public void HowToPlay()
    {
        AudioManager.Instance.PlaySound("click");
        howToPlay.SetActive(true);
        credits.SetActive(false);
    }
    public void Credits()
    {
        AudioManager.Instance.PlaySound("click");
        howToPlay.SetActive(false);
        credits.SetActive(true);
    }
    public void CloseUI()
    {
        AudioManager.Instance.PlaySound("click");
        howToPlay.SetActive(false);
        credits.SetActive(false);
    }
    public void QuitGame()
    {
        AudioManager.Instance.PlaySound("click");
        Application.Quit();
    }
}
