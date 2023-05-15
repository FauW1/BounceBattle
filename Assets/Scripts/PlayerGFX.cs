using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    //character select
    [SerializeField] private Character animalType;
    
    //controls
    [SerializeField] private string hAxis = "Horizontal";
    [SerializeField] private string jump = "Jump";
    private KeyCode down;

    //audio
    private AudioSource jumpSound;
    private AudioSource landSound;

    //prefab
    [SerializeField] private GameObject dust;

    //check for animation
    private PlayerMovement playerScript;
    private Rigidbody2D parentRB;
    private Collider2D groundCheck;
    private LayerMask ground;
    private LayerMask player;
    private Animator anim;
    private SoundManager sound;

    private void Start()
    {
        sound = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();
        jumpSound = sound.jump;
        landSound = sound.land;

        playerScript = GetComponentInParent<PlayerMovement>();
        parentRB = GetComponentInParent<Rigidbody2D>();
        groundCheck = GetComponent<BoxCollider2D>();
        
        ground = LayerMask.GetMask("Ground");
        player = LayerMask.GetMask("Player");
        
        anim = GetComponent<Animator>();
        down = playerScript.downKey;

        if(playerScript.gameObject.CompareTag("Player 1")) //set the character number in animator to match static character number on player
        {
            anim.SetFloat("CharNum", CharacterManager.player1/3f); //div by float so outcome is a float
        }
        else
        {
            anim.SetFloat("CharNum", CharacterManager.player2 / 3f);
        }
    }

    private void FixedUpdate() //animation
    {
        Animation();
    }

    private void Animation() //manages animation according to action
    {
        if (Input.GetButtonDown(jump)) //jump takeoff
        {
            if (playerScript.hangCount >= 0 && playerScript.buffCount >= 0 && !Input.GetKey(playerScript.downKey))
            {
                jumpSound.Play();
                anim.SetTrigger("TakeOff");
            }
        }

        if(parentRB.velocity.y > 0.1) //jump
        {
            anim.SetBool("IsJumping", true);
        }
        else if (anim.GetBool("IsJumping") && parentRB.velocity.y < 0.1) //fall
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", true);
        }
        else if (groundCheck.IsTouchingLayers(ground) || groundCheck.IsTouchingLayers(player)) //land from fall
        {
            if (anim.GetBool("IsFalling"))
            {
                landSound.Play();
                Instantiate(dust, transform.position, Quaternion.identity);
            }
            anim.SetBool("IsFalling", false);
        }

        float horizontal = Input.GetAxis(hAxis); //running animation

        if(Mathf.Abs(horizontal) != 0f) //epsilon is smallest possible value
        {
            anim.SetBool("IsRunning", true);
        } 
        else if (Input.GetKey(down))
        {
            anim.SetTrigger("Squish"); //moving takes precedent over squishing down, which takes precedent over idle
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }

        if (playerScript.abilityOn)
        {
            anim.SetBool("IsPower", true);
        }
        else
        {
            anim.SetBool("IsPower", false);
        }
    }

    private IEnumerator BecomeTemporarilyInvincible() //change to altering scale STILL DOESN'T WORK
    {
        Debug.Log("Player turned invincible!");
        playerScript.invincible = true;

        for (float i = 0; i < playerScript.invincibleTime; i += playerScript.invincibleDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (transform.localScale == Vector3.one)
            {
                transform.localScale = Vector3.zero;
            }
            else
            {
                transform.localScale = Vector3.one;
            }
            yield return new WaitForSeconds(playerScript.invincibleDeltaTime);
        }

        Debug.Log("Player is no longer invincible!");
        transform.localScale = Vector3.one;
        playerScript.invincible = false;
    }

    //code from: https://www.aleksandrhovhannisyan.com/blog/dev/invulnerability-frames-in-unity/ 
}
