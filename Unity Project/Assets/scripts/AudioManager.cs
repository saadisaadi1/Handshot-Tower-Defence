using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{ 

	[SerializeField]
	private Sound[] pool;         // A pool with a bunch of sounds and settings for said sounds
	private GameObject song;
	private Button music_button;
	private Button sound_button;
	public Sprite music_on_sprite;
	public Sprite music_off_sprite;
	public Sprite sound_on_sprite;
	public Sprite sound_off_sprite;
	private Server server;

    public void Start()
    {
		song = GameObject.FindGameObjectWithTag("song");
		sound_button = GameObject.FindGameObjectWithTag("sound button").GetComponent<Button>();
		music_button = GameObject.FindGameObjectWithTag("music button").GetComponent<Button>();
		server = GameObject.FindGameObjectWithTag("server").GetComponent<Server>();
        if (!server.sound_on)
        {
			sound_button.image.sprite = sound_off_sprite;
		}
        if (!server.music_on)
        {
			music_button.image.sprite = music_off_sprite;
			song.GetComponent<AudioSource>().mute = true;
		}
	}
    // Play a sound from the pool
    public void Play(string name, Vector3 position)
	{
        if (server.sound_on)
        {
			Sound sound = Array.Find(pool, element => element.name == name);
			if (sound == null)
			{
				Debug.LogWarning("Sound: '" + name + "' couldn't be played because it wasn't found.");
				return;
			}

			GameObject new_sound = Instantiate(sound.prefab, position, Quaternion.identity);
			if (!sound.remain)
			{
				StartCoroutine(DestructionCoroutine(sound.duration, new_sound));
			}
		}
	}
	public IEnumerator DestructionCoroutine(float duration, GameObject sound)
    {
		yield return new WaitForSeconds(duration);
		sound.GetComponent<DestroyScript>().DestroySound();
    }
	
	public void turn_sound()
    {
        if (server.sound_on)
        {
			server.sound_on = false;
			sound_button.image.sprite = sound_off_sprite;
        }
        else
        {
			server.sound_on = true;
			sound_button.image.sprite = sound_on_sprite;
		}
    }

	public void turn_music()
	{
		if (server.music_on)
		{
			server.music_on = false;
			music_button.image.sprite = music_off_sprite;
			song.GetComponent<AudioSource>().mute = true;
		}
		else
		{
			server.music_on = true;
			music_button.image.sprite = music_on_sprite;
			song.GetComponent<AudioSource>().mute = false;
		}
	}
}
