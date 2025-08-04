using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class ShieldTriggerController : TriggerBoundaryCotroller,IHealth
{
    [SerializeField] GameObject shieldHit;
    [SerializeField] float fadeSize = .1f;
    [SerializeField] float shieldRebuildTime = 5f;
    [SerializeField] float shieldRechargeRate = 2.0f;
    private CharacterController parentController;
    private CircleCollider2D collider;
    private float shieldStrength = 100f;
    private bool regenerateRunning = false;
    // Start is called before the first frame update
    public override void Start()
    {
        parentController = transform.parent.gameObject.GetComponent<CharacterController>();
        collider = GetComponent<CircleCollider2D>();
        base.Start();
    }

    public void Update()
    {
        if (shieldStrength == 0)
            parentController.setShieldUp(false);
        else
            parentController.setShieldUp(true); //maybe add a latch

        if(shieldStrength<100&&!regenerateRunning)
            StartCoroutine(regenerateShield());
        UIHandler.instance.setShieldValue(shieldStrength);
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Projectile"&& shieldStrength > 0)
        {
            StartCoroutine(spawnShieldHit(trigger.gameObject));
            trigger.gameObject.GetComponent<IDamager>().damage(gameObject);
            if (shieldStrength < 0)
                shieldStrength = 0;
        }
    }

    private IEnumerator regenerateShield()
    {
        regenerateRunning = true;
        while (shieldStrength < 100)
        {
            if (shieldStrength == 0)
                yield return new WaitForSeconds(shieldRebuildTime);
            shieldStrength += .1f;
            yield return new WaitForSeconds(1/shieldRechargeRate);
        }
        regenerateRunning = false;
    }
    private IEnumerator spawnShieldHit(GameObject trigger)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        float radius = collider.radius* Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        var (strikeLocation,rotation) = StrikeLocation.determineStrikeLocation(trigger,gameObject,radius);

        GameObject shieldHitTemp = Instantiate(shieldHit, new Vector3(strikeLocation.x,strikeLocation.y,transform.position.z), rotation);
        shieldHitTemp.transform.parent = gameObject.transform.parent;
        SpriteRenderer shieldHitTempSR = shieldHitTemp.GetComponent<SpriteRenderer>();
        SparkTrigger sparkTrigger = shieldHitTemp.GetComponent<SparkTrigger>();
        sparkTrigger.triggerSparks();
        float opacity = 1;
        while(opacity > 0)
        {
            HelperFunctions.changeOpacity(shieldHitTempSR, opacity-=fadeSize);
            yield return new WaitForSeconds(.05f);
        }
        GameObject.Destroy(shieldHitTemp);
        yield return null;
    }

    public void setHealth(float health)
    {
        shieldStrength = health*10f;
    }

    public float getHealth()
    {
        return shieldStrength/10f;
    }
}
