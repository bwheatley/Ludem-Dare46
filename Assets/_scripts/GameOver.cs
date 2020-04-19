using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public bool Soundfired = false;
    public static GameOver instance;
    public GameObject BatmanGO;

    public TMP_Text GameOverMessage;


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
    void Start()
    {
        if (GameManager.instance.gameResult == GameManager.GameResult.Victory) {
            GameManager.instance.GameOver_Victory();

            GameOverMessage.text = GameManager.instance.GameOverTxt_Victory;
        }
        else {
            GameManager.instance.GameOver_Failure();

            GameOverMessage.text = GameManager.instance.GameOverTxt_Failure;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame() {
        //
        GameManager.instance.gameObject.transform.SetParent(BatmanGO.transform);
        var _diffGO = GameObject.FindGameObjectsWithTag("Difficulty");
        for (int i = 0; i < _diffGO.Length; i++) {
            _diffGO[i].transform.SetParent(BatmanGO.transform);
        }
        var _audioGO = GameObject.FindGameObjectsWithTag("AudioManager");
        for (int i = 0; i < _audioGO.Length; i++) {
            _audioGO[i].transform.SetParent(BatmanGO.transform);
        }


        SceneManager.LoadScene(0);
    }

    public void ExitGame() {
        Application.Quit();
    }


}
