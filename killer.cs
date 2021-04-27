using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision) // убиство врагов
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 8f, ForceMode2D.Impulse);
            gameObject.GetComponentInParent<enemy>().startDeath();
        }
    }
}
