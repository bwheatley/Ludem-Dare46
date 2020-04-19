using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public bool showWaterMovement = true;
    public Vector2 Map_BL = new Vector2(-21, -12);
    public Vector2 Map_TL = new Vector2(-21, 10);
    public Vector2 Map_BR = new Vector2(21, -12);
    public Vector2 Map_TR = new Vector2(21, 10);
    public float TimeLeft;
    public float DefaultTime = 60f;
    public float[] difficultyMultipler;
    public difficulty currentDifficulty;
    public List<GameObject> dontDestroyObjects;

    //Sound Shit
    public AudioClip[] shark_hit;
    public AudioClip[] squid_hit;
    public AudioClip[] fish_eaten;
    public AudioClip[] shark_eat;
    public AudioClip[] squid_eat;
    public AudioClip[] victory;
    public AudioClip[] dead;

    public AudioSource masterEffectSource;
    public AudioSource masterSharkEffectSource;
    public AudioSource masterSquidEffectSource;
    public AudioSource masterSoundsSource;
    public AudioSource masterMusicSource;
    //End Sound Shit

    public GameResult gameResult;

    public string GameOverTxt_Failure = "Game over, you lose!";
    public string GameOverTxt_Victory = "Game over, you are victorious";



    public enum GameResult {
        InProgress,
        Victory,
        Failure

    }

    public enum difficulty {
        Easy,
        Normal,
        Hard,
        Impossible
    }



    void Awake () {
        //We only ever want 1 game manager
        if (instance == null ) {
            instance = this;
        }
        else if ( instance != this ) {
            Debug.Log( string.Format( "More then one copy of Gamemanager..i must end myself!" ) );
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        DontDestroyObjects();



        int _difficulty = 0;
        //Determine enemy count from difficulty
        if (Difficulty.instance != null) {
            _difficulty = Difficulty.instance.GetDifficulty();
        }

        switch (_difficulty) {
        case 0:        //Easy
            currentDifficulty = difficulty.Easy;
            TimeLeft = difficultyMultipler[0] * DefaultTime;
            break;
        case 1:        //Normal
            currentDifficulty = difficulty.Normal;
            TimeLeft = difficultyMultipler[1] * DefaultTime;
            break;
        case 2:        //Hard
            currentDifficulty = difficulty.Hard;
            TimeLeft = difficultyMultipler[2] * DefaultTime;
            break;
        case 3:        //Impossible
            currentDifficulty = difficulty.Impossible;
            TimeLeft = difficultyMultipler[3] * DefaultTime;
            break;
        }

    }

    public float GetDifficultyMupliplier(int _difficulty) {
        return difficultyMultipler[_difficulty];
    }

    public void DontDestroyObjects() {
        for (int i = 0; i < dontDestroyObjects.Count; i++) {
            var _myItem = dontDestroyObjects[i];
            DontDestroyOnLoad(_myItem);
        }
    }
    public void SetDifficulty() {

    }

    public void PlayClip(string soundToPlay, string group = "Sounds") {
        AudioSource _myAudio = masterEffectSource.GetComponent<AudioSource>();
        if (group != "Sounds") {
            _myAudio = masterSoundsSource.GetComponent<AudioSource>();
        }

        switch (group) {
        case "shark_eat":
            _myAudio = masterSharkEffectSource.GetComponent<AudioSource>();
            break;
        case "squid_eat":
            _myAudio = masterSquidEffectSource.GetComponent<AudioSource>();
            break;

        }


        //LowerCasing it since it bite m in the ass once
        if (!_myAudio.isPlaying) {
            switch (soundToPlay.ToLower()) {
                case "shark_hit":
                    _myAudio.clip = shark_hit[Random.Range(0, shark_hit.Length)];
                    break;
                case "squid_hit":
                    _myAudio.clip = squid_hit[Random.Range(0, squid_hit.Length)];
                    break;
                case "fish_eaten":
                    _myAudio.clip = fish_eaten[Random.Range(0, fish_eaten.Length)];
                    break;
                case "shark_eat":
                    _myAudio.clip = shark_eat[Random.Range(0, shark_eat.Length)];
                    break;
                case "squid_eat":
                    _myAudio.clip = squid_eat[Random.Range(0, squid_eat.Length)];
                    break;
                case "victory":
                    _myAudio.clip = victory[Random.Range(0, victory.Length)];
                    break;
                case "dead":
                    _myAudio.clip = dead[Random.Range(0, dead.Length)];
                    break;
                // case "menu_dropdown":
                //     _myAudio.clip = menu_dropdown;
                //     break;
                // case "ui_submit":
                //     _myAudio.clip = ui_submit;
                //     break;
                // case "victory":
                //     //Util.WriteDebugLog("Play Victory");
                //     _myAudio.clip = VictoryAudioClip;
                //     break;
            }

            //Play the selected clip
            //Util.WriteDebugLog(string.Format("Clip Played {0} Audio Manager {1} Clip {2}", soundToPlay, _myAudio.name, _myAudio.clip.name), GameManager.LogLevel_Debug);
            _myAudio.Play();
        }
    }


    /// <summary>
    /// Add time to the clock
    /// </summary>
    public void AddTime() {

        int _difficulty = 0;
        //Determine enemy count from difficulty
        if (Difficulty.instance != null) {
            _difficulty = Difficulty.instance.GetDifficulty();
        }

        switch (_difficulty) {
            case 0: //Easy
                TimeLeft          = difficultyMultipler[0] + TimeLeft;
                break;
            case 1: //Normal
                TimeLeft          = difficultyMultipler[1] + TimeLeft;
                break;
            case 2: //Hard
                TimeLeft          = difficultyMultipler[2] + TimeLeft;
                break;
            case 3: //Impossible
                TimeLeft          = difficultyMultipler[2] + TimeLeft;
                break;
        }

    }


    // Update is called once per frame
    void Update() {
        // UiManager.instance.RemoveFish();
        if (TimeLeft >= 0) {
            TimeLeft -= Time.deltaTime;
            UiManager.instance.SetTheTimer(TimeLeft);
        }

        CheckGameOver();

    }


    //Is the game over...man?
    void CheckGameOver() {
        if (TimeLeft <= 0f || UiManager.instance.GetFishCount() == 0 || UiManager.instance.GetEnemyCount() == 0) {
            TimeLeft = 0f;



            //Did we win?
            if (gameResult == GameResult.InProgress) {
                if (UiManager.instance.GetEnemyCount() <= 0) {
                    gameResult = GameResult.Victory;
                }
                else {
                    gameResult = GameResult.Failure;
                }

                //Game over
                SceneManager.LoadScene(2);
            }

        }
    }

    public void GameOver_Victory() {
        string soundToPlay = "victory";
        GameManager.instance.PlayClip(soundToPlay);

    }

    public void GameOver_Failure() {
        string soundToPlay = "dead";
        GameManager.instance.PlayClip(soundToPlay);

    }


}
