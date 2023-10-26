using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public GameObject playerGO;
    public ZombieSpawnerController zombieSpawnerSC;

    public float zombieSpeed = 1.5f;

    private bool isZombieAlive;
    // Start is called before the first frame update
    void Start()
    {
        isZombieAlive = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(zombieSpawnerSC.isZombiesAttacking == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerGO.transform.position, Time.fixedDeltaTime * zombieSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && isZombieAlive == true)
        {
            isZombieAlive = false;
            zombieSpawnerSC.ZombieAttackThisCop(collision.gameObject, this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            zombieSpawnerSC.ZombieGotShoot(this.gameObject);
        }
    }
}
