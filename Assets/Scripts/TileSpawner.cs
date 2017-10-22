using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

	public List<TileHandler> tilesPrefab = new List<TileHandler> ();
	//public List<TileHandler> tiles = new List<TileHandler> ();
	public List<Transform> tilesObj = new List<Transform> ();
	public float tileSize;
	Transform playerPos;
	TileHandler prevTile;
	public float frontView;
	public float backView;

	void Start () {
			
		playerPos = GameObject.FindGameObjectWithTag ("Player").transform;

		//tileSize = tilesPrefab [0].transform.localScale.z;
		SpawnTile ();
	}

	void Update () {
		if (prevTile != null && prevTile.gameObject.transform.position.z <= playerPos.position.z + frontView) {
			SpawnTile ();
		}

		for (int i = 0; i < tilesObj.Count; i++) {
			if (tilesObj [i].position.z <= playerPos.position.z - backView) {
				GameObject.Destroy (tilesObj [i].gameObject);
				tilesObj.Remove (tilesObj [i]);
			}
		}
	}

	float rand (int min, int max) {
		return Mathf.Floor (Random.value * (max - min + 1)) + min;
	}

	[ContextMenu ("SpawnTile")]
	void SpawnTile () {
		int rand = Random.Range (0, tilesPrefab.Count);

        TileHandler tileH;

        if (prevTile != null)
        {
            float unspawnedTilesNbBeforeShark = GameManager.Instance.NextFightingTime / GlobalVars.Instance.speed;
            unspawnedTilesNbBeforeShark -= (prevTile.transform.position.z + tileSize - playerPos.position.z) / GlobalVars.Instance.speed;

            if (GameManager.Instance.State == GameManager.GameState.FIGHTING || (GameManager.Instance.NextFightingTime > 0 && unspawnedTilesNbBeforeShark < 1f))
                tileH = MathUtilities.Draw(tilesPrefab.Where(t => t.IsOkayWithSharks).ToList());
            else
                tileH = MathUtilities.Draw(tilesPrefab);
        }
        else
            tileH = MathUtilities.Draw(tilesPrefab);

		TileHandler tile = Instantiate (tileH);

		tile.gameObject.SetActive (true);

		if (prevTile == null) {
			tile.transform.position = new Vector3 (0, 0, 0);
		} else {
			tile.transform.position = new Vector3 (0, 0, prevTile.transform.position.z + tileSize);
		}

		tile.Init (prevTile);

		tilesObj.Add (tile.transform);
		prevTile = tile;
	}

}
