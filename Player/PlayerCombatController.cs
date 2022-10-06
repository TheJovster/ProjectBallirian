using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    //private components
    private PlayerController playerController;


    [Header("Combat Behaviour Variables and Components - Exposed Private")]
    [SerializeField] private float attackSpeed;
    [SerializeField] private float timeSinceLastAttack;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private GameObject weaponGameObject; //this will need reworking later

    private bool isInCombat = false;
    private bool isSpellCasting = false;
    //private Weapon currentWeapon? otherwise, Weapon will be dependent on the combatController

    private void Awake()
    {
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        weaponGameObject.SetActive(false);
    }

    private void Update()
    {
        HandleAttackState();
        ToggleWeaponReady();
        Attack();
    }


    private void HandleAttackState()
    {
        playerController.playerAnimator.SetBool("IsInCombat", isInCombat);
    }

    //Attack behaviours
    private void Attack() //currently out of order
    {
        if (Input.GetMouseButtonDown(0) && isInCombat)
        {
            playerController.playerAnimator.SetTrigger("Attack");
            TryToAttackTarget(); //will need more logic, perhaps even on the weapon itself
        }
        else return;
    }

    private void TryToAttackTarget()
    {
        Ray ray = new Ray(playerController.mainCamera.transform.position, playerController.mainCamera.transform.forward);
        RaycastHit hitInfo;

        if(isInCombat) 
        {
            if(Physics.Raycast(ray, out hitInfo, attackRange, attackableLayer)) 
            {
                Debug.Log("Attacked " + hitInfo.collider.gameObject.name);
            }
            else 
            {
                Debug.Log("Whatever you are attacking is either not an Enemy or is out of range");
                return;
            }
        }
    }


    private void ToggleWeaponReady()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isInCombat)
        {
            weaponGameObject.SetActive(true);
            isInCombat = true;
            //can attack
            //game manager(?) can start combat music
            //attack logic in the Attack method
        }
        else if (Input.GetKeyDown(KeyCode.R) && isInCombat)
        {
            //todo: sheathe the sword
            isInCombat = false;
            //game manager(?) stops combat music if all enemies are dead if not, returns
            //play sheathe animation
            weaponGameObject.SetActive(false);
        }
    }

    //spellcasting controls

    private void ToggleSpellcasting()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isSpellCasting)
        {
            //spellcasting logic
            //enter spellcasting state
            //enable isSpellCasting
        }
        else if (Input.GetKeyDown(KeyCode.F) && isSpellCasting)
        {
            //go back to idle state
            //disable spellcasting
        }
    }
}

