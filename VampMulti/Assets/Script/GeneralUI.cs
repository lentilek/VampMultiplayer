using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralUI : MonoBehaviour
{
    public static GeneralUI Instance;

    [SerializeField] private GameObject gameEnd;
    [SerializeField] private GameObject won;
    [SerializeField] private GameObject lost;
    [HideInInspector] public bool endGame;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        endGame = false;
        gameEnd.SetActive(false);
        won.SetActive(false);
        lost.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void GameEnd()
    {
        gameEnd.SetActive(true);        
        //lost.SetActive(true);
        endGame = true;
    }
    public void Win()
    {
        lost.SetActive(false);
        won.SetActive(true);
    }
}
