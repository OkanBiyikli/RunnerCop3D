using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletGO;
    public Transform bulletSpawnTransform;
    public float bulletSpeed = 13f;
    public float bulletSpeedCoolDown = 2f;
    bool isShootingOn;
    Animator anim;

    Transform playerSpawnerCenterPoint;
    float goingToCenter = 4f;

    PlayerController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this; 
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerSpawnerCenterPoint = transform.parent.gameObject.transform;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerSpawnerCenterPoint.position, Time.fixedDeltaTime * goingToCenter);

        if (bulletSpeedCoolDown <= 1)
        {
            bulletSpeedCoolDown = 1;
        }
    }

    public void StartShooting()
    {
        StartShootingAnim();
        isShootingOn = true;
        StartCoroutine(Shooting());
    }

    public void StopShooting()
    {
        isShootingOn = false;
        StartRunAnim();
    }

    IEnumerator Shooting()
    {
        while(isShootingOn)
        {
            yield return new WaitForSeconds(.1f);//yarım saniye bekle öyle ateş et
            Shoot();
            yield return new WaitForSeconds(bulletSpeedCoolDown);
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletGO, bulletSpawnTransform.position, Quaternion.identity);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.velocity = transform.forward * bulletSpeed;
    }

    private void StartShootingAnim()
    {
        anim.SetBool("isShooting", true);
        anim.SetBool("isRunning", false);
    }

    private void StartRunAnim()
    {
        anim.SetBool("isShooting", false);
        anim.SetBool("isRunning", true);
    }

    public void StartIdleAnim()
    {
        anim.SetBool("isIdle", true);
        anim.SetBool("isRunning", false);
    }
}
