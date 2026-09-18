using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Rendering;

public class VCamController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float initialZoom = 5f;
    [SerializeField] float duration = .12f;
    [SerializeField] float shakeMagnitude = .2f;
    [SerializeField] float shootMagnitude = 2f;
    [SerializeField] bool shake = false;
    [SerializeField] bool shakeContinuously = false;
    [SerializeField] GameObject hand;
    private Vector3 originalLocal;
    private Vector2 direction = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        originalLocal = new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.zKey.isPressed)
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, 30f, Time.deltaTime*4f);
        else
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, initialZoom, Time.deltaTime*4f);

        if (shake)
        {
            StartCoroutine(shakeFunc());
            shake = false;
        }

        if(direction != Vector2.zero)
        {
             if (hand != null && hand.transform.childCount > 0)
             {
                GunController gunController = hand.transform.GetChild(0).GetComponent<GunController>();
                if(gunController != null)
                {
                    shootMagnitude = gunController.getRecoilAmount();
                    StartCoroutine(gunRecoil(direction));
                    direction = Vector2.zero;
                }
             }
        }
    }

    private IEnumerator shakeFunc()
    {
        transform.localPosition = new Vector3(originalLocal.x,originalLocal.y,originalLocal.z);
        float elapsedTime = 0f;
        while(elapsedTime < duration||shakeContinuously) //shake continuously is for keping the camera shaking at high velocities
        {
            float xOffset = Random.Range(-.5f, .5f) * shakeMagnitude;
            float yOffset = Random.Range(-.5f, .5f) * shakeMagnitude;

            transform.localPosition = new Vector3(xOffset, yOffset+20, transform.localPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = new Vector3(originalLocal.x, originalLocal.y, originalLocal.z);
    }

    private IEnumerator gunRecoil(Vector2 direction)
    {
        direction = HelperFunctions.rotateVector(direction, -transform.eulerAngles.z); //this is to account for the rotation of the player
        float elapsedTime = 0f;
        float recoilDuration = duration * .35f;
        while (elapsedTime < recoilDuration)
        {
            float progress = elapsedTime / recoilDuration;
            float offset = Mathf.Sin(progress * Mathf.PI * .5f) * shootMagnitude;
            transform.localPosition = originalLocal + new Vector3(direction.x * offset, direction.y * offset, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            float progress = elapsedTime / recoilDuration;
            float offset = (1f - progress) * shootMagnitude;
            transform.localPosition = originalLocal + new Vector3(direction.x * offset, direction.y * offset, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalLocal;
    }

    public void setShake(bool shake)
    {
        this.shake = shake;
    }

    public void setShakeContinuously(bool shakeContinuously)
    {
        this.shakeContinuously = shakeContinuously;
    }

    public void setShakeMagnitude(float shakeMagnitude)
    {
        this.shakeMagnitude = shakeMagnitude;
    }

    public void setGunRecoil(Vector2 direction)
    {
        this.direction = direction;
    }

}
