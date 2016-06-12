using UnityEngine;
using System.Collections;

public class BuildingScript : MonoBehaviour {
	
	public int 		hp;
	public int 		MaxHp;
	public GameObject Town;
	public bool 	send;
	// Use this for initialization
	void Start () {
		send = false;
		MaxHp = hp;
	}
	
	// Update is called once per frame
	void Update () {
		if (Town != null) {
			if (MaxHp > hp && !send) {
				Town.GetComponentInParent<TownScript> ().beingAttacked = true;
				send = true;
			}
		}
		if (hp <= 0) {
			GetComponent<Collider2D>().enabled = false;
			if (Town != null)
				Town.GetComponentInParent<TownScript>().spawn += 2.5f;
			Destroy (this.gameObject);
		}
	}
}
