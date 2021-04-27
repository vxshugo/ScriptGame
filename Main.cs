using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public player player;
    public Text coinText;
    public Image[] hearts;
    public Sprite isLife, nonLife;
    public GameObject PauseScreen;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    float timer = 0f;
    public  Text timeText;
    public Soundeffector soundeffector;
    public AudioSource musicSource, soundSource;

    public void ReloadLvl() // Когда персонаж умирает перезапускает сцену
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Start()
    {
        musicSource.volume = (float)PlayerPrefs.GetInt("MusicVolume") / 9;
        soundSource.volume = (float)PlayerPrefs.GetInt("SoundVolume") / 9;
    }
    public void Update() 
    {
        coinText.text = player.GetCoins().ToString();

        for (int i = 0; i < hearts.Length; i++){
            if (player.GetHP() > i)
                hearts[i].sprite = isLife;
            else
                hearts[i].sprite = nonLife;
        }
        //timer
        timer += Time.deltaTime;
        timeText.text = timer.ToString("F2").Replace(",", ":");

        
    }

    public void PauseOn(){
        Time.timeScale = 0f;
        player.enabled = false;
        PauseScreen.SetActive(true);
        musicSource.volume = 0;
    }

    public void PauseOff(){
        Time.timeScale = 1f;
        player.enabled = true;
        PauseScreen.SetActive(false);
        musicSource.volume = musicSource.volume = (float)PlayerPrefs.GetInt("MusicVolume") / 9;
    }

    public void Win()
    {
        musicSource.volume = 0;
        soundeffector.PlayWinSound();
        Time.timeScale = 0f;
        player.enabled = false;
        WinScreen.SetActive(true);

        if(!PlayerPrefs.HasKey("Lvl") || PlayerPrefs.GetInt("Lvl") < SceneManager.GetActiveScene().buildIndex)
        {
            PlayerPrefs.SetInt("Lvl", SceneManager.GetActiveScene().buildIndex);
        }
        

        if(PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + player.GetCoins());
        else
            PlayerPrefs.SetInt("coins", player.GetCoins());
        
    }
    public void Lose()
    {
        musicSource.volume = 0;
        soundeffector.PlayLoseSound();
        Time.timeScale = 0f;
        player.enabled = false;
        LoseScreen.SetActive(true);
    }

    public void MenuLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene("Menu");
    }

    public void NextLvl(){
        musicSource.volume = musicSource.volume = (float)PlayerPrefs.GetInt("MusicVolume") / 9;
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
