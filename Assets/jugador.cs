using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jugador : MonoBehaviour {
    public float upForce = 1;                   //Upward force of the "flap".
    private bool isDead = false;            //Has the player collided with a wall?
    public GameObject salto;
    public GameObject muerte;
    public AudioSource audioFondo;
    public GameObject musicaFondo;

    private Animator anim;                  //Reference to the Animator component.
    private Rigidbody2D rb2d;               //Holds a reference to the Rigidbody2D component of the bird.

    // Use this for initialization
    void Start () {
        //Get reference to the Animator component attached to this GameObject.
        anim = GetComponent<Animator>();
        //Get and store a reference to the Rigidbody2D attached to this GameObject.
        rb2d = GetComponent<Rigidbody2D>();
        audioFondo = musicaFondo.GetComponent<AudioSource>(); ;
        audioFondo.Play();

    }
    

    

    // Update is called once per frame
    void Update () {
        //Don't allow control if the bird has died.
        if (isDead == false)
        {
            //Look for input to trigger a "flap".
            if (Input.GetMouseButtonDown(0))
            {
                AudioSource audioSalto= salto.GetComponent<AudioSource>(); ;
                audioSalto.Play();
                //...tell the animator about it and then...
                anim.SetTrigger("Flap");
                //...zero out the birds current y velocity before...
                rb2d.velocity = Vector2.zero;
                //  new Vector2(rb2d.velocity.x, 0);
                //..giving the bird some upward force.
                rb2d.AddForce(new Vector2(0, upForce));

                
       
            }
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {

        // Zero out the bird's velocity
        rb2d.velocity = Vector2.zero;
        // If the bird collides with something set it to dead...
        isDead = true;
        //...tell the Animator about it...
        anim.SetTrigger ("Die");
        //...and tell the game control about it.
        AudioSource audioMuerte = muerte.GetComponent<AudioSource>(); ;
        audioMuerte.Play();
        audioFondo.Stop();

        GameControl.instance.BirdDied ();
    }
}
