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
            Stats stats = pc.GetComponent<Stats>();

            switch (type)
            {
                case PickUpType.OrbShield:
                    pc.AddShieldOrbs((int)value);
                    pc.score += 250;
                    break;
                case PickUpType.Heal:
                    stats.health += value;
                    if (stats.health > stats.maxHealth)
                        stats.health = stats.maxHealth;
                    pc.UpdateHealthImage();
                    pc.score += 100;
                    break;
                case PickUpType.HPUpgrade:
                    stats.maxHealth += value;
                    //hardcapped at 6 hp
                    if (stats.maxHealth > 6)
                        stats.maxHealth = 6;  
                    stats.health = stats.maxHealth;
                    pc.UpdateHealthImage();
                    pc.score += 500;
                    break;
                case PickUpType.GunUpgrade:
                    if (pc.GetComponent<Gun>())
                        pc.GetComponent<Gun>().upgradeLvl = (int)value;
                    pc.score += 250;
                    break;
                case PickUpType.Shield:
                    pc.hasShield = true;
                    pc.score += 350;
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
    GunUpgrade,
    Shield
};
