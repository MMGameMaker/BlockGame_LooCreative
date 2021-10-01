using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button pause;

    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private Button reset;

    [SerializeField] private GameObject grid;

 //   public Text gameOver;

    private void Start()
    {
        SetPauScreenOff();
    }

    private void SetPauScreenOn()
    {
        pauseScreen.SetActive(true);
        reset.enabled = true;
    }

    private void SetPauScreenOff()
    {
        pauseScreen.SetActive(false);
        reset.enabled = false;
    }

    public void OnPauseClick()
    {
        SetPauScreenOn();
        GridManager.Instance.PauseGame();
    }

    public void OnResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
