using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class ShieldTriggerController : MonoBehaviour
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
    public void Start()
    {
        parentController = transform.parent.gameObject.GetComponent<CharacterController>();
        collider = GetComponent<CircleCollider2D>();
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
            shieldStrength -= trigger.gameObject.GetComponent<IDamager>().damage(gameObject)*10f;
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
        var (strikeLocation,rotation) = determineStrikeLocation(trigger);
        stopwatch.Stop();

        double elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        UnityEngine.Debug.Log($"Took {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
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
        Vector2 worldCenter = (Vector2)transform.position;
        float worldRadius = collider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        Vector2 intersection = new Vector2();
        bool found = HelperFunctions.chordIntersection(triggerLocationPrevious, triggerLocationCurrent, (Vector2)transform.position, worldRadius, out intersection);

        float angle = Mathf.Atan2((intersection.y - worldCenter.y), (intersection.x - worldCenter.x)) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        return (intersection,rotation);
    }
}
