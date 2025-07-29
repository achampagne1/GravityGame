using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGunController : MonoBehaviour
{
    [SerializeField] bool shoot = false;
    [SerializeField] float bulletForce = 35.0f;
    [SerializeField] float shootInterval = 2f;
    [SerializeField] GameObject laser;
    [SerializeField] AudioClip gunShot;
    private AudioSource soundFx;
    private Coroutine shootCoroutine = null;
    private Timer playerSeenTimer = new Timer(.5f);
    private Vector3 locationBuffer = new Vector3(0f,0f,0f);
    private int angleOffset = 0;
    private bool playerSeen = false;
    private bool recoilRunning = false;
    private Coroutine recoilCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        soundFx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 playerPos = detectPlayer();

        bool playerSeen;
        (playerSeen,playerPos) = calculatePlayerSeen(playerPos);
        Vector3 dir = playerPos - transform.position;
        float angleRad = Mathf.Atan2(dir.y, dir.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        if (playerSeen && shootCoroutine==null)
        {
            shootCoroutine = StartCoroutine(shootFunction());
        }
        else if(!playerSeen && shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        if (shoot)
        {
            if(recoilRunning) 
                StopCoroutine(recoilCoroutine);
            recoilCoroutine = StartCoroutine(recoil());
            SoundManager.instance.playSound(gunShot, transform, .8f);
            GameObject laserClone = Instantiate(laser, transform.position, transform.rotation);
            laserClone.GetComponent<LaserController>().init(gameObject.layer);
            laserClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * bulletForce, ForceMode2D.Impulse);
        }
        shoot = false;
    }

    private IEnumerator shootFunction()
    {
        while (true)
        {
            shoot = true;
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private IEnumerator recoil()
    {
        recoilRunning = true;
        Vector3 originalPosition = transform.localPosition;
        Vector3 recoilOffset = new Vector3(-1f, 0f, 0f);
        Vector3 targetPosition = originalPosition + (transform.rotation * recoilOffset);

        float recoilDuration = 0.05f;   
        float returnDuration = 0.1f;    
        float t = 0f;

        while (t < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t / recoilDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;

        t = 0f;
        while (t < returnDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, t / returnDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
        recoilRunning = false;
    }

    private Vector3 detectPlayer()
    {
        if ((angleOffset += 1) > 360)
            angleOffset = 0;

        for (int i = 0; i < 36; i++)
        {
            float angle = i*10f+angleOffset; //the angle offset it to ensure there are no blind spots
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

    private (bool,Vector3) calculatePlayerSeen(Vector3 input)
    {
        if (input == Vector3.zero)
        {
            input = locationBuffer;
            if (!playerSeenTimer.getIsRunning())
            {
                playerSeenTimer.startTimer();
            }
            else
            {
                if (playerSeenTimer.checkTimer())
                    playerSeen = false;
            }
        }
        else
        {
            locationBuffer = input;
            playerSeen = true;
            playerSeenTimer.resetTimer();
        }

        return (playerSeen,input);
    }
}
