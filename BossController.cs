using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BossController : MonoBehaviour
{

    public float speed;
    private float timeBtwAttacks = 2f;
    private float shootTimer;
    float fireRate;
    float nextFire;
    int currentHealth;
    public int maxHealth = 3;
    public int health { get { return currentHealth; } }



    public AudioClip fixSound;
    public AudioClip hitSound;


    public GameObject enemyProjectile;


    public bool vertical;
    public float changeTime = 3.0f;
    public ParticleSystem smokeEffect;




    bool broken;

    Rigidbody2D rigidbody2D;

    float timer;
    int direction = 1;

    private RubyController rubyController;


    Animator animator;
    AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        broken = true;
        currentHealth = maxHealth;
        GameObject rubyControllerObject = GameObject.FindWithTag("Player");
        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
            print("Found the RubyController Script!");
        }
        if (rubyController == null)
        {
            print("Cannot find GameController Script!");
        }




    }

    void Update()
    {
        timer -= Time.deltaTime;
        shootTimer += Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
        if (!broken)
        {
            return;
        }
        if (transform.position.y < 8)
        {
            transform.position = new Vector3(transform.position.x, 8, transform.position.z);
        }
        if (shootTimer >= timeBtwAttacks)
        {
            shootTimer = 0;
            Fire();
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);

        if (!broken)
        {
            return;
        }
    }
    void Fire()
    {
        if (broken == true)
        {
            Instantiate(enemyProjectile, transform.position, Quaternion.identity);

            nextFire = Time.time + fireRate;


        }
    }
    public void Fix()
    {

        broken = false;
        rigidbody2D.simulated = false;
        smokeEffect.Stop();
        rubyController.ChangeScore(1);
        rubyController.PlaySound(fixSound);

    }
    public void ChangeHealthBoss(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBarBoss.instance.SetValue((float)currentHealth / maxHealth);
        BossController controller = GetComponent<BossController>();
        RubyController player = GetComponent<RubyController>();

        if (currentHealth == 0)
        {
            Fix();
            player.ChangeScore(1);
        }

    }
    public void PlaySound(AudioClip clip)
    {
        audiosource.PlayOneShot(clip);
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }

    }
}
