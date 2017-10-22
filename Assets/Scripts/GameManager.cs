using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public enum GameState { IDLE, EATING, FIGHTING, GAME_OVER }
    public GameState State
    {
        get;
        private set;
    }

    [SerializeField] private TextMesh _gameOverText;
    private SharkManager _sharkManager;
    private RayMouth _rayMouth;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _musicTransitions;
    [SerializeField] private AudioClip _eatingMusic;
    [SerializeField] private AudioClip _fightingMusic;
    [SerializeField] private AudioClip _transitionToFightingMusic;
    [SerializeField] private AudioClip _transitionToEatingMusic;

    private float _difficulty = 0f;
    private float _completeDifficultyCompletionTime = 0f;
    private float _easyEatingTime = 30f;
    private float _hardEatingTime = 5f;
    private float _easyFightingTime = 20f;
    private float _hardFightingTime = 10f;
    private float _timingRandomnessFactor = 0.2f;

    private float _nextFightingTime = float.MaxValue;
    public float NextFightingTime { get { return _nextFightingTime; } }
    private float _fightingTime = float.MinValue;
    protected GameManager() { } // guarantee this will be always a singleton only - can't use the constructor!

    private void Start()
    {
        _sharkManager = FindObjectOfType<SharkManager>();
        _rayMouth = FindObjectOfType<RayMouth>();
        GetGlobalVars();

        Reset();
    }

    private void Reset()
    {
        State = GameState.IDLE;
        _difficulty = 0f;
        _nextFightingTime = float.MaxValue;
        _fightingTime = float.MinValue;
        _gameOverText.gameObject.SetActive(false);
    }

    private void GetGlobalVars()
    {
        _completeDifficultyCompletionTime = GlobalVars.Instance.completeDifficultyCompletionTime;
        _easyEatingTime = GlobalVars.Instance.easyEatingTime;
        _hardEatingTime = GlobalVars.Instance.hardEatingTime;
        _easyFightingTime = GlobalVars.Instance.easyFightingTime;
        _hardFightingTime = GlobalVars.Instance.hardFightingTime;
        _timingRandomnessFactor = GlobalVars.Instance.timingRandomnessFactor;
    }

    public void GameOver(int score)
    {
        // spawn the game over panel
        int thisPlayerIndex = 0;
        while (PlayerPrefs.HasKey("" + thisPlayerIndex++)) { }
        PlayerPrefs.SetInt("" + thisPlayerIndex, GetScore());

        _gameOverText.text = "GAME OVER\nscore: " + GetScore();
        _gameOverText.gameObject.SetActive(true);

        State = GameState.GAME_OVER;
    }

    public int GetScore()
    {
        return _rayMouth.Score;
    }

    private void Update()
    {
        GetGlobalVars();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(State == GameState.IDLE)
            {
                FindObjectOfType<PlayerController>().StartGame();
                State = GameState.EATING;
                _nextFightingTime = Mathf.Lerp(_easyEatingTime, _hardEatingTime, _difficulty) * Random.Range(1f - _timingRandomnessFactor, 1f + _timingRandomnessFactor);
            }
            else if(State == GameState.GAME_OVER)
            {
                SceneManager.LoadScene("Main Scene");
            }
        }

        if(State == GameState.EATING) // everything related to playing
        {
            // shark phase drawing according to difficulty
            if(_nextFightingTime <= 0f)
            {
                // shark attack
                _musicTransitions.PlayOneShot(_transitionToFightingMusic);

                _sharkManager.Start(_difficulty);
                State = GameState.FIGHTING;
                _fightingTime = Mathf.Lerp(_easyFightingTime, _hardFightingTime, _difficulty) * Random.Range(1f - _timingRandomnessFactor,1f + _timingRandomnessFactor);
            }
            else
            {
                _nextFightingTime -= Time.deltaTime;
            }
        }
        else if(State == GameState.FIGHTING)
        {
            if(_fightingTime <= 0f)
            {
                // end shark attack
                _musicTransitions.PlayOneShot(_transitionToEatingMusic);

                _sharkManager.Stop();
                State = GameState.EATING;
                _nextFightingTime = Mathf.Lerp(_easyEatingTime, _hardEatingTime, _difficulty) * Random.Range(1f - _timingRandomnessFactor, 1f + _timingRandomnessFactor);
            }
            else
            {
                _fightingTime -= Time.deltaTime;
            }
        }

        _difficulty += Time.deltaTime / (60 * 10f);

    }

    // vars for panel spawn
    public GameObject player;
    public GameObject panelPrefab;
    public float corridorWidth = 16f;
    public float zOffset = 1.5f;
    public float startHeight = 30f;
    public float panelsSpeed = 10f;

    public void NewScore(int s)
    {
        Debug.LogWarning("NEW SCORE");
        // Display score panels
        if (s % 20 == 0 && s > 0)
        {
            SpawnScorePanel(s);
        }
    }

    public void SpawnScorePanel(int s)
    {
        // Compute distance
        float distToTravelPanel = startHeight;
        float timeToTravel = distToTravelPanel / panelsSpeed; // seconds
        float distToTravelPlayer = timeToTravel * player.GetComponent<PlayerController>().speedForce;
        // Random Z
        float newZ = player.transform.position.z + zOffset;
        // Compute shark initial position
        Vector3 sharkPos = new Vector3(0f, startHeight, distToTravelPlayer + newZ);
        // Spawn shark
        GameObject panelObj = GameObject.Instantiate(panelPrefab, sharkPos, Quaternion.identity);
        ScorePanel panel = panelObj.GetComponent<ScorePanel>();
        panel.speed = panelsSpeed;
        panel.SetScore(s);
    }
}