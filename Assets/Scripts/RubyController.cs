using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip1win;
    public AudioClip clip2lose;
    public float volume=0.5f;

    public float speed = 3.0f;

    public Text fixedBots;

    public Text gameOverText;

    bool gameOver;

    private int score = 0;
    
    public int maxHealth = 5;
    
    public GameObject projectilePrefab;
    public GameObject damagePrefab;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;
        gameOver = false;

        audioSource = GetComponent<AudioSource>();

        gameOverText.text = " ";

        fixedBots.text = "Robots Fixed: " + score;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            }

        }
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeScore(int amount)
    {
        score = score + amount;
        fixedBots.text = "Robots Fixed: " + score;
        if(score == 5)
        {
            gameOverText.text = "You Win! Game by Grayson Wells - R to Restart";
            gameOver = true;
            audioSource.PlayOneShot(clip1win, volume);
            speed = 0.0f;
        }



    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject DamageParticle = Instantiate(damagePrefab, transform.position, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth == 0)
        {
            gameOverText.text = "You Lose! Press R to Restart";
            gameOver = true;
            speed = 0.0f;
            audioSource.PlayOneShot(clip2lose, volume);
        }
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "SlowGrass")
        {
            speed = 1.0f;
        }
    }
     void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "SlowGrass")
        {
            speed = 3.0f;
        }
    }
}