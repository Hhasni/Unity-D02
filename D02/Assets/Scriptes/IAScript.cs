using UnityEngine;
using System.Collections;

public class IAScript : MonoBehaviour {

	public GameObject[] BuildingOrc;
	public int			hp;
	public int			dam;
	public float		my_time;
	public float		timer;
	public float		moveSpeed;
	public GameObject	Target;
	private Animator 	anim;
	private float		delta;
	public bool 		isDead;
	public bool 		win;
	public AudioClip 	deathSound;
	public AudioSource 	my_audio;
	private bool		isReach;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		hp = 100;
		dam = 50;
		moveSpeed = 1f;
		delta = 0.3f;
		AttackCloser ();
	}

	void AttackCloser (){
		if (BuildingOrc [0]) {
			float Origin = Vector2.Distance (transform.position, BuildingOrc [0].transform.position);
			float tmp = Origin;
			Target = BuildingOrc [0].gameObject;
			for (int i = 0; i < BuildingOrc.Length; i++) {
				if (BuildingOrc [i]) {
					tmp = Vector2.Distance (transform.position, BuildingOrc [i].transform.position);
					if (Origin > tmp) {
						Origin = tmp;
						Target = BuildingOrc [i].gameObject;
					}
				}
			}
		} else if (!win){
			win = true;
			Debug.Log("The Human Team win"); 
		}
	}

	void	RecoverDir(Vector2 Vec){
		if ((Vec.x - transform.position.x) > delta) {
			if ((Vec.y - transform.position.y) > delta)
				anim.SetInteger ("Dir", 1);
			else if ((transform.position.y - Vec.y) > delta)
				anim.SetInteger ("Dir", 3);
			else
				anim.SetInteger ("Dir", 2);
		} else if ((transform.position.x - Vec.x) > delta) {
			if ((Vec.y - transform.position.y) > delta)
				anim.SetInteger ("Dir", 7);
			else if ((transform.position.y - Vec.y) > delta)
				anim.SetInteger ("Dir", 5);
			else
				anim.SetInteger ("Dir", 6);
		} else {
			if ((Vec.y - transform.position.y) > delta)
				anim.SetInteger ("Dir", 0);
			else
				anim.SetInteger ("Dir", 4);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		//Debug.Log (col.gameObject.tag + " " + col.gameObject.name);
		if (col.gameObject.tag == "Player") {
			Target = col.gameObject;
			isReach = true;
		}
	}

	void fight(){
		Debug.Log ("ATTACK PLAYER");
		if (!anim.GetBool ("Fight"))
			anim.SetBool ("Fight", true);
		if (anim.GetBool ("Run"))
			anim.SetBool ("Run", false);
		Target.GetComponent<CharacterScript> ().hp -= dam;
		if ((Target.GetComponent<CharacterScript> ().hp - dam) >= 0) {
			Debug.Log ("Orc Unit [" + 
				Target.GetComponent<CharacterScript> ().hp
				+ "/100 HP has been attacked.");
		} else	Debug.Log ("Orc Unit [0/100] HP has been attacked.");
	}

	void attack_building(){
		Debug.Log ("ATTACK BUILINDG");
		if (!anim.GetBool ("Fight"))
			anim.SetBool ("Fight", true);
		if (anim.GetBool ("Run"))
			anim.SetBool ("Run", false);
		Target.GetComponent<BuildingScript> ().hp -= dam;
		if ((Target.GetComponent<BuildingScript> ().hp - dam) >= 0) {
			Debug.Log("Orc Building [" + 
			          Target.GetComponent<BuildingScript> ().hp
			          +"/100 HP has been attacked.");
		}
		else
			Debug.Log ("Orc Buildin [0/100] HP has been attacked.");
	}

	void OnTriggerStay2D(Collider2D col){
		if (isDead)
			return;
		if (timer >= 1) {
			timer = 0;
			if (col != null) {
				if (col.tag == "Player") {
					Target = col.gameObject;
					fight ();
				}
				if (col.tag == "PlayerBuilding") {
					Target = col.gameObject;
					attack_building ();
				}
			}
			if (Target != null) {
				if (Target != null && Target.tag == "Player"){
					CharacterScript tmp = Target.GetComponent<CharacterScript> ();
					if (tmp){
						if (tmp.hp <= 0){
							Debug.Log("ALOA");
							anim.SetBool ("Fight", false);
							Target = null;
						}
					}
				}
				if (Target != null && Target.tag == "PlayerBuilding"){
					if (Target.GetComponent<BuildingScript> ().hp <= 0){
						anim.SetBool ("Fight", false);
						Target = null;
					}
				}
			}
		}
		timer += Time.deltaTime;
	}

	void DestroyAfterPlaying(){
		Destroy(this.gameObject);
	}

	
	void	ft_check_life(){
		if (hp <= 0) {
			if (!anim.GetBool("Death"))
				anim.SetBool ("Death", true);
			if (!my_audio.isPlaying){
				my_audio.clip = deathSound;
				my_audio.Play();
			}
			Invoke ("DestroyAfterPlaying", my_audio.clip.length);
		}
	}

	// Update is called once per frame
	void Update () {
		ft_check_life ();
		if (Target == null) {
			anim.SetBool ("Fight", false);
			AttackCloser();
			Debug.Log("YESS");
			isReach = false;
		}
		if (Target && !isReach) {
			RecoverDir (Target.transform.position);
			anim.SetBool("Run", true);
			transform.position = Vector3.MoveTowards (transform.position, Target.transform.position, moveSpeed * Time.deltaTime);
			if (transform.position.x >= Target.transform.position.x - delta && transform.position.x <= Target.transform.position.x + delta
			    && transform.position.y >= Target.transform.position.y - delta && transform.position.y <= Target.transform.position.y + delta) {
				isReach = true;
				anim.SetBool ("Run", false);
			}
		}
		my_time += Time.deltaTime;
	}
}
