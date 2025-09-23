using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] AudioClip gunshotClip;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject muzzleFlash;

    public override void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 13, true);
        Physics2D.IgnoreLayerCollision(11, 13, true);
        Physics2D.IgnoreLayerCollision(13, 13, true);

        base.Start();

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

    public override void useItem()
    {
        if (!fireLimiter.getIsRunning())
                fireLimiter.start();
        if(fireLimiter.getIsRunning() && fireLimiter.getElapsedTime() > fireLimiterVariable){
            shootDirection = transform.rotation * Vector3.right;
            shootWrapper();
            fireLimiter.reset();
            fireLimiter.start();
        }
    }


    private void shootWrapper()
    {
        Vector3 offset = new Vector3(.5f, .25f, 0);
        offset.y = offset.y * (facingLeft ? -1 : 1);
        GameObject bulletClone= Instantiate(bullet, transform.position + transform.rotation * offset, transform.rotation);
        GameObject muzzleFlashClone = Instantiate(muzzleFlash,transform);
        muzzleFlashClone.transform.parent = transform;
        muzzleFlashClone.transform.localPosition = new Vector3(3.04f, 1.05f, 0f);
        Destroy(muzzleFlashClone, .05f);
        animator.SetTrigger("Shoot");
        bulletClone.GetComponent<BulletController>().init(transform.parent.gameObject.layer);
        bulletClone.GetComponent<Rigidbody2D>().AddForce(shootDirection * bulletForce, ForceMode2D.Impulse);
        SoundManager.instance.playSound(gunshotClip, transform, 1f);

    }
}