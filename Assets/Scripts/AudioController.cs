using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    //object creation
    private AudioSource loopingAudioSource;
    private Animator animator;
    private string loopingSoundName;

    //private game variables

    // Start is called before the first frame update
    void Start()
    {
        loopingAudioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Walk")&&!animator.GetBool("Airborn"))
        {
            startLoopingSound();
        }
        else
        {
            stopLoopingSound();
        }
    }

    public void setLoopingSound(string loopingSoundName)
    {
        loopingSoundName=this.loopingSoundName;
    }

    public void startLoopingSound()
    {
        if (!loopingAudioSource.isPlaying)
            loopingAudioSource.Play();
    }

    public void stopLoopingSound()
    {
        if (loopingAudioSource.isPlaying)
            loopingAudioSource.Stop();
    }
}
