using UnityEngine;
using System.Collections;

public class RunScript : MonoBehaviour {

	public Vector3 		Target;
	public float 		moveSpeed;
	public float 		delta;
	private Animator 	anim;
	public AudioSource	sound;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		sound = GetComponent<AudioSource>();
		moveSpeed = 1;
		delta = 0.3f;
	}
	
	// Update is called once per frame
	void	RecoverDir(){
		if ((Target.x - transform.position.x) > delta) {
			if ((Target.y - transform.position.y) > delta)
				anim.SetInteger ("Dir", 1);
			else if ((transform.position.y - Target.y) > delta)
				anim.SetInteger ("Dir", 3);
			else
				anim.SetInteger ("Dir", 2);
		} else if ((transform.position.x - Target.x) > delta) {
			if ((Target.y - transform.position.y) > delta)
				anim.SetInteger ("Dir", 7);
			else if ((transform.position.y - Target.y) > delta)
				anim.SetInteger ("Dir", 5);
			else
				anim.SetInteger ("Dir", 6);
		} else {
			if ((Target.y - transform.position.y) > delta)
				anim.SetInteger ("Dir", 0);
			else
				anim.SetInteger ("Dir", 4);
		}
	}

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Debug.Log (Input.mousePosition);
			Target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Target.z = 0f;
			if (!sound.isPlaying)
				sound.Play();
			RecoverDir();
			anim.SetBool("Run", true);
		}
		if (anim.GetBool("Run") == true) {
			transform.position = Vector3.MoveTowards (transform.position, Target , moveSpeed * Time.deltaTime);
			if (transform.position.x == Target.x && transform.position.y == Target.y ){
				anim.SetBool("Run", false);
			}
		}
	}
}
