﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndTuto : MonoBehaviour 
{
	public GameObject DestroyThis;
	void OnTriggerEnter ( Collider collision )
	{
		string getTag = collision.tag;
		if ( getTag == Constants._PlayerTag )
		{
			//GlobalManager.AudioMa.CloseAllAudio();
			GlobalManager.GameCont.thisCam.fieldOfView = Constants.DefFov;
			GlobalManager.GameCont.Player.GetComponent<PlayerController> ( ).ResetPosDo ( );
			GlobalManager.GameCont.soundFootSteps.Kill();
			Destroy ( DestroyThis, 0.58f );
		}
	}
}
