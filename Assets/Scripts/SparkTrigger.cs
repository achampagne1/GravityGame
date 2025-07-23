using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkTrigger : MonoBehaviour
{
    [SerializeField] ParticleSystem sparkBurst;
    [SerializeField] AudioClip audioClip;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void triggerSparks()
    {
        audioSource = GetComponent<AudioSource>();
        sparkBurst.Play();
        audioSource.PlayOneShot(audioClip, 0.2f);
    }
}
