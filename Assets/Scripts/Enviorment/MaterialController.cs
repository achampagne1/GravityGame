using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField] ParticleSystem hitMark;
    [SerializeField] float dirtThrowThreshold = 5f;
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
        if (collision.relativeVelocity.magnitude < dirtThrowThreshold)
            return;

        //TODO: use the collision code for the shield hit locaiton and explosiions
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 collisionPoint = contact.point;
        ParticleSystem hitMarkClone = Instantiate(hitMark, collisionPoint, Quaternion.identity);
        Quaternion rotationAdjuster = Quaternion.Euler(90f, -90f, 0f);
        hitMarkClone.transform.up = contact.normal;
        hitMarkClone.transform.rotation *= rotationAdjuster;
        hitMarkClone.Play();
        Destroy(hitMarkClone.gameObject, .1f);
    }
}
