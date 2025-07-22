using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTriggerController : TriggerBoundaryCotroller
{
    [SerializeField] GameObject shieldHit;
    private CharacterController parentController;
    private float shieldStrength = 10f;
    // Start is called before the first frame update
    public override void Start()
    {
        parentController = transform.parent.gameObject.GetComponent<CharacterController>();
        base.Start();
    }

    public void Update()
    {
        if (shieldStrength == 0)
            parentController.setInvincible(false);
        else
            parentController.setInvincible(true); //maybe add a latch
    }

    protected override void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "Bullet(Clone)"&& trigger.gameObject.GetComponent<BulletController>().getShotBy() != parent.layer)
        {
            if (shieldStrength > 0)
            {
                StartCoroutine(spawnShieldHit());
                shieldStrength -= 1;
                if (shieldStrength < 0)
                    shieldStrength = 0;
            }
           // UIHandler.instance.setHealthValue(characterController.getHealth()); change to shield when you make the ui
        }
    }

    private IEnumerator spawnShieldHit()
    {
        GameObject shieldHitTemp = Instantiate(shieldHit, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        GameObject.Destroy(shieldHitTemp);
        yield return null;
    }
}
