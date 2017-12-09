﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canBeDest : MonoBehaviour 
{	
	public float ForcePropulse = 5;
	public float DelayDestruc = 1;
    public int nbPunchDestroy = 5;

	void OnCollisionEnter ( Collision collision )
	{
		if ( collision.collider.tag == Constants._PlayerTag )
		{
			StartCoroutine ( GlobalManager.GameCont.MeshDest.SplitMesh ( gameObject, collision.transform, ForcePropulse, DelayDestruc ) );
            int randomSong = UnityEngine.Random.Range(0, 5);
            GlobalManager.AudioMa.OpenAudio(AudioType.FxSound, "Wood_" + (randomSong + 1), false);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Constants._PunchTag && tag == Constants._Intro)
        {
            nbPunchDestroy--;
            if(nbPunchDestroy == 0)
            {
                GameObject.FindObjectOfType<GameController>().ActiveGame();
            }
        }
    }
}
