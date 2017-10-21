using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

	public List<TileHandler> tilesPrefab = new List<TileHandler>();
	public List<GameObject> tiles = new List<GameObject>();
	float tileSize;
	Transform playerPos;
	Transform prevTile;
	float fogDistance;

	void Start () {
		for (int i = 0; i < tilesPrefab.Count; i++) {
			tiles.Add (Instantiate(tilesPrefab [i].gameObject));
		}
		for (int i = 0; i < tiles.Count; i++) {
			tiles [i].SetActive (false);
		}
			
		tileSize = tilesPrefab [0].transform.localScale.z * 2;
		SpawnTile ();
	}

	float rand (int min, int max)
	{
		return Mathf.Floor(Random.value * (max - min + 1)) + min;
	}

	[ContextMenu("SpawnTile")]
	void SpawnTile()
	{
		int rand = Random.Range (0, tilesPrefab.Count);
		
		GameObject tile = tiles [rand].gameObject;
		tile.SetActive (true);

		if (prevTile == null) {
			tile.transform.position = new Vector3(0,0,0);
			prevTile = tile.transform;
		}
		else {
			tile.transform.position = new Vector3(0,0,prevTile.position.z + tileSize * 5);
			prevTile = tile.transform;
		}
	}

}
