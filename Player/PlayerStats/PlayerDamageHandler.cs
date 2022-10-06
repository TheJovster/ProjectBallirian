using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private int baseDamage;
    private int weaponDamage;
    private int extraDamage;

    void Start()
    {
        baseDamage = CalculateBaseDamage();
        weaponDamage = GetWeaponDamage();
        extraDamage = CalculateExtraDamage();

        if(playerStats == null) 
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        
    }

    public int TotalDamageAmount()
    {
        int totalDamage;
        totalDamage = baseDamage + weaponDamage + extraDamage;
        return totalDamage;
    }

    private int CalculateBaseDamage() 
    {
        //base damage functionaity;
        return 15;
    }

    private int GetWeaponDamage() 
    {
        //archetcture wip
        return 5;
    }

    private int CalculateExtraDamage() 
    {
        //architecture wip
        return 5;
    }
}
