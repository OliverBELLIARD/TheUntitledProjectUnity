using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BushesRenderState : MonoBehaviour
{
    TilemapRenderer tr;
    Player playerScript;

    //[Tooltip("Drag here all the enemies scripts")]
    //public Enemy[] enemiesScripts;
    //public EnemyAI[] enemiesScripts;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<TilemapRenderer>();
        playerScript = GameObject.Find("Player").GetComponentInChildren<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time < playerScript.nextAttackTime && playerScript.animator.GetFloat("Vertical") < 0)
        {
            tr.sortingOrder = -1;
        }
        else
        {
            tr.sortingOrder = 0;
        }
    }
}
