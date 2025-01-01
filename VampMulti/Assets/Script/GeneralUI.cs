using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralUI : MonoBehaviour
{
    public static GeneralUI Instance;

    [SerializeField] private GameObject gameEnd;
    [SerializeField] private GameObject won;
    [SerializeField] private GameObject lost;
    [HideInInspector] public bool endGame;

    [SerializeField] public GameObject hubBig;
    [SerializeField] public GameObject[] playerUI;
    [SerializeField] public TextMeshProUGUI[] playerUIPoints;
    [SerializeField] public GameObject[] playerAvatars;
    public TextMeshProUGUI[] playerBulletsTXT;
    public GameObject[] winnerImages;
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
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject go = FindObjectOfType<Spawner>().gameObject;
            Destroy(go);
            SceneManager.LoadScene(0);
        }
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
