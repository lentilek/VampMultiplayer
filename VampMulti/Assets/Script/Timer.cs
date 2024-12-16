using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    public float timeMax;
    public float currentTime;
    [SerializeField] private TextMeshProUGUI timerTXT;
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
        StopAllCoroutines();
        currentTime = timeMax;
    }
    public void StartTiming()
    {
        StartCoroutine(Timing());
    }
    IEnumerator Timing()
    {
        do
        {
            Display();
            yield return new WaitForSeconds(1f);
            currentTime--;
        } while (currentTime >= 0);
    }
    private void Display()
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerTXT.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
