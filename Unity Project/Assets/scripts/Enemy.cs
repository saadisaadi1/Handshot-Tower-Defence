using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    private AudioManager audio_manager;
    // Start is called before the first frame update
    public string type;
    public float max_walk_speed;
    public float acceleration;
    private float walk_speed;
    private Vector3 starting_position;
    private Animator animator;
    private GameManager game_manager;
    private bool dead;
    [Header("Health Bar")]
    public Canvas health_canvas;
    public Image health_bar;
    public TextMeshProUGUI health_text;
    public int max_health;
    private int health_amount;
    public Transform collider_transform;
    public GameObject dust;

    void Awake()
    {
        game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
        audio_manager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<AudioManager>();
        walk_speed = max_walk_speed;
        health_amount = max_health;
        if(health_text != null)
        {
            health_text.text = health_amount.ToString();
        }
        animator = transform.GetComponent<Animator>();
        if(animator == null)
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
        starting_position = transform.position;
        if(type == "pirate")
        {
            animator.SetBool("pirate", true);
        }
        else if (type == "target")
        {
            transform.position = new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(1f, 4.5f), Random.Range(-3f, -1f) );
        }
        else if (type == "eye")
        {
            transform.position = new Vector3(transform.position.x, 15f, transform.position.z + 25f);
        }
        else if(type == "orc")
        {
            audio_manager.Play("orc start", transform.position);
        }
        if(dust != null)
        {
            Instantiate(dust, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type != "target" && type != "eye")
        {

            if (!dead)
            {
                walk_speed = Mathf.Min(walk_speed + acceleration * Time.deltaTime, max_walk_speed);
                bool knocked = false;
                if (walk_speed <= 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(starting_position.x / 2f, transform.position.y, -10000f), walk_speed * Time.deltaTime);
                    knocked = true;
                }
                if (!knocked)
                {
                    if (transform.position.z > -7.5)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(starting_position.x / 2f, transform.position.y, -7.5f), walk_speed * Time.deltaTime);
                    }
                    else
                    {
                        animator.SetBool("reached", true);
                    }
                }
            }
        }
        else if (type == "eye")
        {
            if (!dead)
            {
                walk_speed = Mathf.Min(walk_speed + acceleration * Time.deltaTime, max_walk_speed);
                bool knocked = false;
                if (walk_speed <= 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(starting_position.x / 2f, transform.position.y, -10000f), walk_speed * Time.deltaTime);
                    knocked = true;
                }
                if (!knocked)
                {
                    if (transform.position.z > -7.5)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(starting_position.x / 2f, 1.5f, -7.5f), walk_speed * Time.deltaTime);
                    }
                    else
                    {
                        animator.SetBool("reached", true);
                    }
                }
            }
        }
    }

    public void Damage(int damage, bool healing, float knockbock)
    {
        if(type == "skeleton" || type == "pirate")
        {
            audio_manager.Play("skeleton hurt", transform.position);
        }
        else if (type == "eye")
        {
            audio_manager.Play("eye hurt", transform.position);
        }
        else if (type == "orc")
        {
            int hurt_index = Random.Range(0, 3);
            if (hurt_index == 0)
            {
                audio_manager.Play("hurt1", transform.position);
            }
            else if (hurt_index == 1)
            {
                audio_manager.Play("hurt2", transform.position);
            }
            else if (hurt_index == 2)
            {
                audio_manager.Play("hurt3", transform.position);
            }
        }
        health_amount -= damage;
        animator.SetBool("reached", false);
        walk_speed = -knockbock;
        if (health_bar != null)
        {
            health_bar.fillAmount = health_amount / (float)max_health;
            if (health_text != null)
            {
                health_text.text = health_amount.ToString();
            }
        }
        if (health_amount <= 0)
        {
            if(type != "eye" && type != "target")
            {
                collider_transform.GetComponent<Collider>().enabled = false;
            }
            else
            {
                collider_transform.tag = "Untagged";
            }
            if (health_canvas != null)
            {
                health_canvas.enabled = false;
            }
            if (healing)
            {
                game_manager.Heal();
            }
            Die();
        }
    }

    public void Die()
    {
        if (!dead)
        {
            animator.SetBool("dead", true);
            game_manager.CountEnemyAsDead();
            dead = true;
        }
    }

    public void Hit()
    {
        audio_manager.Play("get hit", transform.position);
    }
}
