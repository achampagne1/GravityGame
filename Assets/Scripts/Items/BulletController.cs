using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletController : ObjectController,IProjectile
{

    //game variables
    private int shotBy = 0;
    private float drag = .1f;
    [SerializeField] float damage = 1f;

    //objects
    ProjectileHelper projectileHelper;

    // Start is called before the first frame update
    public void Start()
    {
        simulated = true;
        updateGravityField = true;
        calculateStart();
    }

    public void init(int shotBy)
    {
        projectileHelper = new ProjectileHelper(shotBy, damage, gameObject);
    }

    // Update is called once per frame
    public void Update()
    {
        calculateRotation();
        calculateUpdate();
        rb.velocity = calculateDrag(rb.velocity);  //drag prevent bullets from infinitly orbiting
    }

    protected override void calculateRotation()
    {
        // Create a quaternion representing the desired rotation angle around the y-axis
        // bullet rotation is slightly different from other object rotations. it must take into account its velocity
        // due to this, the calculateRotation() parent function is overidden
        float angle = Mathf.Atan2(rb.velocity.y+gravityDirection.y, rb.velocity.x+gravityDirection.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.Euler(0f, 0f,angle);
        transform.rotation = desiredRotation;
    }

    Vector2 calculateDrag(Vector2 input)
    {
        float magnitude = input.magnitude;
        Vector2 unitVector = input.normalized;
        magnitude -= drag;
        return (magnitude * unitVector);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        projectileHelper.OnCollisionEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        projectileHelper.OnTriggerEnter2D(trigger);
        //maybe have it so if the player and shield colliders are hit, that that doesnt count as a hit
    }

    public int getShotBy()
    {
        return projectileHelper.getShotBy();
    }

    public float getDamage()
    {
        return projectileHelper.getDamage();
    }

}
