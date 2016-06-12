using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownScript : MonoBehaviour {

	public float 		timer;
	public GameObject	WarriorObj;
	public GameObject	Hotel;
	public float		spawn;
	public	bool		win;
	public	string		type;
	public bool			beingAttacked;
	public AudioClip[]	AudioAttacked;
	public AudioSource 	my_audio;

	// Use this for initialization
	void Start () {
		beingAttacked = false;
		spawn = 10;
		win = false;
		if (Hotel.tag == "EnemyBuilding")
			type = "Orc";
		else
			type = "Human";
		my_audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (beingAttacked == true){
			my_audio.clip = AudioAttacked[Random.Range(0, AudioAttacked.Length)];
			my_audio.Play();
			beingAttacked = false;
		}
		timer += Time.deltaTime;
		if (Hotel != null) {
			if (timer >= spawn) {

				timer = 0;
				GameObject tmp = Instantiate (WarriorObj, WarriorObj.transform.position, WarriorObj.transform.rotation) as GameObject;
				tmp.SetActive (true);
			}
		}
		if (win == false && Hotel == null){
			Debug.Log("The " + type + " Team wins");
			win	= true;
		}			
	}
}
