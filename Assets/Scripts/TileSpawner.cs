using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

	public List<TileHandler> tilesPrefab = new List<TileHandler> ();
	//public List<TileHandler> tiles = new List<TileHandler> ();
	List<Transform> tilesObj = new List<Transform> ();
	List<Transform> sideObj = new List<Transform> ();
	public float tileSize;
	Transform playerPos;
	TileHandler prevTile;
	public float frontView;
	public float backView;
	public GameObject sideTile;
	Transform prevSide;
	float sideSize;


	void Start () {

		playerPos = GameObject.FindGameObjectWithTag ("Player").transform;
		sideSize = 250f;
		//tileSize = tilesPrefab [0].transform.localScale.z;
		SpawnTile ();
		SpawnSide ();
	}

	void Update () {
		if (prevTile != null && prevTile.gameObject.transform.position.z <= playerPos.position.z + frontView) {
			SpawnTile ();
		}
		if (prevSide != null && prevSide.gameObject.transform.position.z <= playerPos.position.z + frontView * 2) {
			SpawnSide ();
		}

		for (int i = 0; i < tilesObj.Count; i++) {
			if (tilesObj [i].position.z <= playerPos.position.z - backView) {
				GameObject.Destroy (tilesObj [i].gameObject);
				tilesObj.Remove (tilesObj [i]);
			}
		}

		for (int i = 0; i < sideObj.Count; i++) {
			if (sideObj[i] != null && sideObj [i].position.z <= playerPos.position.z - backView*2) {
				GameObject.Destroy (sideObj [i].gameObject);
				tilesObj.Remove (sideObj [i]);
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
            unspawnedTilesNbBeforeShark -= (prevTile.transform.position.z - playerPos.position.z) / GlobalVars.Instance.speed;

            if (GameManager.Instance.NextFightingTime > 0 && unspawnedTilesNbBeforeShark < 1f)
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

	void SpawnSide()
	{
		GameObject side = Instantiate (sideTile);

		if (prevSide == null) {
			side.transform.position = new Vector3 (0, 0, 0);
		} else if (prevSide.gameObject.transform.position.z <= playerPos.position.z + frontView * 2) {

			side.transform.position = new Vector3 (0, 0, prevSide.transform.position.z + sideSize);
		}

		prevSide = side.transform;
		sideObj.Add (side.transform);
	}


}
