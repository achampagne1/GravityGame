using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using System.Xml;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private Dictionary<EntityId, AudioSource> loopingSourceId = new Dictionary<EntityId, AudioSource>();
    private Transform center;

    private void Awake()
    {
        if (instance == null)
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

    public void playSound(AudioClip sound, Transform location, float volume)
    {
        AudioSource soundFx = soundBasics(sound, location, volume);
        float clipLength = soundFx.clip.length;
        soundFx.gameObject.GetEntityId();
        Destroy(soundFx.gameObject, clipLength);
    }

    public void playLoop(AudioClip sound, GameObject spawningObject, float volume)
    {
        AudioSource soundFx = soundBasics(sound, spawningObject.transform, volume);
        soundFx.loop = true;
        EntityId entityId = spawningObject.GetEntityId();
        loopingSourceId[entityId] = soundFx;
    }

    public void stopLoop(GameObject spawningObject)
    {
        EntityId entityId = spawningObject.GetEntityId();
        Destroy(loopingSourceId[entityId]);
    }

    private AudioSource soundBasics(AudioClip sound, Transform location, float volume)
    {
        AudioSource soundFx = Instantiate(new AudioSource(), location.position, Quaternion.identity);
        soundFx.transform.parent = transform;
        soundFx.clip = sound;
        float magnitude = (location.position - center.position).magnitude;
        soundFx.volume = volume * (1 / magnitude);
        soundFx.loop = true;
        soundFx.Play();
        return soundFx;
    }
}
