using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTriggerController : TriggerBoundaryCotroller
{
    [SerializeField] GameObject shieldHit;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.name == "Bullet(Clone)"&& trigger.gameObject.GetComponent<BulletController>().getShotBy() != parent.layer)
        {
            GameObject.Destroy(trigger.gameObject);
            StartCoroutine(spawnShieldHit());
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
