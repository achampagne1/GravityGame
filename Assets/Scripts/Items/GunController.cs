using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.FilePathAttribute;

public class GunController : ItemController
{
    //serialized variables
    [SerializeField] private float bulletForce = 50.0f;
    [SerializeField] private float fireLimiterVariable = .1f;

    //vectors
    private Vector3 shootDirection = Vector3.zero;

    //object creation
    private Animator animator;
    private StopWatch fireLimiter = new StopWatch();
    [SerializeField] private float recoilAmount = .5f;
    [SerializeField] private AudioClip gunshotClip;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject muzzleFlash;

    public override void Start()
    {

        base.Start();
        parentedEffect.float1 = recoilAmount;
        try
        {
            animator = GetComponent<Animator>();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void useItemOnce()
    {
        if (!fireLimiter.getIsRunning())
                fireLimiter.start();
        if(fireLimiter.getIsRunning() && fireLimiter.getElapsedTime() > fireLimiterVariable){
            shootDirection = transform.parent.TransformDirection(facingLeft ? Vector2.left : Vector2.right);
            shootWrapper();
            fireLimiter.reset();
            fireLimiter.start();
            base.useItemOnce();
        }
    }


    private void shootWrapper()
    {
        Vector3 offset = new Vector3(.5f, .25f, 0);
        GameObject bulletClone= Instantiate(bullet, transform.position + transform.rotation * offset, transform.rotation);
        GameObject muzzleFlashClone = Instantiate(muzzleFlash,transform);
        muzzleFlashClone.transform.parent = transform;
        muzzleFlashClone.transform.localPosition = new Vector3(3.04f, 1.05f, 0f);
        Destroy(muzzleFlashClone, .05f);
        animator.SetTrigger("Shoot");
        bulletClone.GetComponent<BulletController>().init(transform.parent.gameObject.layer);
        bulletClone.GetComponent<Rigidbody2D>().AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
        SoundManager.instance.playSound(gunshotClip, transform, 1f);
        parentedEffect?.parentedEffect(transform, facingLeft ? -1 : 1, shootDirection, handOffset1, new Vector2(180f, -180f), handOffset1);
    }

    public float getRecoilAmount()
    {
        return recoilAmount;
    }
}