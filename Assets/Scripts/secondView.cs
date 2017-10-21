using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondView : MonoBehaviour {

	public GameObject prefab;
	public float spawnHight;
	public Camera cam;
	Vector3 pos = new Vector3();

	public GameObject player;
	private Vector3 offset;  

	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		offset = transform.position - player.transform.position;
	}

	void Update () 
	{

		if(Input.GetMouseButtonDown(0))   
		{

			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			//Debug.Log (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {

				GameObject obj = Instantiate (prefab);
				obj.transform.position = new Vector3 (hit.point.x, hit.point.y + spawnHight, hit.point.z);
			}
			/*
			Vector2 mousePos = new Vector2();

			mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			pos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
			Debug.Log (pos);
			Instantiate (prefab, pos, Quaternion.identity);*/
		}
	}

	void LateUpdate () 
	{
		transform.position = player.transform.position + offset;
	}
}
