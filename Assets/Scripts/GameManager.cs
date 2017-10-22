using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public enum GameState { IDLE, EATING, FIGHTING, GAME_OVER }
    public GameState State
    {
        get;
        private set;
    }

    private SharkManager _sharkManager;
    private RayMouth _rayMouth;
    private SteamVR_TrackedObject _rightDevice;

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

    public float NextFightingTime
    {
        get { return _nextFightingTime; }
    }
    private float _nextFightingTime = float.MaxValue;
    private float _fightingTime = float.MinValue;
    protected GameManager() { } // guarantee this will be always a singleton only - can't use the constructor!

    private void Start()
    {
        _sharkManager = FindObjectOfType<SharkManager>();
        _rayMouth = FindObjectOfType<RayMouth>();
        GetGlobalVars();

        _musicSource.loop = true;
        _musicSource.clip = _eatingMusic;
        _musicSource.Play();

        Reset();
    }

    private void Reset()
    {
        _sharkManager = FindObjectOfType<SharkManager>();
        _rayMouth = FindObjectOfType<RayMouth>();

        State = GameState.IDLE;
        _difficulty = 0f;
        _nextFightingTime = float.MaxValue;
        _fightingTime = float.MinValue;
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
        if (State != GameState.GAME_OVER)
        {
            // spawn the game over panel
            int thisPlayerIndex = 0;
            while (PlayerPrefs.HasKey("" + thisPlayerIndex++)) { }
            PlayerPrefs.SetInt("" + thisPlayerIndex, GetScore());
            /*
            _gameOverText.text = "GAME OVER\nscore: " + GetScore();
            _gameOverText.gameObject.SetActive(true);
            */
            State = GameState.GAME_OVER;

            // Spawn panneau
            SpawnScorePanel(GetScore(), true);
        }
    }

    public int GetScore()
    {
        return _rayMouth.Score;
    }

    private void Update()
    {
        GetGlobalVars();

        if(_rightDevice == null)
        {
            _rightDevice = FindObjectsOfType<SteamVR_TrackedObject>().FirstOrDefault(t => t.gameObject.name.ToLower() == "right");
        }

        if (Input.GetKeyDown(KeyCode.Space) || (_rightDevice != null && SteamVR_Controller.Input((int)_rightDevice.index).GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)))
        {
            if(State == GameState.IDLE)
            {
                FindObjectOfType<PlayerController>().StartGame();
                State = GameState.EATING;
                _nextFightingTime = Mathf.Lerp(_easyEatingTime, _hardEatingTime, _difficulty) * Random.Range(1f - _timingRandomnessFactor, 1f + _timingRandomnessFactor);
            }
            else if(State == GameState.GAME_OVER)
            {
                SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
                Reset();
            }
        }

        if(State == GameState.EATING) // everything related to playing
        {
            // shark phase drawing according to difficulty
            if(_nextFightingTime <= 0f)
            {
                // shark attack
                _musicTransitions.PlayOneShot(_transitionToFightingMusic);
                _musicSource.clip = _fightingMusic;
                _musicSource.Play();

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
                _musicSource.clip = _eatingMusic;
                _musicSource.Play();

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

        if (Input.GetKeyUp("g"))
        {
            GameOver(GetScore());
        }

    }

    // vars for panel spawn
    public GameObject panelPrefab, endPanelPrefab;
    public float corridorWidth = 16f;
    public float zOffset = 5f;
    public float startHeight = 30f;
    public float panelsSpeed = 10f;

    public void NewScore(int s)
    {
        // Display score panels
        if (s % 25 == 0 && s > 0 || s==66)
        {
            SpawnScorePanel(s, false);
        }
    }

    public void SpawnScorePanel(int s, bool end = false)
    {
        var player = FindObjectOfType<PlayerController>();

        // Compute distance
        float distToTravelPanel = startHeight;
        float timeToTravel = distToTravelPanel / panelsSpeed; // seconds
        float distToTravelPlayer = timeToTravel * player.speedForce;
        // Random Z
        float newZ = player.transform.position.z + zOffset;
        // Compute initial position
        Vector3 sharkPos = new Vector3(0f, startHeight, distToTravelPlayer + newZ);
        // Spawn 
        GameObject prefab = panelPrefab;
        if(end)
        {
            prefab = endPanelPrefab;
        }
        GameObject panelObj = GameObject.Instantiate(prefab, sharkPos, Quaternion.identity);

        ScorePanel panel = panelObj.GetComponent<ScorePanel>();
        panel.speed = panelsSpeed;
        panel.SetScore(s);

        if (!end)
        {
            float a = 15;
            panelObj.transform.rotation = Quaternion.Euler(Random.Range(-a, a), Random.Range(-a, a), Random.Range(-a, a));
        }
        
    }
}