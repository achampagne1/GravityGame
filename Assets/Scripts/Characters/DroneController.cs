using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : CharacterController
{
    private EnemyAssistant[] enemyAssistants = new EnemyAssistant[5];
    private GameObject droneGun;
    private DroneGunController droneGunController;
    private Vector3 playerNotFound = new Vector3(0f, 0f, 1f);
    private Vector2 center = Vector2.zero;

    [SerializeField] float width = 3f;  
    [SerializeField] float height = .2f;  
    [SerializeField] float speed = 1f;
    [SerializeField] GameObject player;
    private float hoverTime = 0f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        center = (Vector2)transform.position;
        droneGun = transform.Find("Gun").gameObject;
        droneGunController = droneGun.GetComponent<DroneGunController>();
        int angleOffset = 0;
        for (int i = 0; i < enemyAssistants.Length; i++)
        {
            enemyAssistants[i] = new EnemyAssistant(gameObject,player);
            enemyAssistants[i].setAngleOffsetSetting(angleOffset);
            angleOffset += 72;
        }
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        hover();
        Vector3 playerPos = detectPlayerWrapper();
        if (playerPos != playerNotFound)
        {
            float angleRad = Mathf.Atan2(playerPos.y, playerPos.x);
            float angleDeg = angleRad * Mathf.Rad2Deg;
            droneGun.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg)*transform.rotation;
            droneGunController.setPlayerSeen(true);
        }
        else
            droneGunController.setPlayerSeen(false);
        base.FixedUpdate();
    }

    public void hover()
    {
        hoverTime = (hoverTime + (Time.deltaTime * speed)) % (2 * Mathf.PI);
        float localX = width * Mathf.Sin(hoverTime);
        float localY = height * Mathf.Sin(2 * hoverTime);
        Vector2 offset = (transform.right * localX) + (transform.up * localY);
        transform.position = center + offset;
    }
    private Vector3 detectPlayerWrapper()
    {
        for (int i = 0; i < enemyAssistants.Length; i++)
        {
            Vector3 temp = enemyAssistants[i].detectPlayer(false);
            if (temp != playerNotFound)
                return temp;
        }
        return playerNotFound;
    }
}
