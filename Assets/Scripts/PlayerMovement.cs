using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("player GFX")]
    public PlayerGFX playergfx;

    //basic controls
    [Header("basic controls")]
    private Rigidbody2D rb;
    public string hAxisVer = "Horizontal";
    [SerializeField] private string jumpButton = "Jump";
    [SerializeField] private KeyCode abilityKey;
    public KeyCode downKey;
    private bool flipped = false; //used for kangaroo

    //colliders / other player
    [Header("colliders / other player")]
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private Collider2D coll;
    private LayerMask ground;
    private LayerMask player;
    [SerializeField] private string otherPlayer = "Player 2";

    //invincible timer
    [Header("invincible timer")]
    public bool invincible = false;
    public float invincibleTime = 3f;
    public float invincibleDeltaTime = 0.3f;

    //instantiate prefabs
    [Header("instantiate prefabs")]
    [SerializeField] private GameObject push;
    [SerializeField] private GameObject lost;
    [SerializeField] private ParticleSystem runDust;
    public GameObject dust;

    //player stats (based on SO)
    [Header("player stats")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float maxHealth = 100f;
    public float health = 10f;
    public float attack = 50f;
    private GameObject chrman;
    private Character charac;
    private int characterNumber;
    private bool smashInitiated = true;
    [SerializeField] private float smashAcc = 700f; //var for charac also

    //special abilities
    [Header("special abilities")]
    private bool abilityAvail = true;
    private float abilityDuration = 1f;
    private float tempBoost = 0f; //temporary attack boost
    public bool abilityOn = false;

    public Image mask;
    private float originalSize;
    private float coolDown;
    private float coolCount;

    //health UI
    [Header("health UI")]
    [SerializeField] private GameObject healthUI;
    private UIHealthBar healthBar;

    //Better jump system
    [Header("better jump system")]
    //hangtime
    [SerializeField] private float hangTime = 0.2f;
    public float hangCount;
    //jump buffer
    [SerializeField] private float jumpBuffer = 0.1f;
    public float buffCount;
    //quick fall
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float currentGravityVal = 1f;

    //sound
    private SoundManager sound;
    private AudioSource hitSound;
    private AudioSource dieSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sound = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();
        hitSound = sound.hit;
        dieSound = sound.die;
        
        //layermasks
        ground = LayerMask.GetMask("Ground");
        player = LayerMask.GetMask("Player");

        //access character type and stats
        chrman = GameObject.FindGameObjectWithTag("Character Manager");
        switch (tag)
        {
            case "Player 1":
                characterNumber = CharacterManager.player1;
                charac = chrman.GetComponent<CharacterManager>().characters[characterNumber]; //assign to player 1 character in array

                transform.localRotation = Quaternion.Euler(0, 180, 0); //flip character to face player 2
                flipped = true;
                break;
            case "Player 2":
                characterNumber = CharacterManager.player2;
                charac = chrman.GetComponent<CharacterManager>().characters[characterNumber]; //assign to player 2 character in array
                break;
            default:
                charac = chrman.GetComponent<CharacterManager>().characters[0]; //if something goes wrong, it's a kangaroo
                characterNumber = 0;
                break;
        }
        speed = charac.speed;
        jumpForce = charac.jumpForce;
        maxHealth = charac.health;
        attack = charac.attack;
        smashAcc = charac.smashAcc;
        
        abilityDuration = charac.abilityDuration;
        coolDown = charac.coolDown;
        originalSize = mask.rectTransform.rect.height;

        //health stuff
        healthBar = healthUI.GetComponent<UIHealthBar>(); //getting healthbar
        health = maxHealth;
        healthBar.SetText(health.ToString() + "/" + maxHealth.ToString());
    }

    //use fixedupdate for physics
    private void FixedUpdate() 
    {
        //health and ability
        HealthCheck();

        //movement and power
        Movement();
        if (Input.GetKeyDown(abilityKey))
        {
            StartCoroutine("Ability");
        }

        AbilityCool();
        if (groundCheck.IsTouchingLayers(ground)) 
        {
            if (characterNumber == 1) 
            {
                return;
            }
            else
            {
                abilityOn = false;
            }
        }
    }

    //moves player with legacy input system
    private void Movement()
    {

        float horizontal = Input.GetAxis(hAxisVer);

        if (horizontal > 0) //!on so that movement doesn't override rolling and stuff
        {
            if (characterNumber == 2)
            {
                rb.velocity = new Vector2(speed * horizontal * Time.deltaTime, rb.velocity.y);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                flipped = false;
            } else if (!abilityOn)
            {
                rb.velocity = new Vector2(speed * horizontal * Time.deltaTime, rb.velocity.y);
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                flipped = false;
            }
        }
        else if (horizontal < 0 && !abilityOn)
        {
            if (characterNumber == 2)
            {
                rb.velocity = new Vector2(speed * horizontal * Time.deltaTime, rb.velocity.y);
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                flipped = true;
            }
            else if (!abilityOn)
            {
                rb.velocity = new Vector2(speed * horizontal * Time.deltaTime, rb.velocity.y);
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                flipped = true;
            }
        }

        //run dust manager
        if(horizontal != 0 && groundCheck.IsTouchingLayers(ground))
        {
            runDust.Play();
        }
        else
        {
            if (characterNumber != 1) //so that rabbit roll makes dust too
            {
                runDust.Stop();
            }
            else if(!abilityOn)
            {
                runDust.Stop();
            }
        }

        //smash attack manager
        if (hangCount > 0 && buffCount >= 0)
        {
            smashInitiated = false;
        }

        if(Input.GetKeyDown(downKey) && !groundCheck.IsTouchingLayers(ground) && !smashInitiated)
        {
            smashInitiated = true;

            rb.AddForce(Vector2.down * smashAcc); //add force (may stack with low jump or fall things)
        }

        Jump();
    }

    private void Jump()
    {
        //hangtime manager
        if (groundCheck.IsTouchingLayers(ground) || groundCheck.IsTouchingLayers(player))
        {
            hangCount = hangTime;
        }
        else
        {
            hangCount-= Time.deltaTime;
        }

        //jump buffer manager
        if (Input.GetButtonDown(jumpButton))
        {
            buffCount = jumpBuffer;
        }
        else
        {
            buffCount -= Time.deltaTime;
        }

        //jump
        if(hangCount > 0 && buffCount >= 0 && !Input.GetKey(downKey))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            buffCount = 0;
        }

        //quickfall manager
        if (smashInitiated)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - currentGravityVal) * Time.deltaTime;
        }
        else if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - currentGravityVal) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton(jumpButton))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - currentGravityVal) * Time.deltaTime;
        }
    }

    /*private void Invincibility() //short time invincible
    {
        if (invincibleTimer <= 0f)
        {
            invincible = false;
            invincibleTimer = invincibleTime;
        }

        if (invincible)
        {
            invincibleTimer -= Time.deltaTime;
        }

        Debug.Log(invincibleTimer);
        Debug.Log(invincible);
    }*/

    private void AbilityCool()
    {
        if (!abilityAvail)
        {
            coolCount -= Time.deltaTime;
            mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize * (1-coolCount/coolDown)); //scale mask to show coolDown graphic
        }

        if(coolCount <= 0)
        {
            coolCount = coolDown;
            abilityAvail = true;
        }
    }

    //old code for getting hurt
    /*private void OnTriggerEnter2D(Collider2D collision) //getting hit [TEMPORARY] [12/7/2020: REWRITE!! MAKE THIS HITTING THE OTHER ONE INSTEAD, LIKE BRACKEY'S VID; PASS IN OWN ATTACKK]
    {
        if (collision.gameObject.tag == otherPlayer)
        {
            PlayerMovement otherScript = collision.gameObject.GetComponent<PlayerMovement>();
            float otherAttack = otherScript.attack;

            if (collision.gameObject.transform.position.y > transform.position.y && !invincible) //PROBLEM: if player jumps up beneath the other player and hits the other player's feet, the player's health decreases
            {
                health -= otherAttack; //can make a function called "GetHurt" and pass in an attack value
                if (health < 0)
                {
                    health = 0;
                }
                invincible = true;
                hitSound.Play();

                healthBar.SetText(health.ToString() + "/" + maxHealth.ToString());
                healthBar.SetValue(health/maxHealth);
                //Debug.Log(gameObject.tag + ", " + health);
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Instantiate(push, new Vector2(transform.position.x, transform.position.y + 0.3f), Quaternion.identity);
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision) //sometimes they get hurt together
    {
        if (collision.gameObject.tag == otherPlayer)
        {
            PlayerMovement otherScript = collision.gameObject.GetComponent<PlayerMovement>();

            if (collision == otherScript.groundCheck && collision.gameObject.transform.position.y >= transform.position.y) //if ground Check; if below (collision.gameObject.transform.position.y < transform.position.y)
            {
                //if (characterNumber != 0)
                //{
                    GetHurt(otherScript.attack + otherScript.tempBoost); //gets hurt
                /*}else if (collision.gameObject.transform.position.y != transform.position.y && !abilityOn && otherScript.abilityOn)
                {
                    GetHurt(otherScript.attack + otherScript.tempBoost);
                }*/ //problem with code: if both kangaroos sidekick, no damage
            }

            if (collision.gameObject.transform.position.y < transform.position.y)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); //this bounces
            }

            Instantiate(push, new Vector2(transform.position.x, transform.position.y + 0.3f), Quaternion.identity);
        }

        //turn collision back on
        if (!collision.gameObject.CompareTag("Platform"))
        {
            coll.isTrigger = false;
        }
    }

    //manage falling through
    private void OnCollisionStay2D(Collision2D collision) //when it stays on platform
    {
        if(collision.gameObject.CompareTag("Platform") && Input.GetKeyDown(downKey))
        {
            StartCoroutine(FallThru());
        }
    }
    IEnumerator FallThru()
    {
        coll.isTrigger = true;

        yield return new WaitForSeconds(.6f);

        coll.isTrigger = false;
    }

    private void HealthCheck() //dies
    {
        if(health <= 0)
        {
            dieSound.Play();
            Instantiate(lost, new Vector2(transform.position.x, transform.position.y + 0.3f), transform.localRotation);
            //Animator lostAnim = lost.GetComponent<Animator>();
            //lostAnim.SetFloat("CharNum", characterNumber/3f);
            healthBar.SetText("0/" + maxHealth.ToString());
            healthBar.SetValue(0 / maxHealth);
            health = 0;
        }
    }

    IEnumerator Ability() //right slash or q for power
    {
        float specVal = charac.specialValue;

        if (abilityAvail) {
            switch (characterNumber) //ROUGH DRAFT; IF ABILITY ON, DO TEMP BOOST ACCORDING TO CHAR TYPE
            {
                case 0: //kangaroo
                    if (flipped && !groundCheck.IsTouchingLayers(ground)) //sidekick toward what direction
                    {
                        rb.velocity = Vector2.left * specVal;
                        abilityAvail = false;
                        abilityOn = true;
                        tempBoost = -10; //on attack
                        groundCheck.offset = new Vector2(0.2f, 0.3f);
                        groundCheck.size = new Vector2(0.5f, 0.3f);
                    }
                    else if (!flipped && !groundCheck.IsTouchingLayers(ground))
                    {
                        rb.velocity = Vector2.right * specVal;
                        abilityAvail = false;
                        abilityOn = true;
                        tempBoost = -10; //on attack
                        groundCheck.offset = new Vector2(0.2f, 0.3f);
                        groundCheck.size = new Vector2(0.5f, 0.3f);
                    }
                    
                    Debug.Log("kangaroo abilities");

                    yield return new WaitForSeconds(abilityDuration); //set time to ability duration (sync animation to this as well)

                    groundCheck.offset = new Vector2(0.08f, 0f);
                    groundCheck.size = new Vector2(0.5f, 0.085f);
                    abilityOn = false;
                    tempBoost = 0;

                    break;
                case 1: //rabbit
                    if (flipped && groundCheck.IsTouchingLayers(ground)) //roll toward that direction
                    {
                        rb.velocity += Vector2.left * specVal;
                        abilityAvail = false;
                        abilityOn = true;
                        runDust.Play();
                    }
                    else if (!flipped && groundCheck.IsTouchingLayers(ground))
                    {
                        rb.velocity += Vector2.right * specVal;
                        abilityAvail = false;
                        abilityOn = true;
                        runDust.Play();
                    }
                    Debug.Log("rabbit abilities");

                    yield return new WaitForSeconds(abilityDuration); //set time to ability duration (sync animation to this as well)

                    runDust.Stop();
                    abilityOn = false;
                    break;
                case 2: //frog
                    if (!groundCheck.IsTouchingLayers(ground))
                    {
                        rb.velocity = new Vector2(rb.velocity.x, specVal);
                        abilityAvail = false;
                        abilityOn = true;
                        Instantiate(dust, transform.position, Quaternion.identity);
                    }
                    Debug.Log("frog abilities");

                    yield return new WaitForSeconds(abilityDuration);

                    abilityOn = false;
                    break;
                case 3: //puma
                    if (!groundCheck.IsTouchingLayers(ground) && !groundCheck.IsTouchingLayers(player))
                    {
                        rb.AddForce(Vector2.down * specVal);
                        Debug.Log("puma abilities");

                        tempBoost = 10;
                        abilityAvail = false;
                        abilityOn = true;
                    }

                    yield return new WaitForSeconds(abilityDuration);

                    abilityOn = false;

                    break;
                default:
                    Debug.Log("ERROR");
                    break;
            }
        }
    }

    public void GetHurt(float damage)
    {
        if (invincible) return;
        
        health -= damage; //can make a function called "GetHurt" and pass in an attack value
        playergfx.StartCoroutine("BecomeTemporarilyInvincible");
        hitSound.Play();

        healthBar.SetText(health.ToString() + "/" + maxHealth.ToString());
        healthBar.SetValue(health / maxHealth);
    }
}
