using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    //object creation
    GameObject bulletObject;
    public void Start()
    {
        bulletObject = GameObject.Find("Bullet");
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            shoot(transform.position, getMouseDirection(Input.mousePosition, transform.rotation));
        }
    }

    public void shoot(Vector3 location, Vector3 direction)
    {
        GameObject ShotBullet = Instantiate(bulletObject, location, Quaternion.identity);
        ShotBullet.GetComponent<BulletController>().newInstance(direction);
        ShotBullet.GetComponent<BulletController>().Start();
    }

    private static Vector3 getMouseDirection(Vector3 mousePosition, Quaternion playerRotation)
    {
        //This function was written by chat GPT
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = new Vector2(mousePosition.x, mousePosition.y) - screenCenter;
        Vector2 normalizedDirection = direction.normalized;
        Vector3 direction3D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0f);
        Vector3 rotatedDirection = playerRotation * direction3D;
        return new Vector2(rotatedDirection.x, rotatedDirection.y).normalized;
    }
}
