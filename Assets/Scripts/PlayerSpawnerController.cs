using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnerController : MonoBehaviour
{
    public GameObject playerGO;
    public float playerSpeed = 5f;
    float xSpeed;
    float maxXPosition = 4.1f;
    public bool isMoving;
    public float newBulletSpeedCD;

    public List<GameObject> playersList = new List<GameObject>();//player gameobjeleri için liste oluşturduk

    PlayerController playerController;
    public static PlayerSpawnerController instance;

    public AudioSource playerSpawnerAudioSource;
    public AudioClip gateClip, congratsClip, failClip, shootClip;

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
        //isMoving = true;
        playerController = GetComponent<PlayerController>();
        newBulletSpeedCD = 2;
    }

    // Update is called once per frame
    void Update()
    {
        //AllCopsDied();
        if (isMoving == false)
        {
            return;
        }
        float touchX = 0;
        float newXValue = 0;
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            xSpeed = 250f;
            touchX = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if(Input.GetMouseButton(0))
        {
            xSpeed = 500;
            touchX = Input.GetAxis("Mouse X");
        }

        newXValue = transform.position.x + xSpeed * touchX * Time.deltaTime;
        newXValue = Mathf.Clamp(newXValue, -maxXPosition, maxXPosition);

        Vector3 playerNewPosition = new Vector3(newXValue, transform.position.y, transform.position.z + playerSpeed * Time.deltaTime);
        transform.position = playerNewPosition;
    }

    public void SpawnPlayer(float gateValue, GateType gateType)
    {
        PlayAudio(gateClip);
        if (gateType == GateType.additionalType)
        {
            for (int i = 0; i < gateValue; i++)//deneme
            {
                //newplayergo yeni oluşturacağımız objemiz
                GameObject newPlayerGO = Instantiate(playerGO, GetPlayerPosition(), Quaternion.identity, transform);
                //burda transform yazmamızın sebebi hiyerarşideki playerspawnerın childi olarak gelsin diye
                playersList.Add(newPlayerGO);//oluşturduğumuz yeni playerı listeye ekle dedik

                PlayerController cop = playersList[i].GetComponent<PlayerController>();
                cop.bulletSpeedCoolDown = newBulletSpeedCD;
            }
        }
        else if (gateType == GateType.multiplyType)
        {
            float newPlayerCount = (playersList.Count * gateValue) - playersList.Count;//mesela 2x2=4 yeni player sayımız 4+2=6 olur ama playerlistcountu çıkarırsak tekrar 6-2=4 olcak
            for(int i = 0; i < newPlayerCount; i++)
            {
                GameObject newPlayerGO = Instantiate(playerGO, GetPlayerPosition(), Quaternion.identity, transform);
                playersList.Add(newPlayerGO);

                PlayerController cop = playersList[i].GetComponent<PlayerController>();
                cop.bulletSpeedCoolDown = newBulletSpeedCD;
            }
        }

        else if (gateType == GateType.increaseBullet)
        {
            StartDecreaseAllCopsBullets(gateValue);
            //float newBulletSpeedCD = playerController.bulletSpeedCoolDown + gateValue;
            //playerController.bulletSpeedCoolDown = newBulletSpeedCD;
            Debug.Log("Atış hızı azaltıldı");
        }

        else if (gateType == GateType.decreaseBullet)
        {
            StartIncreaseAllCopsBullets(gateValue);
            //float newBulletSpeedCD = playerController.bulletSpeedCoolDown - gateValue;
            //playerController.bulletSpeedCoolDown = newBulletSpeedCD;
            Debug.Log("Atış hızı yükseltildi");
        }
    }

   /* public void AdjustBulletSpeed(int gateValue, GateType gateType)
    {
        if(gateType == GateType.increaseBullet)
        {
            float newBulletSpeedCD = playerController.bulletSpeedCoolDown + gateValue;
            playerController.bulletSpeedCoolDown = newBulletSpeedCD;
            Debug.Log("Atış hızı azaltıldı");
        }

        else if (gateType == GateType.decreaseBullet)
        {
            float newBulletSpeedCD = playerController.bulletSpeedCoolDown - gateValue;
            playerController.bulletSpeedCoolDown = newBulletSpeedCD;
            Debug.Log("Atış hızı yükseltildi");
        }
    }*/

    public Vector3 GetPlayerPosition()
    {
        Vector3 position = Random.insideUnitSphere * .1f;//0.1 yarıçaplı bi alan belirledik
        Vector3 newPos = transform.position + position;//yeni bi pozisyon oluşturduk ve üst satırdaki yarıçaplı alanı oraya ekledik
        newPos.y = 0.5f;
        return newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "FinishLine")
        {
            StopBackgroundClip();
            isMoving = false;
            PlayAudio(congratsClip);
            StartAllCopsIdleAnim();
            GameManager.instance.ShowSuccessPanel();
        }
    }

    public void PlayerGotKilled(GameObject playerGO)
    {
        playersList.Remove(playerGO);
        Destroy(playerGO);
        CheckPlayersCount();
    }

    private void CheckPlayersCount()
    {
        if(playersList.Count <= 0)
        {
            StopBackgroundClip();
            PlayAudio(failClip);
            StopPlayer();
            GameManager.instance.ShowFailPanel();
        }
    }

    public void ZombieDetected(GameObject target)
    {
        isMoving = false;
        LookAtZombies(target);
        //zombiye bak
        StartAllCopsShooting();
        PlayAudio(shootClip);
    }

    public void LookAtZombies(GameObject target)
    {
        Vector3 dir = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        lookRotation.x = 0;
        lookRotation.z = 0;
        transform.rotation = lookRotation;

    }

    private void LookAtForward()
    {
        transform.rotation = Quaternion.identity;
    }


    public void AllZombiesKilled()
    {
        LookAtForward();//polisler tekrar ileriye bakacak
        MovePlayer();//yürümeye devam edecekler
    }


    public void MovePlayer()
    {
        StartAllCopsRunning();
        isMoving = true;
    }

    private void StopPlayer()
    {
        isMoving = false;
    }


    public void StartAllCopsShooting()
    {
        for(int i = 0; i < playersList.Count; i++)
        {
            PlayerController cop = playersList[i].GetComponent<PlayerController>();
            cop.StartShooting();
        }
    }

    public void StartAllCopsRunning()
    {
        for(int i = 0; i < playersList.Count; i++)
        {
            PlayerController cop = playersList[i].GetComponent<PlayerController>();
            cop.StopShooting();
        }
    }

    public void StartAllCopsIdleAnim()
    {
        for(int i = 0; i < playersList.Count; i++)
        {
            PlayerController cop = playersList[i].GetComponent<PlayerController>();
            cop.StartIdleAnim();
        }
    }

    public void AllCopsDied()
    {
        if(playersList.Count < 1)
        {
            Time.timeScale = 0;
        }
    }

    public void StartDecreaseAllCopsBullets(float gateValue)
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            PlayerController cop = playersList[i].GetComponent<PlayerController>();
            // newBulletSpeedCD’yi public bir float yaptım, aşağıda nedenini açıklayacağım
            newBulletSpeedCD = cop.bulletSpeedCoolDown + gateValue;
            cop.bulletSpeedCoolDown = newBulletSpeedCD;
            // mermilerin ilk çıkış zamanını yavaşlatır.
        }
    }

    public void StartIncreaseAllCopsBullets(float gateValue)
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            PlayerController cop = playersList[i].GetComponent<PlayerController>();
            // newBulletSpeedCD’yi public bir float yaptım, aşağıda nedenini açıklayacağım
            newBulletSpeedCD = cop.bulletSpeedCoolDown - gateValue;
            cop.bulletSpeedCoolDown = newBulletSpeedCD;
            //mermilerin ilk çıkış zamanını hızlandırır
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        if(playerSpawnerAudioSource != null)
        {
            playerSpawnerAudioSource.PlayOneShot(clip, 0.5f);
        }
    }

    private void StopBackgroundClip()
    {
        Camera.main.GetComponent<AudioSource>().Stop();
    }

}
