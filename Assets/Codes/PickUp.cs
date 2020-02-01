using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public PickUpType type;
    public float value;
    public bool destroyOnPickUp;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            PlayerController pc = other.GetComponent<PlayerController>();

            switch (type)
            {
                case PickUpType.OrbShield:
                    pc.AddShieldOrbs((int)value);
                    break;
                case PickUpType.Heal:
                    //heal
                    break;
                case PickUpType.HPUpgrade:
                    //upgrade max hp and heal to full
                    break;
                case PickUpType.GunUpgrade:
                    if (pc.GetComponent<Gun>())
                        pc.GetComponent<Gun>().upgradeLvl = 1;
                    break;
            }

            if (destroyOnPickUp)
                Destroy(gameObject);
        }
    }
}


public enum PickUpType
{
    Heal,
    HPUpgrade,
    OrbShield,
    GunUpgrade
};
