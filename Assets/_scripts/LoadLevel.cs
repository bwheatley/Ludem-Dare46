using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadALevel(int difficulty) {

        //0 - Easy 1 - Normal 2 - Hard 3 - Impossible
        Difficulty.instance.SetDifficulty(difficulty);SceneManager.LoadScene(1);

    }
    
}
