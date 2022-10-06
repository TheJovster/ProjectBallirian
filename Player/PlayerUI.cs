using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;

    [SerializeField] private GameObject mapGameObject;

    [Header("Player Main Stat Bars")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Slider manaBar;

    private bool mapActive = false;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();

        }

    }

    void Start()
    {
        healthBar.maxValue = playerStats.maxHealth;
        staminaBar.maxValue = playerStats.maxStamina;
        manaBar.maxValue = playerStats.maxMana;
        StartCoroutine("PopulateStatValues");
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    private void Update()
    {
        ToggleMap();

    }

    private void ToggleMap()
    {
        if (Input.GetKeyDown(KeyCode.M) && !mapActive)
        {
            mapGameObject.SetActive(true);
            mapActive = true;
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapActive)
        {
            mapGameObject.SetActive(false);
            mapActive = false;
        }
    }


    //Player stat handling

    public void CheckHealthBar()
    {
        healthBar.value = playerStats.currentHealth;
    }

    public void CheckStaminaBar() 
    {
        staminaBar.value = playerStats.currentStamina;
    }

    public void CheckManaBar() 
    {
        staminaBar.value = playerStats.currentMana;
    }

    private IEnumerator PopulateStatValues() 
    {
        yield return new WaitForSeconds(0.01f);
        healthBar.value = playerStats.currentHealth;
        staminaBar.value = playerStats.currentStamina;
        manaBar.value = playerStats.currentMana;
    }
}
