using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    private int currentLevel;
    private int maxLevel;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        maxLevel = 2;//şu an iki tane olduğu için
        DontDestroyOnLoad(this.gameObject);
        GetLevel();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetLevel()
    {
        currentLevel = PlayerPrefs.GetInt("keyLevel", 1);//yüklü level yoksa 1. leveli getir
        LoadLevel();
    }

    public void LoadLevel()
    {
        string levelName = "LevelScene" + currentLevel;
        SceneManager.LoadScene(levelName);
    }

    public void NextLevel()
    {
        currentLevel++;
        if(currentLevel > maxLevel)
        {
            currentLevel = 1;
        }
        PlayerPrefs.SetInt("keyLevel", currentLevel);//current leveli kaydet
        LoadLevel();
    }
}
