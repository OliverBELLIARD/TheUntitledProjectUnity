using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnDark : MonoBehaviour
{
    public Color Shadow;
    public Color Light;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<SpriteRenderer>().color = Shadow;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<SpriteRenderer>().color = Light;
    }
}
