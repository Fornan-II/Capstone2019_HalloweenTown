﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBagSpawner : MonoBehaviour {

    public List<GameObject> bags;
    Pawn pawn;
    GameObject myBag;
    bool setbag = true;
	void Start () {
        pawn = GetComponent<Pawn>();
        Debug.Log("player number " + pawn.MyController.PlayerNumber);
        
    }
    
    void Update () {

        if(pawn.MyController != null && setbag)
        {
            myBag = Instantiate(bags[(int)pawn.MyController.PlayerNumber-1]);
            myBag.transform.SetParent(pawn.LhandBagSpawn.transform, false);
            setbag = false;
        }
    }
}
