using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private AudioManager audio_manager;
    private GameManager game_manager;
    public float max_arrow_speed;
    public float min_arrow_speed;
    public float gravity;
    public GameObject explosion;
    public float explosion_radius;
    private bool stopped;
    [HideInInspector] Bow bow;
    [HideInInspector] public float max_piercing;
    [HideInInspector] public float piercing_number;
    [HideInInspector] public float max_bounce;
    [HideInInspector] public float bounce_number;
    [HideInInspector] public bool explosive;
    [HideInInspector] public bool critical;
    [HideInInspector] public bool healing;
    [HideInInspector] public bool big;
    [HideInInspector] public int damage;
    [HideInInspector] public float knockback;
    public Transform Head_transform;
    private Rigidbody arrow_rb;
    private bool fired = false;
    // Start is called before the first frame update
    void Awake()
    {
        audio_manager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<AudioManager>();
        bow = GameObject.FindGameObjectWithTag("bow").GetComponent<Bow>();
        arrow_rb = GetComponent<Rigidbody>();
        game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fired && !stopped)
        {
            arrow_rb.velocity = arrow_rb.velocity - gravity * Vector3.up * Time.deltaTime;
            transform.forward = arrow_rb.velocity.normalized;
            if (Head_transform.position.y < 0)
            {
                if(bounce_number >= max_bounce)
                {
                    if (!explosive)
                    {
                        audio_manager.Play("arrow hit", transform.position);
                    }
                    stopped = true;
                    transform.position = new Vector3(transform.position.x, transform.position.y - Head_transform.position.y, transform.position.z);
                    arrow_rb.velocity = Vector3.zero;
                    transform.GetComponent<Collider>().enabled = false;
                }
                else
                {
                    if (!explosive)
                    {
                        audio_manager.Play("bounce", transform.position);
                    }
                    transform.position = new Vector3(transform.position.x, transform.position.y - Head_transform.position.y, transform.position.z);
                    arrow_rb.velocity = new Vector3(arrow_rb.velocity.x, -(3/4f) * arrow_rb.velocity.y, arrow_rb.velocity.z);
                }

                if (explosive)
                {
                    Explode(new Vector3(transform.position.x, transform.position.y - Head_transform.position.y, transform.position.z));
                    //if (bounce_number == max_bounce)
                    //{
                    //    Destroy(gameObject);
                    //}
                }
                bounce_number++;
            }
        }
    }

    public void FireArrow(Vector3 direction, float percent)
    {
        fired = true;
        arrow_rb.velocity = Mathf.Lerp(min_arrow_speed, max_arrow_speed, percent) * direction;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!stopped)
        {
            if (other.CompareTag("enemy"))
            {
                if(piercing_number == max_piercing)
                {
                    audio_manager.Play("arrow hit", transform.position);
                    stopped = true;
                    transform.position += other.ClosestPoint(Head_transform.position) - Head_transform.position;
                    Destroy(arrow_rb);
                    transform.SetParent(other.transform);
                    arrow_rb.velocity = Vector3.zero;
                    transform.GetComponent<Collider>().enabled = false;
                }
                else
                {
                    audio_manager.Play("arrow pass", transform.position);
                }
                if (explosive)
                {
                    Vector3 contact_point = other.ClosestPoint(Head_transform.position);
                    Explode(contact_point);
                    if(piercing_number == max_piercing)
                    {
                        Destroy(gameObject);
                    }
                }
                Transform topmost_parent = GetTopMostParent(other.transform);
                int critical_factor = 1;
                if (critical)
                {
                    int random_number = Random.Range(0, 8);
                    Debug.Log("random is = " + random_number.ToString());
                    if (random_number == 0)
                    {
                        critical_factor = 2;
                    }
                }
                topmost_parent.GetComponent<Enemy>().Damage(damage * critical_factor, healing, knockback);
                piercing_number += 1;
            }
        }
    }

    public void Explode(Vector3 explosion_point)
    {
        Instantiate(explosion, explosion_point, Quaternion.identity);
        audio_manager.Play("explosion", transform.position);
        Collider[] hitColliders = Physics.OverlapSphere(explosion_point, explosion_radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("enemy"))
            {
                Transform topmost_parent = GetTopMostParent(hitCollider.transform);
                topmost_parent.GetComponent<Enemy>().Damage(damage / 2, healing, knockback * 0.5f);
            }
        }
    }

    public Transform GetTopMostParent(Transform child)
    {
        Transform topmost_parent = child;
        int x = 0;
        while (x < 100)
        {
            x++;
            if (topmost_parent.parent != null)
            {
                topmost_parent = topmost_parent.parent;
            }
            else
            {
                break;
            }
        }
        return topmost_parent;
    }

    public void SetArrowToReady()
    {
        bow.ready = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Head_transform.position, explosion_radius);
    }
}
