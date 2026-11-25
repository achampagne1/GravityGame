using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField] ParticleSystem hitMark;
    [SerializeField] float dirtThrowThreshold = 5f;
    [SerializeField] List<AudioClip> footsteps = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterController character = collision.gameObject.GetComponent<CharacterController>();
        if (character != null)
        {
            character.setMaterialFootsteps(footsteps);
        }
    }
}
