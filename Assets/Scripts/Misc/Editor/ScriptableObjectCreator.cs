﻿using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();

		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");

		AssetDatabase.CreateAsset (asset, assetPathAndName);

		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("CustomTools/Scriptable/Chunks")]
	public static void CreateChunk ( )
	{
		ScriptableObjectUtility.CreateAsset<ChunksScriptable> ();
	}

	[MenuItem("CustomTools/Scriptable/AudioScriptable")]
	public static void CreateAudio ( )
	{
		ScriptableObjectUtility.CreateAsset<AudioScriptable> ();
	}

	[MenuItem("CustomTools/Scriptable/ListChunk")]
	public static void CreateListChunk ( )
	{
		ScriptableObjectUtility.CreateAsset<ListChunkScriptable> ( );
	}

    [MenuItem("CustomTools/Scriptable/CoinBonus")]
    public static void CreateListCoinBonus()
    {
        ScriptableObjectUtility.CreateAsset<CoinBonusScriptable>();
    }
}