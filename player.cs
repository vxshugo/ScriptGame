using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public float jumpHeight;
    public Transform groundCheck;
    public bool isGrounded;
    Animator anim;
    int curHp;
    int maxHp = 3;
    bool isHit = false;
    public Main main;
    public bool key = false;
    bool canTp = true;
    public bool inWater = false;
    bool isClimbing = false;
    int coins = 0;
    bool canHit = true;
    public GameObject blueGem, greenGem;
    int gemCount = 0;
    public Inventory inventory;
    public Soundeffector soundeffector;
    public Joystick joystick;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {   // движение лево право (плавное)

        
        if(inWater && !isClimbing){
            anim.SetInteger("State", 4);
            isGrounded = true;
            if (joystick.Horizontal > 0 || joystick.Horizontal < 0f){
                Flip();
            }
        }else{

            CheckGround();
            if (joystick.Horizontal < 0.3f && joystick.Horizontal > -0.3f && (isGrounded)  && !isClimbing)
            {
                anim.SetInteger("State", 1);
            }
            else
            {
                Flip();
                if (isGrounded  && !isClimbing)
                    anim.SetInteger("State", 2);
            }
        }
    }

    public void Jump(){
        if (isGrounded)   
            {
                rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
                soundeffector.PlayJumpSound();
            }
    }
    void FixedUpdate()
    {
        if(joystick.Horizontal > 0f){
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }else if (joystick.Horizontal < 0f){
            rb.velocity = new Vector2 (-speed, rb.velocity.y);
        }else{
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }                                                      // движение лево право (плавное)
    }

    public void GoLeft(){
        rb.velocity = new Vector2 (-speed, rb.velocity.y);
    }

    void Flip()  //поворот модельки персонажа
    {
        if (joystick.Horizontal >= 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (joystick.Horizontal <= -0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void CheckGround()  // метод проверки земли под ногами
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        isGrounded = colliders.Length > 1;
        if (!isGrounded  && !isClimbing)
        {
            anim.SetInteger("State", 3);
        }
    }

    public void RecountHp(int deltaHp)  // Метод + и - ХП
    {
    
        if (deltaHp < 0 && canHit)
        {
            curHp = curHp + deltaHp;
            StopCoroutine(OnHit());
            canHit = false;
            isHit = true;
            StartCoroutine(OnHit());
        }
        else if (deltaHp > 0) {
            if(curHp >= maxHp){
                curHp = maxHp;
            }else{
                curHp = curHp + deltaHp;
            }
        }
        if (curHp <= 0)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;

            Invoke("Lose", 1f);

        }
    }


    IEnumerator OnHit() // Эффект удара
    {
        if(isHit)
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g - 0.04f, GetComponent<SpriteRenderer>().color.b - 0.04f);
        else
            GetComponent<SpriteRenderer>().color = new Color(1f, GetComponent<SpriteRenderer>().color.g + 0.04f, GetComponent<SpriteRenderer>().color.b + 0.04f);


        if (GetComponent<SpriteRenderer>().color.g >= 1f){

            canHit = true;
            StopCoroutine(OnHit());

            yield break;
        }

        if (GetComponent<SpriteRenderer>().color.g <= 0){
            isHit = false;
            GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);

        }


        yield return new WaitForSeconds(0.02f);
        StartCoroutine(OnHit());
    }

    void Lose()
    {
        main.GetComponent<Main>().Lose();
    }


    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Key"){
            Destroy(collision.gameObject);
            key = true;
            inventory.Add_key();
        }

        if(collision.gameObject.tag == "Door"){
            if(collision.gameObject.GetComponent<Door>().isOpen && canTp ){
               collision.gameObject.GetComponent<Door>().Teleport(gameObject);
               canTp = false;
               StartCoroutine(TPwait());
            }
            else if (key){
                collision.gameObject.GetComponent<Door>().Unlock();
            }
        }

        if(collision.gameObject.tag == "Coin"){
            Destroy(collision.gameObject);
            coins++;
            soundeffector.PlayCoinSound();
        }

        if(collision.gameObject.tag == "Heart"){
            Destroy(collision.gameObject);
            //RecountHp(1);
            inventory.Add_hp();
        }


        if(collision.gameObject.tag == "Mushroom"){
            Destroy(collision.gameObject);
            RecountHp(-1);
        }


        if(collision.gameObject.tag == "GreenGem"){
            Destroy(collision.gameObject);
            //StartCoroutine(NoHit());
            inventory.Add_gg();
        }


        if(collision.gameObject.tag == "GemBlue"){
            Destroy(collision.gameObject);
            //StartCoroutine(SpeedBoost());
            inventory.Add_bg();
        }
    }

    IEnumerator TPwait(){
        yield return new WaitForSeconds(1f);
        canTp = true;
    }

    private void OnTriggerStay2D(Collider2D collision){
        if(collision.gameObject.tag == "Ladder"){
            isClimbing = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if(Input.GetAxis("Vertical") == 0){
                anim.SetInteger("State", 5);
            }else{
                anim.SetInteger("State", 6);
                transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.5f * Time.deltaTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ladder"){
            isClimbing = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Trampoline")
        {
            StartCoroutine(TrampolineAnim(collision.gameObject.GetComponentInParent<Animator>()));
        }
    }

    IEnumerator TrampolineAnim(Animator an){
        an.SetBool("isJump", true);
        yield return new WaitForSeconds(0.5f);
        an.SetBool("isJump", false);
    }

    IEnumerator NoHit()
    {
        gemCount++;
        blueGem.SetActive(true);
        CheckGems(blueGem);

        canHit = false;
        blueGem.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f , 1f);
        yield return new WaitForSeconds(4f);
        StartCoroutine(Invis(blueGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        canHit = true;

        gemCount--;
        blueGem.SetActive(false);
        CheckGems(greenGem);
    }

    IEnumerator SpeedBoost() 
    {
        gemCount++;
        greenGem.SetActive(true);
        CheckGems(greenGem);



        speed = speed * 2;
        greenGem.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        yield return new WaitForSeconds(4f);
        StartCoroutine(Invis(greenGem.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        speed = speed / 2;

        gemCount --;
        greenGem.SetActive(false);
        CheckGems(blueGem);
    }

    void CheckGems(GameObject obj){
        if(gemCount == 1){
            obj.transform.localPosition = new Vector3(0f, 2.4f, obj.transform.localPosition.z);
        }else if(gemCount == 2){
            blueGem.transform.localPosition = new Vector3(-0.07f, 2.4f, blueGem.transform.localPosition.z);
            greenGem.transform.localPosition = new Vector3(1.12f, 2.4f, greenGem.transform.localPosition.z);
        }
    }

    IEnumerator Invis(SpriteRenderer spr, float time){
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if(spr.color.a > 0){
            StartCoroutine(Invis(spr, time));
        }
    }

    public int GetCoins(){
        return coins;
    }

    public int GetHP(){
        return curHp;
    }

    public void Greengem(){
        StartCoroutine(NoHit());
    }

    public void Bluegem(){
        StartCoroutine(SpeedBoost());
    }
    IEnumerator DevelopersTool()
    {
        speed = speed * 2;
        canHit = false;
        jumpHeight = jumpHeight * 2;

        yield return new WaitForSeconds(10f);
        yield return new WaitForSeconds(1f);

        speed = speed / 2;
        jumpHeight = jumpHeight / 2;
        canHit = true;

    }
    public void Dev(){
        StartCoroutine(DevelopersTool());
    }
}
