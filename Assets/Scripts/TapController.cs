using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 2;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rigidbody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 40);
        rigidbody.simulated = false;
    }

    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOver += OnGameOver;        
    }

    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOver -= OnGameOver;
    }

    void OnGameStarted() {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;

    }

    void OnGameOver() {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;

    }


    void Update() {        
        if (Input.GetMouseButtonDown(0)) { // GetMouseButtonDown(0) translates to Touch on mobile phones
            tapAudio.Play();
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force); // try .Impuls
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
        
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "ScoreZone"){
            // register Score Event, play sound
            OnPlayerScored(); // event sent to GameManager
            scoreAudio.Play();
        }
        
        if (col.gameObject.tag == "DeadZone"){
            rigidbody.simulated = false;
            // register a dead event, play sound
            OnPlayerDied(); // event sent to GameManager
            dieAudio.Play();

        }
        
    }


    
}
