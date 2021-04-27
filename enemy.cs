using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour

{

    bool isHit = false;
    public GameObject drop;


    private void OnCollisionEnter2D(Collision2D collision) // скрипт для врагов, наносит дамаг при столкновении
    {
        if(collision.gameObject.tag == "Player" && !isHit)
        {
            collision.gameObject.GetComponent<player>().RecountHp(-1);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 8f, ForceMode2D.Impulse);
        }
    }


    public IEnumerator Death() // убиство врагов
    {
        if(drop != null){
            Instantiate(drop, transform.position, Quaternion.identity);
        }
        isHit = true;

        GetComponent<Animator>().SetBool("dead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(1).GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void startDeath()
    {
        StartCoroutine(Death());
    }
}
