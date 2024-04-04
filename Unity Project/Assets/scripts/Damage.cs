using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private AudioManager audio_manager;
    public string type;
    public int damage;
    private GameManager game_manager;
    // Start is called before the first frame update
    void Start()
    {
        game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
        audio_manager = GameObject.FindGameObjectWithTag("audio manager").GetComponent<AudioManager>();
    }
    public void DealDamage()
    {
        if (type == "skeleton" || type == "pirate")
        {
            audio_manager.Play("sword", transform.position);
        }
        else if (type == "eye")
        {
            audio_manager.Play("eye hit", transform.position);
        }
        else if (type == "orc")
        {
            audio_manager.Play("heavy sword", transform.position);
        }
        game_manager.TakeDamage(damage);
    }
}
