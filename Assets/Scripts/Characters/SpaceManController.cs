using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceManController : SpacePersonController
{
    
    //game variables
    private bool enemyCollideFlag = false;
    private bool clickPressed = false;
    [SerializeField] float[] playArea = { 50, 50 }; //generic play area 
    [SerializeField] float cameraShift = -110f; //for some reason this corrects the camera shift when the camera is shifted 20
    [SerializeField] VCamController camController;
    [SerializeField] UIHandler uIHandler;

    //vectors
    Vector2 screenCenter;
    Vector2 direction;


    public void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 12, true); //for bullets. I know its a dumb placement but it needs to be somewhere with every level
        Physics2D.IgnoreLayerCollision(13, 12, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
        Physics2D.IgnoreLayerCollision(2, 12, true);
        Physics2D.IgnoreLayerCollision(11, 12, true);

        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        calculateSpacePersonStart();
        setMaxHealth(10f);
    }

    public void FixedUpdate()
    {
        float fuelBuffer = currentFuel;
        setMovement(inputSystemToGetAxis());
        setOrientation(lookLeftOrRight());
        setJump(Keyboard.current.spaceKey.isPressed);
        if (Keyboard.current.qKey.isPressed)
            handController.throwItem();
        calculateSpacePersonUpdate();
        if (fuelBuffer != currentFuel)
            UIHandler.instance.setFuelValue(currentFuel); //move latch to ui

        if (rb.velocity.magnitude > 15)
        {
            camController.setShakeContinuously(true);
            camController.setShake(true); //setshake is a latch and automatically goes back to false
            camController.setShakeMagnitude(rb.velocity.magnitude/40);
        }
        else
            camController.setShakeContinuously(false);

        UIHandler.instance.setHealthValue(health);
    }

    public override void Update()
    {
        Vector3 handDirection = mouseToDirection(Input.mousePosition, transform.rotation);
        handController.setInputDirection(handDirection);
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (handController.getHoldingObject() != null && handController.getHoldingObject().name == "Gun") //will need to be updated wehn more guns are added
                camController.setGunRecoil(handDirection);
            handController.useHand();
        }
    }

    private Vector3 mouseToDirection(Vector3 inputDirection, Quaternion playerRotation)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f+ (cameraShift * Screen.height));
        Vector2 direction = new Vector2(inputDirection.x, inputDirection.y) - screenCenter;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 direction3D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0f);
        Vector3 rotatedDirection = playerRotation * direction3D;
        return new Vector2(rotatedDirection.x, rotatedDirection.y).normalized;
    }

    //NOTE: triggers have to be done on a seperate trigger game object. If you are looking for a trigger, look there

    public int inputSystemToGetAxis()
    {
        if (Keyboard.current.aKey.isPressed)
            return -1;
        if (Keyboard.current.dKey.isPressed)
            return 1;   
        else
            return 0;
    }

    private int lookLeftOrRight()
    {
        direction = (Vector2) Input.mousePosition - screenCenter;

        if (direction.x < 0)
            return -1;
        else
            return 1;
    }

    public override void hit(GameObject hitGameObject)
    {
        base.hit(hitGameObject);
        if (shieldUpFlag && hitGameObject.tag == "Projectile")
        {
            camController.setShakeMagnitude(1f);
            camController.setShake(true);
            return;
        }

        camController.setShakeMagnitude(3f);
        camController.setShake(true);
    }
    protected override IEnumerator die()
    {
        yield return base.die();
    }

}