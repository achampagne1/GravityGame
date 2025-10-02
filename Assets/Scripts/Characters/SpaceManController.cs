using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Diagnostics;

public class SpaceManController : SpacePersonController
{
    
    //game variables
    private bool enemyCollideFlag = false;
    private bool clickPressed = false;
    [SerializeField] float cameraShift = -110f; //for some reason this corrects the camera shift when the camera is shifted 20
    [SerializeField] VCamController camController;
    [SerializeField] UIHandler uIHandler;
    private Stopwatch holdTime = new Stopwatch();


    //vectors
    Vector2 screenCenter;
    Vector2 direction;


    public override void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 12, true); //for bullets. I know its a dumb placement but it needs to be somewhere with every level
        Physics2D.IgnoreLayerCollision(13, 12, true);
        Physics2D.IgnoreLayerCollision(12, 12, true);
        Physics2D.IgnoreLayerCollision(2, 12, true);
        Physics2D.IgnoreLayerCollision(11, 12, true);

        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        base.Start();
        setMaxHealth(10f);
    }

    public override void FixedUpdate()
    {
        float fuelBuffer = currentFuel;
        setMovement(inputSystemToGetAxis());
        space = Keyboard.current.spaceKey.isPressed;
        if (Keyboard.current.qKey.isPressed)
            handController.throwItem();
        base.FixedUpdate();

        if (rb.velocity.magnitude > 15)
        {
            camController.setShakeContinuously(true);
            camController.setShake(true); //setshake is a latch and automatically goes back to false
            camController.setShakeMagnitude(rb.velocity.magnitude/40);
        }
        else
            camController.setShakeContinuously(false);

        UIHandler.instance.setFuelValue(currentFuel);
        UIHandler.instance.setHealthValue(health);
    }

    public override void Update()
    {
        Vector3 handDirection = mouseToDirection(Input.mousePosition, transform.rotation);
        handController.setInputDirection(handDirection);
        setOrientation(lookLeftOrRight());
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            handController.useHandOnce();
            camController.setGunRecoil(handDirection);//this seems clunky 
            //if youre holding a different item then there shouldnt be any gun recoil camera shake. however I dont like the idea of the gun having the cam controlelr
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            handController.useHandHold();
            holdTime.Start();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (holdTime.IsRunning)
            {
                handController.useHandRelease(holdTime.ElapsedMilliseconds);
                //handController.useHandHold(holdTime.ElapsedMilliseconds);
                holdTime.Reset();
            }
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
        Vector2 adjustedScreenCenter = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f + (cameraShift * Screen.height)
        );
        Vector2 direction = (Vector2)Input.mousePosition - adjustedScreenCenter;
        return direction.x < 0 ? -1 : 1;
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