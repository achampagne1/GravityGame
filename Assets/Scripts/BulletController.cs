using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletController : ObjectController
{
    //object creation
    CircleCollider2D circleColliderPlayer;
    CharacterController characterController;
    Timer timer;

    //public variables
    public float bulletForce = 50.0f;

    //game variables
    float drag = .1f;
    bool first = true;
    bool playerInvulnerable = true;

    //vectors
    Vector2 initialForce = new Vector2(0, 0);




    // Start is called before the first frame update
    public void Start()
    {
        calculateStart();
        timer = new Timer(.5f);
        timer.startTimer();
        Physics2D.IgnoreLayerCollision(9, 12, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
        rb.AddForce(initialForce*bulletForce, ForceMode2D.Impulse);
        GameObject temp = GameObject.Find("SpaceMan");
        characterController = temp.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!first)
        {
            if (timer.checkTimer()) //currently the player layer is 9 and the bullet layer is 12
                Physics2D.IgnoreLayerCollision(9, 12, false);

            calculateRotation();
            calculateUpdate();
            rb.velocity = calculateDrag(rb.velocity);
        }
        else //this seciton is for the original bullet
        {
            Vector3 offset = new Vector3(0, .3f, 0);
            offset.y = offset.y * (characterController.getFacingLeft() ? -1 : 1);
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
        if (!first)
            Destroy(this.gameObject);
    }

    public void newInstance(Vector2 direction)
    {
        initialForce = direction;
        first = false;
    }

}
