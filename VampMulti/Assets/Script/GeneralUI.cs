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
    }
}
