using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceManController : CharacterController
{
    
    //game variables
    bool enemyCollideFlag = false;
    private bool clickPressed = false;


    public void Start()
    {
        calculateCharacterStart();
        setMaxHealth(10f);
    }

    public void FixedUpdate()
    {
        float fuelBuffer = currentFuel;
        setMovement(inputSystemToGetAxis());
        setOrientation(lookLeftOrRight());
        setJump(Keyboard.current.spaceKey.isPressed);
        setThrow(Keyboard.current.qKey.isPressed);
        calculateCharacterUpdate();

        if(fuelBuffer!=currentFuel)
            UIHandler.instance.setFuelValue(currentFuel);
    }

    public override void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            handController.useHand();
        handController.setInputDirection(mouseToDirection(Input.mousePosition, transform.rotation));
    }

    private Vector3 mouseToDirection(Vector3 inputDirection, Quaternion playerRotation)
    {
        //This function was written by chat GPT
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
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
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter;

        if (direction.x < 0)
            return -1;
        else
            return 1;
    }
}