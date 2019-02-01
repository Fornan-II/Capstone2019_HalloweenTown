﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    Candy script;
    float radius = 3.0f;
    Dictionary<Candy, Coroutine> dictionary = new Dictionary<Candy, Coroutine>();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, radius);//for testing
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Player")
            {
                script = hitColliders[i].gameObject.GetComponent<Candy>();
                script.showXForCandy = true;
                if (script != null)
                {
                    if (dictionary.ContainsKey(script) == false)
                    {
                        if (script.actionButton == true)
                        {
                            script.candy++;
                            dictionary.Add(script, StartCoroutine(WaitingforCandy(script)));
                        }
                        script.actionButton = false;
                        
                    }

                    /*Debug.Log(hitColliders[i].gameObject.name);
                    if (script.waitforcandy == false)
                    {
                        script.candy++;
                        script.WaitForCandy();
                    }*/
                }
                script.showXForCandy = false;
            }
           
            i++;
        }

    }
    IEnumerator WaitingforCandy(Candy script)
    {
        yield return new WaitForSeconds(3);
        dictionary.Remove(script);
        //Debug.Log(Time.time);
    }
}