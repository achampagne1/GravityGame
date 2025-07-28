using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] AudioSource audioSource;
    private Transform center;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        StartCoroutine(checkForNewCenter());
    }

    private IEnumerator checkForNewCenter()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain != null && brain.ActiveVirtualCamera != null)
            {
                center = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
            }
        }
    }

    public void playSound(AudioClip sound,Transform location,float volume)
    {
        AudioSource soundFx = Instantiate(audioSource, location.position, Quaternion.identity);
        soundFx.clip = sound;
        float magnitude = (location.position - center.position).magnitude;
        soundFx.volume = volume*(1/magnitude);
        soundFx.Play();

        float clipLength = soundFx.clip.length;

        Destroy(soundFx.gameObject,clipLength);
    }
}
