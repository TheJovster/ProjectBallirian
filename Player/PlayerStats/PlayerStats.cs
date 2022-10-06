using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //components
    private PlayerUI playerUI;

    [Header("Main Attributes")]
    public int maxHealth;
    public int currentHealth;
    public int maxStamina;
    public int currentStamina;
    public int maxMana;
    public int currentMana;

    [Header("Player Attributes")]
    [SerializeField] private int strength = 5;
    [SerializeField] private int tenacity = 5;
    [SerializeField] private int agility = 5;
    [SerializeField] private int resourcefulness = 5;
    [SerializeField] private int sage = 5;

    private void Awake()
    {
        maxHealth = CalculateMaxHealth();
        maxStamina = CalculateMaxStamina();
        maxMana = CalculateMaxMana();
        playerUI = GetComponent<PlayerUI>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMana = maxMana;
    }
    private void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) 
        {
            TESTDamagePlayer();
        }
    }

    private int CalculateMaxHealth() //done at start and done at each level up
    {
        int health;
        health = strength * 4 + tenacity * 3;
        return health;
    }

    private int CalculateMaxStamina() //done at start and done at each level up
    {
        int stamina;
        stamina = tenacity * 5 + agility * 3;
        return stamina;
    }

    private int CalculateMaxMana() //done at start and done at each level up
    {
        int mana;
        mana = resourcefulness * 3 + sage * 5;
        return mana;
    }

    //called by the UI script at level up
    public void UpdateStrength()
    {
        strength++;
    }
    public void UpdateTenacity()
    {
        tenacity++;
    }
    public void UpdateAgility()
    {
        agility++;
    }
    public void UpdateResourcefulness()
    {
        resourcefulness++;
    }
    public void UpdateSage() 
    {
        sage++;
    }


    //called by the UI script on commit
    public void LevelUp()
    {
        //saves data changes
        CalculateMaxHealth();
        CalculateMaxStamina();
        CalculateMaxMana();
    }

    public void TakeDamage(int damageToTake) 
    {
        currentHealth -= damageToTake;
        if(currentHealth <= 0) 
        {
            currentHealth = 0;
            PlayerDeathHandler();
        }
    }

    private void PlayerDeathHandler() 
    {
        //route through GameManager;
        //incomplete
        Debug.Log("Player has died");
    }



    //test methods

    private void TESTDamagePlayer() 
    {
        currentHealth -= 5;
        playerUI.CheckHealthBar();
    }
}
