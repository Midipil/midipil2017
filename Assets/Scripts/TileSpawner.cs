using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

	public List<TileHandler> tilesPrefab = new List<TileHandler>();
	//public List<TileHandler> tiles = new List<TileHandler> ();
	public List<Transform> tilesObj = new List<Transform> ();
	float tileSize;
	Transform playerPos;
	Transform prevTile;
	public float frontView;
	public float backView;

	void Start () {
			
		playerPos = GameObject.FindGameObjectWithTag ("Player").transform;

	    tileSize = tilesPrefab [0].transform.localScale.z * 2;
		SpawnTile ();
	}

	void Update()
	{
		if (prevTile != null && prevTile.position.z <= playerPos.position.z + frontView) {
			SpawnTile ();
		}

		for (int i = 0; i < tilesObj.Count; i++) {
			if (tilesObj [i].position.z <= playerPos.position.z - backView) 
			{
				GameObject.Destroy (tilesObj [i].gameObject);
				tilesObj.Remove (tilesObj [i]);
			}
		}
	}

	float rand (int min, int max)
	{
		return Mathf.Floor(Random.value * (max - min + 1)) + min;
	}

	[ContextMenu("SpawnTile")]
	void SpawnTile()
	{
		int rand = Random.Range (0, tilesPrefab.Count);

		TileHandler tileH = MathUtilities.Draw (tilesPrefab);	

		TileHandler tile = Instantiate (tilesPrefab[rand]);

		tile.gameObject.SetActive (true);

		if (prevTile == null) {
			tile.transform.position = new Vector3(0,0,0);
			prevTile = tile.transform;
		}
		else {
			tile.transform.position = new Vector3(0,0,prevTile.position.z + tileSize * 5);
			prevTile = tile.transform;
		}

		tilesObj.Add (tile.transform);
	}

}
