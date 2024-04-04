using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Bow bow;
    private float camera_move_speed;
    private GameManager game_manager;
    private Server server;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.FindGameObjectWithTag("server").GetComponent<Server>();
        game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
        camera_move_speed = bow.bow_move_speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!game_manager.editing && game_manager.in_wave && !bow.using_special_attack && server.keyboard_on)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position -= camera_move_speed * Vector3.right * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += camera_move_speed * Vector3.right * Time.deltaTime;
            }
            if (transform.position.x > bow.max_x_distance - offset.x)
            {
                transform.position = new Vector3(bow.max_x_distance - offset.x, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -bow.max_x_distance - offset.x)
            {
                transform.position = new Vector3(-bow.max_x_distance - offset.x, transform.position.y, transform.position.z);
            }
        }
    }
}
