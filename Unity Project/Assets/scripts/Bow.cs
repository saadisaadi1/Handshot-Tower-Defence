using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Bow : MonoBehaviour
{
    private GameManager game_manager;
    private AudioManager audio_manager;
    [HideInInspector] public Arrow arrow;
    [HideInInspector] public Vector3 starting_position;
    public Vector3 offset;
    private bool fired = false;
    [HideInInspector]public bool ready = false;
    public GameObject arrow_prefab;

    [Header("Bow Features")]
    public float bow_move_speed;
    public float bow_aiming_speed;
    private float angle;
    public float max_angle;
    public float min_angle;
    public float max_x_distance;

    [Header("Arrow Features")]
    public int damage;
    public int extra_damage;
    public float knockback;
    public float extra_knockback;
    public float arrow_reload_time;
    public float arrow_fast_reload_time;
    private float arrow_reload_timer;
    [HideInInspector] public float max_piercing;
    [HideInInspector] public float max_bounce;
    [HideInInspector] public bool explosive;
    [HideInInspector] public bool critical;
    [HideInInspector] public bool healing;
    [HideInInspector] public bool big;
    public Vector3 big_arrow_scale;

    [Header("Special Attack")]
    public float special_attack_duration;
    [HideInInspector] public float special_attack_timer;
    public float time_between_specail_arrows;
    [HideInInspector] public float special_between_timer;
    [HideInInspector] public bool has_special_attack = false;
    [HideInInspector] public bool used_special_attack;
    [HideInInspector] public bool using_special_attack;
    [HideInInspector] public Vector3 special_starting_shooting_position;
    public Vector3 special_offset;
    public float special_forward_factor;
    public Transform special_shooting_Transform;
    public Vector2 vertical_limits;
    public Vector2 horizontal_limits;
    public float special_speed;
    public GameObject aiming_sign;

    [Header("Shooting process")]
    public Image speed_image;
    public float pull_rate;
    [HideInInspector] public bool is_pulling;
    [HideInInspector] public float speed_percent;
    public float pulling_max_distance;
    public Gradient speed_gradient;
    private bool flipped;
    private Server server;
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.FindGameObjectWithTag("server").GetComponent<Server>();
        special_starting_shooting_position = special_shooting_Transform.position;
        fired = true;
        game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
        audio_manager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<AudioManager>();
        starting_position = transform.position + offset.x * transform.forward + offset.y * transform.right + offset.z * transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (!game_manager.editing && game_manager.in_wave && !using_special_attack && server.keyboard_on)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position -= bow_move_speed * Vector3.right * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += bow_move_speed * Vector3.right * Time.deltaTime;
            }
            if(transform.position.x > max_x_distance)
            {
                transform.position = new Vector3(max_x_distance, transform.position.y, transform.position.z);
            }
            else if(transform.position.x < -max_x_distance)
            {
                transform.position = new Vector3(-max_x_distance, transform.position.y, transform.position.z);
            }

            if (Input.GetKey(KeyCode.DownArrow) && 90f - transform.eulerAngles.z > min_angle)
            {
                transform.Rotate(Vector3.forward, bow_aiming_speed * Time.deltaTime, Space.Self);
            }
            else if (Input.GetKey(KeyCode.UpArrow) && 90f - transform.eulerAngles.z < max_angle)
            {
                transform.Rotate(Vector3.forward, -bow_aiming_speed * Time.deltaTime, Space.Self);
            }
            
            
            starting_position = transform.position + offset.x * transform.forward + offset.y * transform.right + offset.z * transform.up;
            if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space)) && arrow != null && ready)
            {
                if (speed_percent >= 1f)
                {
                    flipped = true;
                }
                else if (speed_percent <= 0f)
                {
                    flipped = false;
                }
                if (flipped)
                {
                    speed_percent = Mathf.Max(speed_percent - pull_rate * Time.deltaTime, 0f);
                }
                else
                {
                    speed_percent = Mathf.Min(speed_percent + pull_rate * Time.deltaTime, 1f);
                }
                is_pulling = true;
                speed_image.fillAmount = speed_percent;
                speed_image.color = speed_gradient.Evaluate(speed_percent);
                arrow.transform.position = starting_position - speed_percent * arrow.transform.forward * pulling_max_distance;
            }
        }

        if ((Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Space)) && arrow != null && ready && is_pulling && server.keyboard_on)
        {
            FireArrow();
        }

        if (fired)
        {
            if (arrow_reload_timer < arrow_reload_time)
            {
                arrow_reload_timer += Time.deltaTime;
            }
            else
            {
                ready = false;
                GameObject new_instance = GameObject.Instantiate(arrow_prefab, starting_position, Quaternion.identity);
                arrow = new_instance.GetComponent<Arrow>();
                arrow.transform.SetParent(this.transform);
                arrow.transform.position = starting_position;
                arrow.transform.forward = transform.up;
                UpdateArrow(arrow);
                fired = false;
                arrow_reload_timer = 0f;
            }
        }

        if (has_special_attack && !used_special_attack && game_manager.in_wave && server.keyboard_on)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                using_special_attack = true;
                aiming_sign.SetActive(true);
            }
        }

        if (has_special_attack && game_manager.in_wave && using_special_attack && server.keyboard_on)
        {
            used_special_attack = true;
            if (special_attack_timer < special_attack_duration)
            {
                Vector3 direction = Vector3.zero;
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    direction += Vector3.forward;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    direction += Vector3.back;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    direction += Vector3.right;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    direction += Vector3.left;
                }
                direction = direction.normalized;
                if (direction != Vector3.zero)
                {
                    special_shooting_Transform.position += direction * special_speed * Time.deltaTime;
                }
                if (special_shooting_Transform.position.x < horizontal_limits.x)
                {
                    special_shooting_Transform.position = new Vector3(horizontal_limits.x, special_shooting_Transform.position.y, special_shooting_Transform.position.z);
                }
                else if (special_shooting_Transform.position.x > horizontal_limits.y)
                {
                    special_shooting_Transform.position = new Vector3(horizontal_limits.y, special_shooting_Transform.position.y, special_shooting_Transform.position.z);

                }
                if (special_shooting_Transform.position.z < vertical_limits.x)
                {
                    special_shooting_Transform.position = new Vector3(special_shooting_Transform.position.x, special_shooting_Transform.position.y, vertical_limits.x);

                }
                else if (special_shooting_Transform.position.z > vertical_limits.y)
                {
                    special_shooting_Transform.position = new Vector3(special_shooting_Transform.position.x, special_shooting_Transform.position.y, vertical_limits.y);
                }
                aiming_sign.transform.position = new Vector3(special_shooting_Transform.position.x, aiming_sign.transform.position.y, special_shooting_Transform.position.z);
                special_attack_timer += Time.deltaTime;
                if (special_between_timer  < time_between_specail_arrows)
                {
                    special_between_timer += Time.deltaTime;
                }
                else
                {
                    special_between_timer = 0f;
                    GameObject new_special_arrow = Instantiate(arrow_prefab, special_shooting_Transform.position + special_offset, Quaternion.identity);
                    UpdateArrow(new_special_arrow.GetComponent<Arrow>());
                    new_special_arrow.transform.GetComponent<Arrow>().FireArrow(Vector3.Normalize(special_forward_factor * Vector3.forward), 1f);
                    game_manager.arrows.Add(new_special_arrow);
                }
            }
            else
            {
                aiming_sign.transform.position = new Vector3(special_starting_shooting_position.x, aiming_sign.transform.position.y, special_starting_shooting_position.z);
                aiming_sign.SetActive(false);
                special_attack_timer = 0f;
                special_between_timer = 0f;
                used_special_attack = true;
                using_special_attack = false;
                special_shooting_Transform.position = special_starting_shooting_position;
            }
        }
    }

    public void FireArrow()
    {
        is_pulling = false;
        fired = true;
        arrow.transform.SetParent(null);
        game_manager.arrows.Add(arrow.gameObject);
        arrow.FireArrow(transform.up, speed_percent);
        audio_manager.Play("shoot", transform.position);
        speed_percent = 0f;
        speed_image.fillAmount = 0f;
        arrow = null;
    }
    public void UpdateArrow(Arrow arrow)
    {
        if(arrow != null)
        {
            arrow.damage = damage;
            arrow.knockback = knockback;
            if (big)
            {
                arrow.transform.localScale = big_arrow_scale;
            }
            arrow.max_bounce = max_bounce;
            arrow.max_piercing = max_piercing;
            arrow.explosive = explosive;
            arrow.healing = healing;
            arrow.critical = critical;
        }
    }

    public void ResetBow()
    {
        special_attack_timer = 0f;
        special_between_timer = 0f;
        used_special_attack = false;
        using_special_attack = false;
        aiming_sign.SetActive(false);
    }

}
