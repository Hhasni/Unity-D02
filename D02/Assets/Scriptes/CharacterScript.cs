using UnityEngine;
using System.Collections;

public class CharacterScript : MonoBehaviour {

	public float		moveSpeed;
	public Vector3		Target;
	public GameObject	TargetObj;
	public float 		delta;
	public float 		timer;
	public float 		my_time;
	private Animator 	anim;
	public int			hp;
	public int			dam;
	public bool 		isDead;
	public bool 		fighting;
	public bool 		arrived;
	public AudioClip 	deathSound;
	public AudioSource 	my_audio;
	public int 			selectedNb;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		moveSpeed = 1f;
		Target = transform.position;
		delta = 0.3f;
		hp = 100;
		dam = 40;
		timer = 0;
		fighting = false;
		arrived = true;
	}

	void 	OnTriggerEnter2D(Collider2D col){
		if (isDead)
			return;
	//	Debug.Log (col.tag);
		if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "EnemyBuilding") {
			if (anim.GetBool("Fight") == false)
				anim.SetBool ("Fight", true);
		}

	}

	void 	OnTriggerExit2D(Collider2D col){
		if (isDead)
			return;
		if (col.gameObject.tag == "Enemy") {
			if (anim.GetBool("Fight")){
				anim.SetBool ("Fight", false);
				anim.SetBool("Run", true);
			}
			if (arrived)
				TargetObj = col.gameObject;
		}
	}

	void 	fight(){
		if (!anim.GetBool ("Fight"))
			anim.SetBool ("Fight", true);
		if (anim.GetBool ("Run"))
			anim.SetBool ("Run", false);
		CharacterScript tmp = TargetObj.GetComponent<CharacterScript> ();
		if (tmp){
			if ((tmp.hp - dam) >= 0) {
				TargetObj.GetComponent<CharacterScript> ().hp -= dam;
				Debug.Log ("Human Unit [" + 
			           TargetObj.GetComponent<CharacterScript> ().hp
				+ "/100 HP has been attacked.");
			}
		}
		else {
			TargetObj.GetComponent<IAScript> ().hp -= dam;
			if ((TargetObj.GetComponent<IAScript>().hp - dam) >= 0) {
				Debug.Log ("Human Unit [" + 
				           TargetObj.GetComponent<IAScript> ().hp
				           + "/100 HP has been attacked.");
			}
			else
				Debug.Log ("Human Unit [0/100] HP has been attacked.");
		}
	}

	void 	attack_building(){
		if (!anim.GetBool ("Fight"))
			anim.SetBool ("Fight", true);
		if (anim.GetBool ("Run"))
			anim.SetBool ("Run", false);
		TargetObj.GetComponent<BuildingScript> ().hp -= dam;
		if ((TargetObj.GetComponent<BuildingScript> ().hp - dam) >= 0) {
			Debug.Log ("Human Building [" + 
				TargetObj.GetComponent<BuildingScript> ().hp
				+ "/100 HP has been attacked.");
		} else
			Debug.Log ("Human Building [0/100] HP has been attacked.");
	}

	void 	OnTriggerStay2D(Collider2D col){
		if (isDead)
			return;
		if (timer >= 0.75) {
			timer = 0;
			if (col != null){
				if( col.tag == "Enemy"){
					TargetObj = col.gameObject;
					fight();
				}
				if( col.tag == "EnemyBuilding"){
					TargetObj = col.gameObject;
					attack_building();
				}
			}
		}
		if (TargetObj != null) {
			if (TargetObj != null && TargetObj.tag == "Enemy"){
				CharacterScript tmp = TargetObj.GetComponent<CharacterScript> ();
				if (tmp){
					if (tmp.hp <= 0)	{
						anim.SetBool ("Fight", false);
						TargetObj = null;
					}
				}
				else {
					if (TargetObj.GetComponent<IAScript> ().hp <= 0)	{
						anim.SetBool ("Fight", false);
						TargetObj = null;
					}
				}
			}
			if (TargetObj != null && TargetObj.tag == "EnemyBuilding"){
				if (TargetObj.GetComponent<BuildingScript> ().hp <= 0){
					anim.SetBool ("Fight", false);
					TargetObj = null;
				}
			}
		}
		timer += Time.deltaTime;
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

	public void active(){
		if (TargetObj)
			RecoverDir(TargetObj.transform.position);
		else
			RecoverDir(Target);
		arrived = false;
		anim.SetBool ("Run", true);
	}

	
	void DestroyAfterPlaying(){
		Destroy(this.gameObject);
	}
	
	
	void	ft_check_life(){
		if (hp <= 0) {
			isDead = true;
			if (!anim.GetBool("Death"))
				anim.SetBool ("Death", true);
			my_audio.clip = deathSound;
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
		if (isDead)
			return;
		if (my_time >= 20 && selectedNb > 0) {
			selectedNb = 0;
			my_time = 0;
		}
		if (anim.GetBool("Fight") && TargetObj == null)
			anim.SetBool ("Fight", false);
	//	StartCoroutine (ft_check_life ());
		if (anim.GetBool ("Run") == true) {
			if (TargetObj){
				Target = TargetObj.transform.position;
				RecoverDir(TargetObj.transform.position);
			}
			else
				RecoverDir(Target);
			transform.position = Vector3.MoveTowards (transform.position, Target, moveSpeed * Time.deltaTime);
			if (transform.position.x >= Target.x - delta && transform.position.x <= Target.x + delta
				&& transform.position.y >= Target.y - delta && transform.position.y <= Target.y + delta) {
				arrived = true;
				anim.SetBool ("Run", false);
			}
		}
		my_time += Time.deltaTime;
	}
}
