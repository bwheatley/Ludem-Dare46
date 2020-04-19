using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour {
    public EnemyType type;

    public enum EnemyType {
        Shark,
        Squid
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(string.Format("Shark Awake"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log(string.Format("We've eaten something {0}", other.name));

        //Sharks can't eat sharks
        if (!other.gameObject.GetComponent<Shark>()) {
            string soundToPlay = "";
            //What type of enemy
            if (type == Shark.EnemyType.Shark) {
                soundToPlay = "shark_eat";
            }
            else if (type == Shark.EnemyType.Squid) {
                soundToPlay = "squid_eat";
            }
            GameManager.instance.PlayClip(soundToPlay, soundToPlay);

            FishJobs.instance.DeleteFish(other.gameObject);
        }
    }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     //Sharks can't eat sharks
    //     if (!other.gameObject.GetComponent<Shark>()) {
    //         if (other.gameObject.activeSelf) {
    //             FishJobs.instance.DeleteFish(other.gameObject);
    //         }
    //     }
    // }


}
