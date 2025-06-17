using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : CharacterController
{
    [SerializeField] float persistanceAfterDeath = 5f;
    [SerializeField] float jumpAngle = 20f;
    [SerializeField] float jumpMagnitude = 10f;
    [SerializeField] float pounceCooldownTime = 5f;
    private bool pounceRunning = false;
    bool latch = true;

    // Start is called before the first frame update
    void Start()
    {
        calculateCharacterStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //BIG NOTE: the bug sprtie it facing left generically so we need the inverse of facing left to get the actual facing left
        /**if(EnemyAssistant.detectPlayer(!facingLeft, gameObject)!=new Vector3(0f,0f,1f)&&!pounceRunning)
            StartCoroutine(pounce());
        else
        {
            setMovement(1);
            setOrientation(-1);
        }**/
        setMovement(1);
        setOrientation(1);
        calculateCharacterUpdate();
    }

    private IEnumerator pounce()
    {
        pounceRunning = true;
        setMovement(0);
        forceLocalAdded = true;
        // Step 1: Your intended direction (local jump angle)
        Vector2 localDir = HelperFunctions.angleToDirection(jumpAngle);

        // Step 2: Get the GameObject's rotation angle (in degrees)
        float zRotation = transform.eulerAngles.z;

        // Step 3: Create a quaternion that rotates around Z (2D)
        Quaternion rotation = Quaternion.Euler(0, 0, zRotation);

        // Step 4: Rotate the direction vector
        Vector2 worldDir = rotation * localDir;

        // Step 5: Scale by magnitude
        Vector2 finalJump = worldDir * jumpMagnitude;
        forceLocal = finalJump;
        yield return new WaitForSeconds(pounceCooldownTime);
        pounceRunning = false;
        forceLocalAdded = false;
        yield return null;
    }

    protected override IEnumerator die()
    {
        setMovement(0);
        yield return base.die();
        yield return new WaitForSeconds(persistanceAfterDeath);
        Destroy(gameObject);
    }
}
