using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public Slider healthBar;
    public Slider staminaBar;

    public Player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        staminaBar.minValue = 0;
        healthBar.maxValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.maxValue = playerScript.stamina_max;
        staminaBar.value = playerScript.stamina;

        healthBar.maxValue = playerScript.maxHealth;
        healthBar.value = playerScript.currentHealth;
    }
}
