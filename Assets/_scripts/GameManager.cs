using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        //Set the difficulty
        var _difficulty =  Difficulty.instance.GetDifficulty();

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
        for (int i = 0; i < GameManager.instance.dontDestroyObjects.Count; i++) {
            var _myItem = GameManager.instance.dontDestroyObjects[i];
            DontDestroyOnLoad(_myItem);
        }
    }
    public void SetDifficulty() {

    }

    /// <summary>
    /// Add time to the clock
    /// </summary>
    public void AddTime() {
        //Set the difficulty
        var _difficulty =  Difficulty.instance.GetDifficulty();

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
                TimeLeft          = difficultyMultipler[3] + TimeLeft;
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

            //Game over
            SceneManager.LoadScene(2);
        }
    }

}
