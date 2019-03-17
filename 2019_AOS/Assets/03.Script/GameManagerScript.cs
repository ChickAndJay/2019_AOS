using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
    #region Singleton
    public static GameManagerScript instance = null;

    public GameObject[] _characters = new GameObject[3];
    public GameObject[] _AICharacters = new GameObject[3];

    public GameObject[] _spawnedEnemy { get; private set; }
    public GameObject[] _spawnedCompany { get; private set; }

    public int _selectedCharacterIdx;

    int _ourScore;
    int _enemyScore;
    bool _isOurWinning;
    bool _isEnemyWinning;

    public GameObject _centerObject;

    Coroutine _startCountRoutine;

    AudioSource _audioSource;
    public AudioClip _LobbySound;
    public AudioClip _InGameSound;    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
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
        if (scene.name == "LobbyScene")
        {
            _audioSource.clip = _LobbySound;
            _audioSource.loop = true;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
        }
        else if (scene.name == "FirstMapScene")
        {
            _audioSource.clip = _InGameSound;
            _audioSource.loop = true;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
            AllocatePlayers();
        }
    }

    public void AllocatePlayers()
    {
        _isEnemyWinning = false;
        _isOurWinning = false;
        _ourScore = 0;
        _enemyScore = 0;

        _spawnedCompany = new GameObject[3];
        _spawnedEnemy = new GameObject[3];

        GameObject player = Instantiate(_characters[_selectedCharacterIdx]);
        player.transform.position =
            MapAllocator.instance._ourPos[1].transform.position;
        player.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._ourPos[1].transform.position;
        _spawnedCompany[0] = player;
        GameObject ai;
        //ai = Instantiate(_AICharacters[1]);
        //ai.transform.position =
        //    MapAllocator.instance._ourPos[0].transform.position;
        //ai.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._ourPos[0].transform.position;
        //ai.transform.rotation = Quaternion.Euler(0, 0, 0);
        //ai.gameObject.tag = "Company";
        //_spawnedCompany[1] = ai;

        //ai = Instantiate(_AICharacters[2]);
        //ai.transform.position =
        //    MapAllocator.instance._ourPos[2].transform.position;
        //ai.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._ourPos[2].transform.position;
        //ai.transform.rotation = Quaternion.Euler(0, 0, 0);
        //ai.gameObject.tag = "Company";
        //_spawnedCompany[2] = ai;

        //
        //ai = Instantiate(_AICharacters[1]);
        //ai.transform.position = 
        //    MapAllocator.instance._enemyPos[1].transform.position;
        //ai.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._enemyPos[1].transform.position;
        //ai.transform.rotation = Quaternion.Euler(0, 180, 0);
        //ai.gameObject.tag = "Competition";
        //_spawnedEnemy[0] = ai;

        //ai = Instantiate(_AICharacters[0]);
        //ai.transform.position =
        //    MapAllocator.instance._enemyPos[0].transform.position;
        //ai.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._enemyPos[0].transform.position;
        //ai.transform.rotation = Quaternion.Euler(0, 180, 0);
        //ai.gameObject.tag = "Competition";
        //_spawnedEnemy[1] = ai;

        //ai = Instantiate(_AICharacters[2]);
        //ai.transform.position =
        //    MapAllocator.instance._enemyPos[2].transform.position;
        //ai.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._enemyPos[2].transform.position;
        //ai.transform.rotation = Quaternion.Euler(0, 180, 0);
        //ai.gameObject.tag = "Competition";
        //_spawnedEnemy[2] = ai;
    }

    public void IncreaseOurScore()
    {
        _ourScore++;
        InGameMainUI.instance._ScoreBar_Ourside._scoreText.text = _ourScore.ToString();

        if(_ourScore <= 10)
            InGameMainUI.instance._ScoreBar_Ourside._scoreFilled[_ourScore - 1].enabled = true;

        if (_ourScore >= 10 && _startCountRoutine == null && !_isEnemyWinning)
        {
            _isOurWinning = true;
            _startCountRoutine = StartCoroutine(StartCount(true));
        }
    }

    public void IncreaseEnemyScore()
    {
        _enemyScore++;
        InGameMainUI.instance._ScoreBar_Enemy._scoreText.text = _enemyScore.ToString();

        if(_enemyScore <= 10)
            InGameMainUI.instance._ScoreBar_Enemy._scoreFilled[_enemyScore - 1].enabled = true;

        if (_enemyScore >= 10 && _startCountRoutine == null && !_isOurWinning)
        {
            _isEnemyWinning = true;
            _startCountRoutine = StartCoroutine(StartCount(false));
        }
    }

    IEnumerator StartCount(bool isOur)
    {
        int count = 15;
        InGameMainUI.instance._Count.GetComponent<Text>().enabled = true;
        for(int i=0; i<count; i++)
        {
            InGameMainUI.instance._Count.GetComponent<Text>().text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        for(int i=0; i<3; i++)
        {
            //_spawnedCompany[i].SetActive(false);
            //_spawnedEnemy[i].SetActive(false);
        }
        if (isOur)
        {
            InGameMainUI.instance._Count.GetComponent<Text>().text = "승리";
        }
        else
        {
            InGameMainUI.instance._Count.GetComponent<Text>().text = "패배";
        }

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(1);
    }

    public void Respawn(GameObject deadPlayer)
    {
        if (deadPlayer.CompareTag("Player") || deadPlayer.CompareTag("Company"))
        {
            _ourScore -= deadPlayer.GetComponent<PlayerStats>()._score;
            InGameMainUI.instance._ScoreBar_Ourside._scoreText.text = _ourScore.ToString();
            for (int i = _ourScore; i < 10; i++)
            {
                InGameMainUI.instance._ScoreBar_Ourside._scoreFilled[i].enabled = false;
            }
            if (_ourScore < 10 && _startCountRoutine != null && _isOurWinning)
            {
                _isOurWinning = false;
                StopCoroutine(_startCountRoutine);
                _startCountRoutine = null;
                InGameMainUI.instance._Count.GetComponent<Text>().enabled = false;
            }
        }
        else if (deadPlayer.CompareTag("Competition"))
        {
            _enemyScore -= deadPlayer.GetComponent<PlayerStats>()._score;
            InGameMainUI.instance._ScoreBar_Enemy._scoreText.text = _enemyScore.ToString();

            for(int i= _enemyScore; i<10; i++)
            {
                InGameMainUI.instance._ScoreBar_Enemy._scoreFilled[i].enabled = false;
            }            
            if (_enemyScore < 10 && _startCountRoutine != null && _isEnemyWinning)
            {
                _isEnemyWinning = false;
                StopCoroutine(_startCountRoutine);
                _startCountRoutine = null;
                InGameMainUI.instance._Count.GetComponent<Text>().enabled = false;
            }
        }
        Vector3 respawnPos = deadPlayer.GetComponent<PlayerStats>()._spawnPosition;
        deadPlayer.SetActive(false);
        StartCoroutine(ReActiveGameObject(deadPlayer, respawnPos));
    }

    IEnumerator ReActiveGameObject(GameObject deadPlayer, Vector3 respawnPos)
    {
        yield return new WaitForSeconds(1);
        deadPlayer.transform.position = respawnPos;
        deadPlayer.transform.rotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(2);
        deadPlayer.SetActive(true);
    }
}
