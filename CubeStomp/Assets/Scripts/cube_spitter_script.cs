using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube_spitter_script : MonoBehaviour {
	GameObject[] cubes;
	public GameObject cube;
	public int maxNumCubes = 50;
	int nextActiveCube=0;
	public Vector2 rightForce, leftForce;
	public float cubeLifetime = 1f;
	// Use this for initialization
	void Start () {
		cubes = new GameObject[maxNumCubes];
		initCubes();
	}
	void initCubes(){
		for (int i =0; i<cubes.Length; i++){
			cubes[i] = Instantiate(cube) as GameObject;
			cubes[i].SetActive(false);
		}
	}
	public IEnumerator spawnCubes(){
			throwCube(rightForce);
			yield return new WaitForSeconds(0.02f);
			throwCube(leftForce);
	}
	IEnumerator deSpawnCube(int cubeIndex, float seconds){
		yield return new WaitForSeconds(seconds);
		cubes[cubeIndex].SetActive(false);
	}
	// Update is called once per frame
	void Update () {
	}

	void throwCube(Vector2 throwForce){
		//is it faster to 
		/*
		A- hold cube as gameObject, use .transform and .GetComponent
		B- hold cube as rigidbody2D, use .gameObject and .transform
		C- use an array of structs rather than gameObjects.
		 */
		cubes[nextActiveCube].SetActive(true);
		cubes[nextActiveCube].transform.position = transform.position;
		cubes[nextActiveCube].GetComponent<Rigidbody2D>().AddForce(throwForce);
		StartCoroutine(deSpawnCube(nextActiveCube, cubeLifetime));
		incrementActiveCube();
	}

	void incrementActiveCube(){
		nextActiveCube++;
		if (nextActiveCube >= cubes.Length){
			nextActiveCube = 0;
		}
	}
}
