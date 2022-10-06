using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int currentLevel = 1;

    private float totalXP;
    private float currentXP;
    private float targetXP;
    private float xpToNextLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetXP = GetTargetXP();
    }

    public void LevelUp() 
    {
        totalXP += currentXP;
        currentXP = 0f;
        xpToNextLevel = (xpToNextLevel * currentLevel) * (currentLevel * 1.75f);
        //HUD funcationality;
    }

    public float GetTargetXP() 
    {
        float xp;
        xp = xpToNextLevel - currentXP;
        return xp;
    }
}
