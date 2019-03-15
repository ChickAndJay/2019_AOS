using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
    #region Singleton
    public static GameManagerScript instance = null;

    public GameObject[] _characters = new GameObject[3];
    public int _selectedCharacterIdx;

    int _ourScore;
    int _enemyScore;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FirstMapScene")
        {
            AllocatePlayers();
        }
    }

    public void AllocatePlayers()
    {
        GameObject player = Instantiate(_characters[_selectedCharacterIdx]);
        player.transform.position = MapAllocator.instance._ourPos[Random.Range(0, 3)].transform.position;
    }

    public void IncreaseOurScore()
    {
        _ourScore++;
        if(_ourScore == 10)
        {

        }
    }

    public void DecreaseOurScore(int amount)
    {
        _ourScore-= amount;        
    }

    public void IncreaseEnemyScore()
    {
        _enemyScore++;
        if (_enemyScore == 10)
        {

        }
    }

    public void DecreaseEnemyScore(int amount)
    {
        _enemyScore -= amount;
    }
}
