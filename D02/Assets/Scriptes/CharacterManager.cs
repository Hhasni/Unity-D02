using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

	public List<GameObject> lst;
	public AudioClip[] 	SelectAudio;
	public AudioClip[] 	OrderAudio;
	public AudioClip[] 	AnnoyedAudio;
	public AudioSource	my_audio;
	public float		timer;
	public int			selectNb;
	// Use this for initialization
	void Start () {
		my_audio = GetComponent<AudioSource>();
	}

	IEnumerator PlaySound(){
		my_audio.Play ();
		yield return new WaitForSeconds(my_audio.clip.length);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if (hit.collider != null && hit.collider.tag == "Player") {
				if (!lst.Contains (hit.collider.gameObject)) {
					lst.Add (hit.collider.gameObject);
					if (!my_audio.isPlaying) {
						my_audio.clip = SelectAudio [Random.Range (0, SelectAudio.Length)];
						StartCoroutine (PlaySound ());
					}
				} else {
					hit.collider.gameObject.GetComponent<CharacterScript> ().selectedNb += 1;
					if (!my_audio.isPlaying) {
						if (hit.collider.gameObject.GetComponent<CharacterScript> ().selectedNb > 4)
							my_audio.clip = AnnoyedAudio [Random.Range (0, AnnoyedAudio.Length)];
						else
							my_audio.clip = SelectAudio [Random.Range (0, SelectAudio.Length)];
						StartCoroutine (PlaySound ());
					}
				}
			}
		} else if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if (hit.collider != null && hit.collider.tag == "Player") {
				if (lst.Count > 0) {
					if (!lst.Contains (hit.collider.gameObject)) {
						foreach (GameObject obj in lst) {
							if (obj)
								obj.GetComponent<CharacterScript> ().selectedNb = 1;
						}
						lst = new List<GameObject> ();

					} else
						hit.collider.gameObject.GetComponent<CharacterScript> ().selectedNb += 1;

				} else {
					lst.Add (hit.collider.gameObject);
					hit.collider.gameObject.GetComponent<CharacterScript> ().selectedNb += 1;
				}
				if (!my_audio.isPlaying) {
					if (hit.collider.gameObject.GetComponent<CharacterScript> ().selectedNb > 4)
						my_audio.clip = AnnoyedAudio [Random.Range (0, AnnoyedAudio.Length)];
					else
						my_audio.clip = SelectAudio [Random.Range (0, SelectAudio.Length)];
					StartCoroutine (PlaySound ());
				}

			}
			if (hit.collider != null && lst.Count > 0 && hit.collider.tag == "Enemy") {
				Debug.Log ("HELLO");
				//	selectNb = 0;
				if (!my_audio.isPlaying) {
					my_audio.clip = OrderAudio [Random.Range (0, OrderAudio.Length)];
					StartCoroutine (PlaySound ());
				}
				foreach (GameObject elem in lst) {
					elem.GetComponent<CharacterScript> ().TargetObj = hit.collider.gameObject;
					elem.GetComponent<CharacterScript> ().active ();
				}
			} else {
				//selectNb = 0;
				if (lst.Count > 0 && !my_audio.isPlaying) {
					my_audio.clip = OrderAudio [Random.Range (0, OrderAudio.Length)];
					StartCoroutine (PlaySound ());
				}
				foreach (GameObject elem in lst) {
					if (elem) {
						Vector3 tmp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
						tmp.z = 0f;
						elem.GetComponent<CharacterScript> ().Target = tmp;
						elem.GetComponent<CharacterScript> ().TargetObj = null;
						elem.GetComponent<CharacterScript> ().active ();
					}
				}
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			foreach (GameObject obj in lst)
				obj.GetComponent<CharacterScript> ().selectedNb = 1;
			lst = new List<GameObject> ();
		}
		foreach (GameObject obj in lst) {
			if (!obj)
				lst = new List<GameObject> ();
		}
	
	//	timer += Time.deltaTime;
	}
}
