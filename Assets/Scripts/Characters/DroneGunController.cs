using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGunController : MonoBehaviour
{
    [SerializeField] bool shoot = false;
    [SerializeField] float bulletForce = 35.0f;
    [SerializeField] GameObject laser;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 playerPos = detectPlayer();
        if (playerPos != Vector3.zero)
        {
            Vector3 dir = playerPos - transform.position;
            float angleRad = Mathf.Atan2(dir.y, dir.x);
            float angleDeg = angleRad * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            if (shoot)
            {
                GameObject laserClone = Instantiate(laser, transform.position, transform.rotation);
                laserClone.GetComponent<LaserController>().init(gameObject.layer);
                laserClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * bulletForce, ForceMode2D.Impulse);
            }
        }
        shoot = false;
    }

    private Vector3 detectPlayer()
    {
        for (int i = 0; i < 36; i++)
        {
            float angle = i*10f;
            Vector2 temp = new Vector2(Mathf.Cos(angle * Mathf.PI / 180), Mathf.Sin(angle * Mathf.PI / 180));
            RaycastHit2D[] lookForPlayer = Physics2D.RaycastAll(transform.position, temp, 30f);
            foreach (RaycastHit2D hit in lookForPlayer)
            {
                if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 15)
                    break;
                if (hit.collider.gameObject != gameObject && hit.collider.gameObject.layer == 9)
                {
                    return hit.transform.position;
                }
            }
        }
        return Vector3.zero;
    }
}
