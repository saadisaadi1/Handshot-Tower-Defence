using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    public RawImage rawImage;
    Texture2D texture;
    private Bow bow;
    private GameManager game_manager;
    private CameraScript camera_script;
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    List<int> data_list = new List<int>();
    byte[] image_buffer;
    private static bool instanceExists = false;
    private bool take_data;
    [HideInInspector] public bool music_on = true;
    [HideInInspector] public bool sound_on = true;
    [HideInInspector] public bool keyboard_on;
    [HideInInspector] public int image_mode;
    [HideInInspector] public bool left_handed = false;
    private float blessing_timer = 0f;

    private void Awake()
    {
        if (instanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Start()
    {
        //bow = transform.GetComponent<Bow>();
        bow = GameObject.FindGameObjectWithTag("bow").GetComponent<Bow>();
        camera_script = GameObject.FindGameObjectWithTag("camera").GetComponent<CameraScript>();
        game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
        if(mThread != null)
        {
            mThread.Abort();
        }
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }
    private void Update()
    {
        if (bow == null || camera_script == null || game_manager == null || rawImage == null)
        {
            bow = GameObject.FindGameObjectWithTag("bow").GetComponent<Bow>();
            camera_script = GameObject.FindGameObjectWithTag("camera").GetComponent<CameraScript>();
            game_manager = GameObject.FindGameObjectWithTag("game manager").GetComponent<GameManager>();
            rawImage = GameObject.FindGameObjectWithTag("image").GetComponent<RawImage>();
        }
        bool is_new_data = true;
        if (client != null && client.Connected && client.Available > 0)
        {
            SendAndReceiveData();
        }
        else
        {
            is_new_data = false;
        }
        if (data_list.Count > 0 && !keyboard_on)
        {
            if (left_handed && is_new_data)
            {
                if(data_list[1] != 2000f)
                {
                    data_list[1] = -data_list[1];
                }
                int temp = data_list[2];
                data_list[2] = data_list[3];
                data_list[3] = temp;
                temp = data_list[0];
                data_list[0] = data_list[5];
                data_list[5] = temp;
            }
            bow.starting_position = bow.transform.position + bow.offset.x * bow.transform.forward + bow.offset.y * bow.transform.right + bow.offset.z * bow.transform.up;
            if (bow.has_special_attack && !bow.used_special_attack && game_manager.in_wave && data_list[2] == 4 && data_list[3] == 4)
            {
                bow.using_special_attack = true;
                bow.aiming_sign.SetActive(true);
            }
            if (!game_manager.editing && game_manager.in_wave && !bow.using_special_attack)
            {
                if (data_list[0] > -40f)
                {
                    bow.transform.eulerAngles = new Vector3(bow.transform.eulerAngles.x, bow.transform.eulerAngles.y, 90f - data_list[0]);
                }
                if (data_list[4] >= -1000)
                {
                    bow.transform.position = new Vector3(data_list[4] / 1000f * bow.max_x_distance, bow.transform.position.y, bow.transform.position.z);
                    camera_script.transform.position = new Vector3(bow.transform.position.x - camera_script.offset.x, camera_script.transform.position.y, camera_script.transform.position.z);
                }
                if (data_list[2] == 0 && data_list[3] == 0 && bow.ready && bow.arrow != null && bow.ready)
                {
                    bow.is_pulling = true;
                }
                if (bow.is_pulling && bow.arrow != null && data_list[1] != 2000 && data_list[3] == 0)
                {
                    bow.speed_percent = Mathf.Max(0f, data_list[1]) / 1000f;
                    bow.speed_image.fillAmount = bow.speed_percent;
                    bow.speed_image.color = bow.speed_gradient.Evaluate(bow.speed_percent);
                    bow.starting_position = bow.transform.position + bow.offset.x * bow.transform.forward + bow.offset.y * bow.transform.right + bow.offset.z * bow.transform.up;
                    bow.arrow.transform.position = bow.starting_position - bow.speed_percent * bow.arrow.transform.forward * bow.pulling_max_distance;
                }
                if (data_list[2] == 5 && bow.arrow != null && bow.ready && bow.is_pulling)
                {
                    bow.FireArrow();
                }
            }
            else if (bow.has_special_attack && game_manager.in_wave && bow.using_special_attack)
            {

                bow.used_special_attack = true;
                if (bow.special_attack_timer < bow.special_attack_duration * 2f)
                {
                    float x_precent = ((data_list[4] / 1000f) + 1)/2;
                    float z_precent = Mathf.Max(0f, -data_list[1]) / 1000f;
                    float x_position = Mathf.Lerp(game_manager.horizontal_limits.x, game_manager.horizontal_limits.y, x_precent);
                    float z_position = Mathf.Lerp(game_manager.vertical_limits.x, game_manager.vertical_limits.y, z_precent);
                    bow.special_shooting_Transform.position = new Vector3(x_position, bow.special_shooting_Transform.position.y, z_position);
                    bow.aiming_sign.transform.position = new Vector3(x_position, bow.aiming_sign.transform.position.y, z_position);
                    bow.special_attack_timer += Time.deltaTime;
                    if (bow.special_between_timer < bow.time_between_specail_arrows)
                    {
                        bow.special_between_timer += Time.deltaTime;
                    }
                    else
                    {
                        bow.special_between_timer = 0f;
                        GameObject new_special_arrow = Instantiate(bow.arrow_prefab, bow.special_shooting_Transform.position + bow.special_offset, Quaternion.identity);
                        bow.UpdateArrow(new_special_arrow.GetComponent<Arrow>());
                        new_special_arrow.transform.GetComponent<Arrow>().FireArrow(Vector3.Normalize(bow.special_forward_factor * Vector3.forward), 1f);
                        game_manager.arrows.Add(new_special_arrow);
                    }
                }
                else
                {
                    bow.aiming_sign.transform.position = new Vector3(bow.special_starting_shooting_position.x, bow.aiming_sign.transform.position.y, bow.special_starting_shooting_position.z);
                    bow.aiming_sign.SetActive(false);
                    bow.special_attack_timer = 0f;
                    bow.special_between_timer = 0f;
                    bow.used_special_attack = true;
                    bow.using_special_attack = false;
                    bow.special_shooting_Transform.position = bow.special_starting_shooting_position;
                }
            }
            else if (!game_manager.in_wave && !game_manager.editing)
            {
                if (blessing_timer < 1f)
                {
                    blessing_timer += Time.deltaTime;
                }
                else
                {
                    Button chosen_button = null;
                    if (data_list[0] < 50f && data_list[0] > 20f)
                    {
                        chosen_button = game_manager.button1;
                        game_manager.pinter1.SetActive(true);
                        game_manager.pinter2.SetActive(false);
                        game_manager.pinter3.SetActive(false);
                    }
                    else if (data_list[0] < 20f && data_list[0] > -10f)
                    {
                        chosen_button = game_manager.button2;
                        game_manager.pinter1.SetActive(false);
                        game_manager.pinter2.SetActive(true);
                        game_manager.pinter3.SetActive(false);
                    }
                    else if (data_list[0] < -10f && data_list[0] > -30f)
                    {
                        chosen_button = game_manager.button3;
                        game_manager.pinter1.SetActive(false);
                        game_manager.pinter2.SetActive(false);
                        game_manager.pinter3.SetActive(true);
                    }
                    else
                    {
                        chosen_button = null;
                        game_manager.pinter1.SetActive(false);
                        game_manager.pinter2.SetActive(false);
                        game_manager.pinter3.SetActive(false);
                    }

                    if (data_list[1] <= 0f && !game_manager.editing && chosen_button != null)
                    {
                        chosen_button.onClick.Invoke();
                        game_manager.pinter1.SetActive(false);
                        game_manager.pinter2.SetActive(false);
                        game_manager.pinter3.SetActive(false);
                        blessing_timer = 0f;
                    }
                }
            }
            else if (game_manager.editing)
            {
                game_manager.started_editing = true;
                float x_precent = ((data_list[4] / 1000f) + 1) / 2;
                float z_precent = Mathf.Max(0f, -data_list[1]) / 1000f;
                game_manager.edited_instance.transform.position = new Vector3(Mathf.Lerp(game_manager.horizontal_limits.x, game_manager.horizontal_limits.y, x_precent), game_manager.edited_instance.transform.position.y, Mathf.Lerp(game_manager.vertical_limits.x, game_manager.vertical_limits.y, z_precent));
                if (data_list[2] == 1 && data_list[3] == 0)
                {
                    Instantiate(game_manager.spikes, game_manager.edited_instance.transform.position, Quaternion.identity);
                    Destroy(game_manager.edited_instance);
                    game_manager.editing = false;
                    game_manager.started_editing = false;
                    game_manager.StartWave();
                }
            }
        }
        if (image_buffer != null && image_mode != 2)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
            texture = new Texture2D(640, 480);
            texture.LoadImage(image_buffer);
            texture.Apply();
            rawImage.texture = texture;
        }
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();
        while (true)
        {
            client = listener.AcceptTcpClient();
        }
    }

    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string
        //Debug.Log(dataReceived);
        byte[] myWriteBuffer;
        if (dataReceived != null)
        {
            //---Using received data---
            data_list = StringToList(dataReceived); //<-- assigning receivedPos value from Python
            //---Sending Data to Host----
        }

        string image_mode_string = "";
        if (image_mode == 0)
        {
            image_mode_string = "rgb";
        }
        else if (image_mode == 1)
        {
            image_mode_string = "depth";
        }
        else if (image_mode == 2)
        {
            image_mode_string = "no image";
        }
        myWriteBuffer = Encoding.ASCII.GetBytes(image_mode_string); //Converting string to byte data
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python

        if(image_mode_string != "no image")
        {
            buffer = new byte[1];
            bytesRead = nwStream.Read(buffer, 0, 1); //Getting data in Bytes from Python
            int size_size = int.Parse(Encoding.UTF8.GetString(buffer, 0, bytesRead)); //Converting byte data to string
            //Debug.Log("size_size = " + size_size.ToString());
            buffer = new byte[size_size];
            bytesRead = nwStream.Read(buffer, 0, size_size); //Getting data in Bytes from Python
            string image_size_string = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string
            //Debug.Log("the size length = " + bytesRead.ToString() + " the size = " + image_size_string);
            string size_string = image_size_string;
            int imageSize = int.Parse(image_size_string);
            image_buffer = new byte[imageSize];
            bytesRead = 0;
            while (bytesRead < imageSize)
            {
                int chunkSize = nwStream.Read(image_buffer, bytesRead, imageSize - bytesRead);

                if (chunkSize == 0)
                {
                    // Connection closed by the client
                    break;
                }
                bytesRead += chunkSize;
            }
            //Debug.Log("bytes read = " + bytesRead.ToString() + " size = " + size_string);
        }
        else
        {
            image_buffer = null;
        }
    }

    public static List<int> StringToList(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        List<int> result = new List<int>();
        // split the items
        string[] sArray = sVector.Split(',');
        // store as a Vector3
        for (int i = 0; i < sArray.Length; i++)
        {
            result.Add(int.Parse(sArray[i]));
        }
        return result;
    }
    /*
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    */


    void OnApplicationQuit()
    {
        // Ensure to stop and join the thread when the application is quitting
        if (mThread != null && mThread.IsAlive)
        {
            mThread.Abort(); // Abort the thread (not recommended in general)
            mThread.Join(); // Ensure the thread is joined before exiting the application
        }
        if (listener != null)
        {
            listener.Stop();
        }
    }
}
