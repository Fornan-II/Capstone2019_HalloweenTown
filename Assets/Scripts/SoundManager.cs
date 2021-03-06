﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip boo;
    public AudioClip oof;
    public AudioClip witchScream;
    public AudioClip ghostScream;
    public AudioClip werewolfScream;
    public AudioClip vampireScream;
    public AudioClip zipup;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void StopPlaying()
    {
       if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    public void CostumeOn()
    {
        audioSource.clip = zipup;
        audioSource.Play();
    }

    public void WerewolfAttack()
    {

    }

    public void VampireSlurp()
    {

    }

    public void Oof()
    {
        audioSource.clip = oof;
        audioSource.Play();
    }

    public void Boo()
    {
        StopPlaying();
        audioSource.clip = boo;
        audioSource.Play();
    }

    public void WitchUltScream()
    {
        audioSource.clip = witchScream;
        audioSource.Play();
    }

    public void GhostUltScream()
    {
        audioSource.clip = ghostScream;
        audioSource.Play();
    }

    public void WerewolfUltScream()
    {
        audioSource.clip = werewolfScream;
        audioSource.Play();
    }

    public void VampireUltScream()
    {
        audioSource.clip = vampireScream;
        audioSource.Play();
    }
}
