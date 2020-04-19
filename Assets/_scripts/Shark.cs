using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
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
            if (other.gameObject.activeSelf) {
                FishJobs.instance.DeleteFish(other.gameObject);
            }
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
