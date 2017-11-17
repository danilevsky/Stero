﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnChunks : MonoBehaviour 
{
	#region Variable
	public List<ChunksScriptable> ChunksInfo;
	public Vector3 DefaultPos;


	[HideInInspector]
	public int currLevel = 0;

	List<List<GetSpawnable>> AllSpawnable;
	List<GameObject> getSpawnChunks;
	List<GameObject> otherSpawn;

	GameObject WallOnLastChunk;
	Transform thisT;
	int currNbrCh = 0;
	int currChunk = 0;
	bool randAllChunk = false;
	#endregion
	
	#region Mono
	#endregion
		
	#region Public
	public void RemoveAll ( )
	{
		int a;

		for ( a = 0; a < otherSpawn.Count; a++ )
		{
			if ( otherSpawn [ a ] != null )
			{
				Destroy ( otherSpawn [ a ] );
			}
		}

		for ( a = 0; a < getSpawnChunks.Count; a++ )
		{
			if ( getSpawnChunks [ a ] != null )
			{
				Destroy ( getSpawnChunks [ a ] );
			}
		}
	}

	public void InitChunck ( )
	{
		getSpawnChunks = new List<GameObject> ( );
		AllSpawnable = new List<List<GetSpawnable>> ( );
		otherSpawn = new List<GameObject> ( );
		thisT = transform;

		List<ChunksScriptable> getChunks = ChunksInfo;
		List<List<GetSpawnable>> getSpawnable = AllSpawnable;

		Transform[] getChildrenChunk;

		int a;
		int b;
		int c;

		for ( a = 0; a < getChunks.Count; a++ )
		{
			getSpawnable.Add ( new List<GetSpawnable> ( ) );

			for ( b = 0; b < getChunks [ a ].TheseChunks.Count; b++ )
			{
				getSpawnable [ a ].Add ( new GetSpawnable ( ) );
				getSpawnable [ a ] [ b ].getCoinSpawnable = new List<GameObject> ( );
				getSpawnable [ a ] [ b ].getEnnemySpawnable = new List<GameObject> ( );
				getSpawnable [ a ] [ b ].getObstacleDestrucSpawnable = new List<GameObject> ( );
				getSpawnable [ a ] [ b ].getObstacleSpawnable = new List<GameObject> ( );

				getChildrenChunk = getChunks [ a ].TheseChunks [ b ].GetComponentsInChildren<Transform> ( true );

				for ( c = 0; c < getChildrenChunk.Length; c++ )
				{
					switch ( getChildrenChunk[c].tag )
					{
					case Constants._SAbleCoin:
						getSpawnable [ a ] [ b ].getCoinSpawnable.Add ( getChildrenChunk [ c ].gameObject );
						break;
					case Constants._SAbleDestObs:
						getSpawnable [ a ] [ b ].getObstacleDestrucSpawnable.Add ( getChildrenChunk [ c ].gameObject );
						break;
					case Constants._SAbleObs:
						getSpawnable [ a ] [ b ].getObstacleSpawnable.Add ( getChildrenChunk [ c ].gameObject );
						break;
					case Constants._SAbleEnnemy:
						getSpawnable [ a ] [ b ].getEnnemySpawnable.Add ( getChildrenChunk [ c ].gameObject );
						break;
					}
				}
			}
		}
	}

	public void NewSpawn ( NewChunkInfo sourceSpawn )
	{
		List<ChunksScriptable> getChunks = ChunksInfo;
		List<GameObject> getSpc = getSpawnChunks;

		spawnAfterThis ( sourceSpawn );

		if ( getSpc.Count > 3 )
		{
			Destroy ( getSpc [ 0 ] );
			getSpc.RemoveAt ( 0 );
		}

		if ( getChunks [ currLevel ].NbrChunkOneLvl < currNbrCh )
		{
			newLevel ( );
		}
	}

	public void FirstSpawn ( )
	{

		randAllChunk = false;
		currNbrCh = 0;
		currLevel = 0;
		List<GameObject> getSpc = getSpawnChunks;
		bool doubleFirst = false;

		while ( getSpc.Count > 0 )
		{
			doubleFirst = true;
			Destroy ( getSpc [ 0 ] );
			getSpc.RemoveAt ( 0 );
		}

		if ( doubleFirst )
		{
			StartCoroutine ( waitSpawn ( ) );
		}
		else
		{
			List<ChunksScriptable> getChunks = ChunksInfo;

			spawnAfterThis ( );

			if ( getChunks [ currLevel ].NbrChunkOneLvl < currNbrCh )
			{
				newLevel ( );
			}
		}
	}

	public void AddNewChunk ( GameObject thisSpawn )
	{
		getSpawnChunks.Add ( thisSpawn );
	}
	#endregion
	
	#region Private
	IEnumerator waitSpawn ( )
	{
		yield return new WaitForEndOfFrame ( );
		List<ChunksScriptable> getChunks = ChunksInfo;

		spawnAfterThis ( );

		if ( getChunks [ currLevel ].NbrChunkOneLvl < currNbrCh )
		{
			newLevel ( );
		}
	}

	void newLevel ( )
	{
		List<ChunksScriptable> getChunks = ChunksInfo;
		List<GameObject> getSpc = getSpawnChunks;
		Transform getChunkT = getSpc [ getSpc.Count - 1 ].transform;
		GameObject thisObj;

		if ( getChunks [ currLevel ].WallOnLastChunk != null )
		{
			thisObj = ( GameObject ) Instantiate ( getChunks [ currLevel ].WallOnLastChunk, thisT );
			thisObj.transform.position = getChunkT.position;
			thisObj.transform.localPosition += thisObj.transform.up * thisObj.GetComponent<MeshRenderer> ( ).bounds.size.y / 2;
		}

		currLevel++;

		if ( currLevel >= getChunks.Count || randAllChunk )
		{
			randAllChunk = true;

			currLevel = Random.Range ( 0, getChunks.Count );
		}

		currNbrCh = 0;
	}

	void spawnAfterThis ( NewChunkInfo sourceSpawn = null )
	{
		List<ChunksScriptable> getChunks = ChunksInfo;
		List<List<GetSpawnable>> getSpawnable = AllSpawnable;
		List<GameObject> getSpc = getSpawnChunks;
		GameObject thisSpawn;
		Transform getChunkT;

		if ( sourceSpawn != null )
		{
			List<NewChunkSaveInf> getNewChunk = new List<NewChunkSaveInf> ( );
			List<ToDestChunk> allNewChunk = new List<ToDestChunk> ( );
			NewChunkSaveInf getOtherNC;
			SpawnNewLvl currSL;
			int a;
			int b;
			int getInd;
			int diffLine;
			int randChunk = Random.Range ( 0, 2 );

			for ( a = 0; a < sourceSpawn.ThoseExit.Count; a++ )
			{
				if ( sourceSpawn.ThoseExit.Count > 1 )
				{
					currChunk = Random.Range ( 0, sourceSpawn.ThoseExit [ a ].SpawnEnable.Count );
					thisSpawn = sourceSpawn.ThoseExit [ a ].SpawnEnable [ currChunk ];
				}
				else
				{
					currChunk = currNbrCh;

					if ( currChunk >= getChunks [ currLevel ].TheseChunks.Count )
					{
						currChunk = Random.Range ( 0, getChunks [ currLevel ].TheseChunks.Count );
					}

					thisSpawn = getChunks [ currLevel ].TheseChunks [ currChunk ];
				}

				if ( thisSpawn != null )
				{
					thisSpawn = ( GameObject ) Instantiate ( thisSpawn, thisT );

					currSL = thisSpawn.GetComponentInChildren<SpawnNewLvl> ( );
					getChunkT = thisSpawn.transform;
					getChunkT.rotation = sourceSpawn.ThoseExit [ a ].LevelParent.rotation;
					getChunkT.position = sourceSpawn.ThoseExit [ a ].LevelParent.position;

					getNewChunk.Add ( new NewChunkSaveInf ( ) );
					getInd = getNewChunk.Count - 1;
					getNewChunk [ getInd ].ThisObj = getChunkT.gameObject;
					getNewChunk [ getInd ].NbrLaneDebut = currSL.InfoChunk.NbrLaneFin;
					getNewChunk [ getInd ].CurrLane = sourceSpawn.ThoseExit [ a ].LaneParent;
					//getNewChunk [ getInd ].ParentObj = sourceSpawn.ThoseExit [ a ].LevelParent;

					allNewChunk.Add ( new ToDestChunk ( ) );
					allNewChunk [ allNewChunk.Count - 1 ].ThisSL = currSL;
					allNewChunk [ allNewChunk.Count - 1 ].ThisObj = thisSpawn;

					spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getCoinSpawnable, getChunks [ currLevel ].CoinSpawnable );
					spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getEnnemySpawnable, getChunks [ currLevel ].EnnemySpawnable );
					spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getObstacleSpawnable, getChunks [ currLevel ].ObstacleSpawnable );
					spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getObstacleDestrucSpawnable, getChunks [ currLevel ].ObstacleDestrucSpawnable );

					otherSpawn.Add ( thisSpawn );
				}
			}

			// add the other chunk on current chunk in order to destroye them later
			for ( a = 0; a < allNewChunk.Count; a++ )
			{
				for ( b = 0; b < allNewChunk.Count; b++ )
				{
					if ( a != b )
					{
						allNewChunk [ a ].ThisSL.ToDest.Add ( allNewChunk [ b ].ThisObj );
					}
				}
			}

			// re calculate the order by lane parent
			a = getNewChunk.Count;
			while ( a > 1 )
			{
				a--;

				if ( getNewChunk [ a ].CurrLane < getNewChunk [ a - 1 ].CurrLane )
				{
					getOtherNC = getNewChunk [ a - 1 ];
					getNewChunk [ a - 1 ] = getNewChunk [ a ];
					getNewChunk [ a ] = getOtherNC;
					a = getNewChunk.Count;
				}
			}

			// check the space between each chunks
			if ( randChunk == 0 )
			{
				for ( a = 0; a < getNewChunk.Count - 1; a++ )
				{
					diffLine = ( int ) ( getNewChunk [ a ].NbrLaneDebut.y + getNewChunk [ a + 1 ].NbrLaneDebut.x - Mathf.Abs ( getNewChunk [ a + 1 ].CurrLane - getNewChunk [ a ].CurrLane ) );

					if ( diffLine >= 0 )
					{
						diffLine++;
						getNewChunk [ a + 1 ].ThisObj.transform.localPosition += new Vector3 ( Constants.LineDist * diffLine, 0, 0 );
						getNewChunk [ a + 1 ].CurrLane += diffLine;

						b = a;
						while ( b < getNewChunk.Count - 1 )
						{
							if ( getNewChunk [ b ].CurrLane > getNewChunk [ b + 1 ].CurrLane )
							{
								getOtherNC = getNewChunk [ b + 1 ];
								getNewChunk [ b + 1 ] = getNewChunk [ b ];
								getNewChunk [ b ] = getOtherNC;
								b = a;
							}

							b++;
						}

						a--;
					}
				}
			}
			else
			{
				for ( a = getNewChunk.Count - 1; a > 0; a-- )
				{
					diffLine = ( int ) ( getNewChunk [ a ].NbrLaneDebut.y + getNewChunk [ a - 1 ].NbrLaneDebut.x - Mathf.Abs ( getNewChunk [ a - 1 ].CurrLane - getNewChunk [ a ].CurrLane ) );

					if ( diffLine >= 0 )
					{
						diffLine++;
						getNewChunk [ a - 1 ].ThisObj.transform.localPosition -= new Vector3 ( Constants.LineDist * diffLine, 0, 0 );
						getNewChunk [ a - 1 ].CurrLane -= diffLine;

						b = a;
						while ( b > 0 )
						{
							if ( getNewChunk [ b ].CurrLane < getNewChunk [ b - 1 ].CurrLane )
							{
								getOtherNC = getNewChunk [ b - 1 ];
								getNewChunk [ b - 1 ] = getNewChunk [ b ];
								getNewChunk [ b ] = getOtherNC;
								b = a;
							}

							b--;
						}

						a++;
					}
				}
			}

			// check if there is spaces and place wall if yes
			for ( a = 0; a < getNewChunk.Count - 1; a++ )
			{
				diffLine = ( int ) ( getNewChunk [ a ].NbrLaneDebut.y + getNewChunk [ a + 1 ].NbrLaneDebut.x - Mathf.Abs ( getNewChunk [ a + 1 ].CurrLane - getNewChunk [ a ].CurrLane ) );

				/*while ( diffLine < 0 )
				{
					thisSpawn = ( GameObject ) Instantiate ( getChunks [ currLevel ].WallEndChunk, getNewChunk [ a ].ParentObj );
					thisSpawn.transform.localPosition = new Vector3 ( Constants.LineDist * diffLine, 0, 0 );
					thisSpawn.transform.localEulerAngles = new Vector3 ( 0, 90, 0 );
					diffLine++;
				}*/
			}
		}
		else
		{
			if ( getChunks [ currLevel ].ChunkAleat )
			{
				currChunk = Random.Range ( 0, getChunks [ currLevel ].TheseChunks.Count );
				thisSpawn = getChunks [ currLevel ].TheseChunks [ currChunk ];
			}
			else
			{
				currChunk = currNbrCh;
				thisSpawn = getChunks [ currLevel ].TheseChunks [ currNbrCh ];
			}

			if ( thisSpawn != null )
			{
				thisSpawn = ( GameObject ) Instantiate ( thisSpawn, thisT );

				getChunkT = thisSpawn.transform;
				getChunkT.rotation = Quaternion.identity;
				getChunkT.position = DefaultPos;

				spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getCoinSpawnable, getChunks [ currLevel ].CoinSpawnable );
				spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getEnnemySpawnable, getChunks [ currLevel ].EnnemySpawnable );
				spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getObstacleSpawnable, getChunks [ currLevel ].ObstacleSpawnable );
				spawnElements ( getSpawnable [ currLevel ] [ currChunk ].getObstacleDestrucSpawnable, getChunks [ currLevel ].ObstacleDestrucSpawnable );

				getSpc.Add ( thisSpawn );
			}
		}

		currNbrCh++;

		/*
		GameObject getObj;
		NewChunkInfo getNew = thisSpawn.GetComponentInChildren<SpawnNewLvl> ( ).InfoChunk;
		int getNbr;
		int calNbr;

		if ( Random.Range ( 0, 2 ) == 0 )
		{
			getNbr = -Random.Range ( 0, ( int ) thisLvl.NbrLaneFin.x + 1 );
			thisPos = new Vector3 ( thisPos.x + Constants.LineDist * getNbr, thisPos.y, thisPos.z );
		}
		else
		{
			getNbr = Random.Range ( 0, ( int ) thisLvl.NbrLaneFin.y + 1 );
			thisPos = new Vector3 ( thisPos.x + Constants.LineDist * getNbr, thisPos.y, thisPos.z );
		}

		calNbr = ( int ) thisLvl.NbrLaneFin.x - getNbr;

		if ( getNew.NbrLaneDebut.x < calNbr )
		{
			for ( a = calNbr; a > getNew.NbrLaneDebut.x; a-- )
			{
				getObj = ( GameObject ) Instantiate ( getChunks [ currLevel ].WallEndChunk, thisLvl.transform );
				getObj.transform.position = new Vector3 ( thisPos.x + a * 6, thisPos.y, thisPos.z );
			}
		}

		calNbr = ( int ) thisLvl.NbrLaneFin.x + getNbr;
		if ( getNew.NbrLaneDebut.y > calNbr )
		{
			for ( a = calNbr; a < getNew.NbrLaneDebut.y; a++ )
			{
				Instantiate ( getChunks [ currLevel ].WallEndChunk, thisLvl.transform );
			}
		}*/
	}

	void spawnElements ( List<GameObject> spawnerElem, List<GameObject> elemSpawnable )
	{
		GameObject thisObj;
		int rand = ChunksInfo [ currLevel ].PourcSpawn;
		int a;

		for ( a = 0; a < spawnerElem.Count; a++ )
		{
			if ( Random.Range ( 0, 100 ) <= rand )
			{
				thisObj = ( GameObject ) Instantiate ( elemSpawnable [ Random.Range ( 0, elemSpawnable.Count ) ], spawnerElem [ a ].transform );
				thisObj.transform.localScale = new Vector3 ( 1, 1, 1 );
				thisObj.transform.localPosition = Vector3.zero;
			}
		}
	}
	#endregion
}

public class GetSpawnable
{
	public List<GameObject> getEnnemySpawnable;
	public List<GameObject> getObstacleSpawnable;
	public List<GameObject> getObstacleDestrucSpawnable;
	public List<GameObject> getCoinSpawnable;
}

public class NewChunkSaveInf 
{
	public GameObject ThisObj;
	public Transform ParentObj;
	public Vector2 NbrLaneDebut;
	public int CurrLane;
}

public class ToDestChunk 
{
	public GameObject ThisObj;
	public SpawnNewLvl ThisSL;
}