using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletController : ObjectController
{
    //object creation
    CircleCollider2D circleColliderPlayer;
    CharacterController characterController;
    Timer timer;
    GunController gunController;

    //public variables
    public float bulletForce = 50.0f;

    //game variables
    private float drag = .1f;
    private bool first = true;
    private int shotBy = 0;


    //vectors
    private Vector2 initialForce = new Vector2(0, 0);

    // Start is called before the first frame update
    public void Start()
    {
        calculateStart();
        timer = new Timer(.5f);
        timer.startTimer();
        Physics2D.IgnoreLayerCollision(9, 12, true);
        Physics2D.IgnoreLayerCollision(12, 13, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
        Physics2D.IgnoreLayerCollision(2, 12, true);
        Physics2D.IgnoreLayerCollision(11, 12, true);
        rb.AddForce(initialForce*bulletForce, ForceMode2D.Impulse);
        if(first)
            gunController = transform.parent.GetComponent<GunController>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (!first)
        {
            calculateRotation();
            calculateUpdate();
            rb.velocity = calculateDrag(rb.velocity);
        }
        else //this seciton is for the original bullet
        {
            Vector3 offset = new Vector3(0, .3f, 0);
            offset.y = offset.y * (gunController.getFacingLeft() ? -1 : 1);
            transform.position = transform.parent.position + transform.rotation * offset;
            transform.rotation = transform.parent.rotation;
        }
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
        if (collision.gameObject.layer != shotBy &&  !first)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.GetComponent<TriggerBoundaryCotroller>().getLayerConnectedTo() != shotBy && !first)
            Destroy(this.gameObject);
        //maybe have it so if the player and shield colliders are hit, that that doesnt count as a hit
    }

    public void newInstance(Vector2 direction)
    {
        initialForce = direction;
        first = false;
    }

    public void setShotBy(int shotBy)
    {
        this.shotBy = shotBy;
    }

    public int getShotBy()
    {
        return shotBy;
    }

}
