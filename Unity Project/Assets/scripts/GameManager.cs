using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private AudioManager audio_manager;
    private bool dead;
    [HideInInspector] public List<GameObject> arrows = new List<GameObject>();
    [HideInInspector] public List<GameObject> enemies_for_destruction = new List<GameObject>();
    private Transform camera_transform;
    public Bow bow;
    [Header("Waves")]
    public List<GameObject> enemies_types = new List<GameObject>();
    public List<Wave> waves = new List<Wave>();
    private Wave current_wave;
    private int current_wave_index;
    [HideInInspector] public bool in_wave;
    private float wave_timer;
    private int current_enemy_index;
    public Transform spowning_transform;
    public float spowning_range;
    [Header("Blessings")]
    public List<Blessing> blessings = new List<Blessing>();
    public Canvas canvas;
    public Image blessings_panel;
    public GameObject pinter1;
    public GameObject pinter2;
    public GameObject pinter3;
    public Button button1;
    public Button button2;
    public Button button3;
    private int index1;
    private int index2;
    private int index3;
    [Header("Health Bar")]
    public Image health_bar;
    public TextMeshProUGUI health_text;
    public int max_health;
    private int health_amount;
    public int healing_amount;
    public Image hurt_panel;
    [Header("stats")]
    public TextMeshProUGUI wave_text;
    public TextMeshProUGUI arrow_damage_text;
    public TextMeshProUGUI explosion_damage_text;
    public GameObject explosion_damage_image;
    [Header("Lose")]
    public Image lose_screen;
    [Header("Editing mode")]
    public GameObject edited_spikes;
    public GameObject spikes;
    [HideInInspector] public bool editing;
    [HideInInspector] public bool started_editing;
    public Vector3 editing_position;
    [HideInInspector] public GameObject edited_instance;
    public float editing_speed;
    public Vector2 vertical_limits;
    public Vector2 horizontal_limits;
    [Header("Realsense")]
    public RawImage realsense_image;
    public Image border;
    [Header("Control")]
    public GameObject keyboard_button;
    public GameObject camera_button;
    private Server server;
    public Button hand_button;
    public Sprite left_hand_sprite;
    public Sprite right_hand_sprite;
    void Start()
    {
        audio_manager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<AudioManager>();
        wave_text.text = (current_wave_index + 1).ToString();
        arrow_damage_text.text = bow.damage.ToString();
        camera_transform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        health_amount = max_health;
        health_text.text = health_amount.ToString();
        current_wave = waves[0];
        foreach (Wave wave in waves)
        {
            wave.UpdateWave();
        }
        StartWave();

        server = GameObject.FindGameObjectWithTag("server").GetComponent<Server>();
        if (server.keyboard_on)
        {
            keyboard_button.SetActive(false);
            camera_button.SetActive(true);
        }
        if (server.image_mode == 2)
        {
            realsense_image.enabled = false;
            border.enabled = false;
        }
        if (server.left_handed)
        {
            hand_button.image.sprite = left_hand_sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (in_wave)
        {
            bool entered_loop = false;
            while (wave_timer <= 0 || !entered_loop)
            {
                if (wave_timer < 0)
                {
                    if (current_enemy_index < waves[current_wave_index].enemies_list.Count)
                    {
                        GameObject new_enemy = GameObject.Instantiate(enemies_types[waves[current_wave_index].enemies_list[current_enemy_index].key], spowning_transform.position + Vector3.right * Random.Range(-spowning_range, spowning_range), Quaternion.identity);
                        enemies_for_destruction.Add(new_enemy);
                        if (current_enemy_index + 1 < waves[current_wave_index].enemies_list.Count)
                        {
                            wave_timer = waves[current_wave_index].enemies_list[current_enemy_index + 1].time;
                        }
                    }
                    else
                    {
                        wave_timer = Mathf.Max(wave_timer, 0.01f);
                    }
                    current_enemy_index += 1;
                }
                else
                {
                    wave_timer -= Time.deltaTime;
                }
                entered_loop = true;
            }
        }
        else if (editing)
        {
            started_editing = true;
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
                edited_instance.transform.position += direction * editing_speed * Time.deltaTime;
            }
            if (edited_instance.transform.position.x < horizontal_limits.x)
            {
                edited_instance.transform.position = new Vector3(horizontal_limits.x, edited_instance.transform.position.y, edited_instance.transform.position.z);
            }
            else if (edited_instance.transform.position.x > horizontal_limits.y)
            {
                edited_instance.transform.position = new Vector3(horizontal_limits.y, edited_instance.transform.position.y, edited_instance.transform.position.z);

            }
            if (edited_instance.transform.position.z < vertical_limits.x)
            {
                edited_instance.transform.position = new Vector3(edited_instance.transform.position.x, edited_instance.transform.position.y, vertical_limits.x);

            }
            else if (edited_instance.transform.position.z > vertical_limits.y)
            {
                edited_instance.transform.position = new Vector3(edited_instance.transform.position.x, edited_instance.transform.position.y, vertical_limits.y);
            }
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(spikes, edited_instance.transform.position, Quaternion.identity);
                Destroy(edited_instance);
                editing = false;
                started_editing = false;
                StartWave();
            }
        }
    }
    public void TakeDamage(int damage)
    {
        if (!dead)
        {
            health_amount = Mathf.Max(health_amount - damage, 0);
            health_text.text = health_amount.ToString();
            health_bar.fillAmount = health_amount / (float)max_health;
            hurt_panel.GetComponent<Animator>().SetTrigger("hurt");
            camera_transform.GetComponent<Animator>().SetTrigger("shake");
            if (health_bar.fillAmount <= 0)
            {
                dead = true;
                lose_screen.GetComponent<Animator>().SetTrigger("lose");
                audio_manager.Play("lose", transform.position);
                Invoke("Clean", 2f);
                Invoke("RestartGame", 10f);
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Heal()
    {
        health_amount = Mathf.Min(health_amount + healing_amount, max_health);
        health_text.text = health_amount.ToString();
        health_bar.fillAmount = health_amount / (float)max_health;
    }

    public void StartWave()
    {
        wave_timer = waves[current_wave_index].enemies_list[0].time;
        in_wave = true;
    }

    public void EndWave()
    {
        if (!dead)
        {
            audio_manager.Play("won wave", transform.position);
            current_enemy_index = 0;
            if (current_wave_index < waves.Count - 1)
            {
                current_wave_index += 1;
                wave_text.text = (current_wave_index + 1).ToString();
                if (current_wave_index < waves.Count)
                {
                    current_wave = waves[current_wave_index];
                }
                in_wave = false;
                Clean();
                SetBlessings();
                bow.ResetBow();
            }
            else
            {
                in_wave = false;
                lose_screen.GetComponent<Animator>().SetTrigger("win");
                Invoke("RestartGame", 10f);
            }
        }
    }

    public void Clean()
    {
        foreach (GameObject enemy in enemies_for_destruction)
        {
            Destroy(enemy);
        }
        enemies_for_destruction.Clear();
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();
    }
    public void SetBlessings()
    {
        if (blessings.Count == 3)
        {
            index1 = 0;
            index2 = 1;
            index3 = 2;
        }
        else
        {
            index1 = Random.Range(0, blessings.Count);
            index2 = Random.Range(0, blessings.Count);
            if (index1 == index2)
            {
                index2 = (index1 + 1) % blessings.Count;
            }
            index3 = Random.Range(0, blessings.Count);
            if (index3 == index1 || index3 == index2)
            {
                if (index2 == (index1 + 1) % blessings.Count)
                {
                    index3 = (index1 + 2) % blessings.Count;
                }
                else
                {
                    index3 = (index1 + 1) % blessings.Count;
                }
            }
        }
        button1.image.sprite = blessings[index1].sprite;
        button2.image.sprite = blessings[index2].sprite;
        button3.image.sprite = blessings[index3].sprite;
        blessings_panel.GetComponent<Animator>().SetBool("show blessings", true);
    }

    public void ChoseBlessing(int index)
    {
        if (!in_wave && !editing)
        {
            string name = blessings[index].name;
            if (name == "piercing")
            {
                bow.max_piercing += 1;
            }
            else if (name == "bouncy")
            {
                bow.max_bounce += 1;
            }
            else if (name == "faster")
            {
                bow.arrow_reload_time = bow.arrow_fast_reload_time;
            }
            else if (name == "powerful")
            {
                bow.damage += bow.extra_damage;
                arrow_damage_text.text = (bow.damage).ToString();
                explosion_damage_text.text = (bow.damage / 2).ToString();
            }
            else if (name == "explosive")
            {
                bow.explosive = true;
                explosion_damage_image.SetActive(true);
                explosion_damage_text.text = (bow.damage / 2).ToString();
            }
            else if (name == "knockback")
            {
                bow.knockback += bow.extra_knockback;
            }
            else if (name == "bigger")
            {
                bow.big = true;
            }
            else if (name == "critical")
            {
                bow.critical = true;
            }
            else if (name == "heal")
            {
                bow.healing = true;
            }
            else if (name == "cut")
            {
                health_amount = health_amount / 2;
                health_text.text = health_amount.ToString();
                health_bar.fillAmount = health_amount / (float)max_health;
                if (index == index1)
                {
                    ChoseBlessing(index2);
                    ChoseBlessing(index3);
                }
                else if (index == index2)
                {
                    ChoseBlessing(index1);
                    ChoseBlessing(index3);
                }
                else if (index == index3)
                {
                    ChoseBlessing(index1);
                    ChoseBlessing(index2);
                }
            }
            else if (name == "spikes")
            {
                editing = true;
            }
            else if (name == "special")
            {
                bow.has_special_attack = true;
            }
            blessings.RemoveAt(index);
            if (index1 > index)
            {
                index1--;
            }
            if (index2 > index)
            {
                index2--;
            }
            if (index3 > index)
            {
                index3--;
            }
        }
    }
    public void ChoseBlessing1()
    {
        ChoseBlessing(index1);
        EndChosing();
    }
    public void ChoseBlessing2()
    {
        ChoseBlessing(index2);
        EndChosing();
    }

    public void ChoseBlessing3()
    {
        ChoseBlessing(index3);
        EndChosing();
    }

    public void EndChosing()
    {
        if (!started_editing)
        {
            blessings_panel.GetComponent<Animator>().SetBool("show blessings", false);
            bow.UpdateArrow(bow.arrow);
            if (!editing)
            {
                StartWave();
            }
            else
            {
                edited_instance = Instantiate(edited_spikes, editing_position, Quaternion.identity);
            }
        }
    }

    public void CountEnemyAsDead()
    {
        current_wave.KillEnemy();
        if (current_wave.GetRemainingAmount() <= 0)
        {
            Invoke("EndWave", 2f);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeImageMode()
    {
        server.image_mode = (server.image_mode + 1) % 3;
        if (server.image_mode == 0)
        {
            realsense_image.enabled = true;
            border.enabled = true;
        }
        else if (server.image_mode == 2)
        {
            realsense_image.enabled = false;
            border.enabled = false;
        }
    }

    public void UseKeyboard()
    {
        server.keyboard_on = true;
        keyboard_button.SetActive(false);
        camera_button.SetActive(true);
    }

    public void UseCamera()
    {
        server.keyboard_on = false;
        keyboard_button.SetActive(true);
        camera_button.SetActive(false);
    }
    public void ChangeHands()
    {
        if (server.left_handed)
        {
            server.left_handed = false;
            hand_button.image.sprite = right_hand_sprite;
        }
        else
        {
            server.left_handed = true;
            hand_button.image.sprite = left_hand_sprite;
        }
    }

    [System.Serializable]
    public class Wave {
        public List<EnemyData> enemies_list;
        private int enemies_amount;
        private int remaining_amount;
        public void UpdateWave()
        {
            enemies_amount = enemies_list.Count;
            remaining_amount = enemies_amount;
        }

        public void KillEnemy()
        {
            remaining_amount -= 1;
        }

        public int GetRemainingAmount()
        {
            return remaining_amount;
        }
    }

    [System.Serializable]
    public class EnemyData{
        public float time;
        public int key;
    }

    [System.Serializable]
    public class Blessing
    {
        public string name;
        public Sprite sprite;
    }
}
