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
    private Vector3 playerNotFound = new Vector3(0f, 0f, 1f);
    private bool playerSeen = false;
    private bool recoilRunning = false;
    private Coroutine recoilCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            float angleRad = transform.eulerAngles.z * Mathf.Deg2Rad;
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

    public void setPlayerSeen(bool playerSeen)
    {
        this.playerSeen = playerSeen;
    }
}
