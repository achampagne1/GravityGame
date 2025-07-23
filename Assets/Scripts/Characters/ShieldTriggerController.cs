using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTriggerController : TriggerBoundaryCotroller
{
    [SerializeField] GameObject shieldHit;
    [SerializeField] float fadeSize = .1f;
    private CharacterController parentController;
    private CircleCollider2D collider;
    private float shieldStrength = 10f;
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
                StartCoroutine(spawnShieldHit(trigger.gameObject));
                //shieldStrength -= 1; uncomment later
                if (shieldStrength < 0)
                    shieldStrength = 0;
            }
           // UIHandler.instance.setHealthValue(characterController.getHealth()); change to shield when you make the ui
        }
    }

    private IEnumerator spawnShieldHit(GameObject trigger)
    {
        var (strikeLocation,rotation) = determineStrikeLocation(trigger);
        GameObject shieldHitTemp = Instantiate(shieldHit, new Vector3(strikeLocation.x,strikeLocation.y,transform.position.z), rotation);
        shieldHitTemp.transform.SetParent(transform);
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

    private (Vector2,Quaternion) determineStrikeLocation(GameObject trigger) 
    {
        Vector2 triggerVelocity = (Vector2)trigger.GetComponent<Rigidbody2D>().velocity;
        Vector2 triggerLocationCurrent = (Vector2)trigger.transform.position;
        Vector2 triggerLocationPrevious = triggerLocationCurrent - triggerVelocity;
        Vector2 worldCenter = (Vector2)transform.position + collider.offset;
        float worldRadius = collider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        Vector2 intersection = new Vector2();
        bool found = HelperFunctions.chordIntersection(triggerLocationPrevious, triggerLocationCurrent, worldCenter, worldRadius, out intersection);

        float angle = Mathf.Atan2((intersection.y - worldCenter.y), (intersection.x - worldCenter.x)) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        return (intersection,rotation);
    }
}
