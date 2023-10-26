using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnerController : MonoBehaviour
{
    public GameObject zombieGO;
    public int zombieCount = 0;
    public List<GameObject> zombieList = new List<GameObject>();

    public GameObject playerSpawnerGO;
    public PlayerSpawnerController playerSpawnerSC;

    public bool isZombiesAttacking;
    // Start is called before the first frame update
    void Start()
    {
        playerSpawnerGO = GameObject.FindGameObjectWithTag("PlayerSpawner");
        playerSpawnerSC = playerSpawnerGO.GetComponent<PlayerSpawnerController>();
        SpawnZombies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnZombies()
    {
        for(int i = 0; i < zombieCount; i++)
        {
            Quaternion zombieRotation = Quaternion.Euler(new Vector3(0, 180, 0));//zombiyi döndürdük 
            GameObject zombie = Instantiate(zombieGO, GetZombiePos(), zombieRotation, transform);

            ZombieController zombieScript = zombie.GetComponent<ZombieController>();
            zombieScript.playerGO = playerSpawnerGO;//zombiecontroller scriptindeki playerı bu scripteki GO'ya eşitledik
            zombieScript.zombieSpawnerSC = this;//zombicontrollerdaki zombiespawnerda bu 
            zombieList.Add(zombie);
        }

    }

    public Vector3 GetZombiePos()
    {
        Vector3 pos = Random.insideUnitSphere * .1f;
        Vector3 newPos = transform.position + pos;
        //newPos.y = .4f;
        return newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GetComponent<BoxCollider>().enabled = false;//collidera bir kere çarptıktan sonra yok olsun 
            //playerların harkeetini durdur
            //playerler zombilere baksın
            //PlayerSpawnerController.instance.ZombieDetected(gameObject);
            playerSpawnerSC.ZombieDetected(gameObject);//direkt gameobject yazmamızın sebebi bu scripte bağlı olan 
            LookAtPlayer(other.gameObject);//other.gameobje yapmamızın sebebi triggerdaki othero lan obje
            isZombiesAttacking = true;
        }
    }

    private void LookAtPlayer(GameObject target)
    {
        Vector3 dir = transform.position - target.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);//verdiğimiz dir noktasında dönme değeri yarattık
        lookRotation.x = 0;//dönme işlemi sadece y ekseninde olcağı için x ve z yi sıfıra eşitledik
        lookRotation.z = 0;
        transform.rotation = lookRotation;
    }

    public void ZombieAttackThisCop(GameObject player, GameObject zombie)
    {
        zombieList.Remove(zombie);
        
        CheckZombieCount();//zombie sayılarını kontrol et
        
        playerSpawnerSC.PlayerGotKilled(player);//playerspawnercontroller bir zombi sana saldırdı   
        Destroy(zombie);
    }

    private void CheckZombieCount()
    {
        if(zombieList.Count <= 0)
        {
            playerSpawnerSC.AllZombiesKilled();//bütün zombiler öldü
        }
    }

    public void ZombieGotShoot(GameObject zombie)
    {
        zombieList.Remove(zombie);
        Destroy(zombie);
        CheckZombieCount();
    }
}
