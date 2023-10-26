using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GateType { multiplyType, additionalType, increaseBullet, decreaseBullet}
public class GateController : MonoBehaviour
{
    public float gateValue;
    public TextMeshProUGUI gateText;
    public GateType gateType;
    private GameObject playerSpawnerGO;
    private PlayerSpawnerController playerSpawnerSc;
    bool hasGateUsed;
    private GateHolderController gateHolderSC;
    // Start is called before the first frame update
    void Start()
    {
        playerSpawnerGO = GameObject.FindGameObjectWithTag("PlayerSpawner");
        playerSpawnerSc = playerSpawnerGO.GetComponent<PlayerSpawnerController>();
        gateHolderSC = transform.parent.gameObject.GetComponent<GateHolderController>();//gateholder scmiz parent objemizde olduğu için parent objeye gidip çağırdık

        AddGateValueAndSymbol();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && hasGateUsed == false)
        {
            hasGateUsed = true;
            playerSpawnerSc.SpawnPlayer(gateValue, gateType);
            //playerSpawnerSc.AdjustBulletSpeed(gateValue, gateType);
            gateHolderSC.CloseGates();
            Destroy(gameObject);
        }

        /*else if(other.tag == "Player" && hasGateUsed == false && gateType ==GateType.increaseBullet && gateType == GateType.decreaseBullet)
        {
            hasGateUsed = true;
            playerSpawnerSc.AdjustBulletSpeed(gateValue, gateType);
            gateHolderSC.CloseGates();
            Destroy(gameObject);
        }*/
    }

    private void AddGateValueAndSymbol()
    {
        switch(gateType)
        {
            case GateType.multiplyType:
                gateText.text = "x" + gateValue.ToString();
                break;

            case GateType.additionalType:
                gateText.text = "+" + gateValue.ToString();
                break;

            case GateType.increaseBullet:
                gateText.text = "-Atk" + gateValue.ToString();
                break;

            case GateType.decreaseBullet:
                gateText.text = "+Atk" + gateValue.ToString();
                break;

            default:
                break;
        }
         
    }
}
