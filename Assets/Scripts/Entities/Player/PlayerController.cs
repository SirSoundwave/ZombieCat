using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : EntityBase {
    
    private PlayerMovement playerMovement;
    private PlayerDecay playerDecay;
    private PlayerAnimation playerAnimation;
    private PlayerEat playerEat;
    private PlayerAttack playerAttack;

    public Slider healthBar;

    public void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerDecay = GetComponent<PlayerDecay>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerEat = GetComponent<PlayerEat>();
        playerAttack = GetComponent<PlayerAttack>();
        
    }

    public void Update()
    {
        base.Update();
        if (Time.time >= playerAttack.nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerAttack.Melee();
                playerAttack.nextAttackTime = Time.time + 1f / playerAttack.attackRate;
            }
        }


        CheckDecayState();

        healthBar.value = health / maxHealth;

        if (health < 1)
        {
            onDeath();
        }

    }
    public override void Move()
    {
        /* Directional Input */
        float moveX = Input.GetAxisRaw("Horizontal");


        /* Walking */
        playerMovement.Walk(moveX);
        if (Time.time >= playerAttack.nextAttackTime)
        {
            if (moveX == 0 && playerMovement.body.velocity.y == 0)
            {
                playerAnimation.idleAnimation(playerDecay.currentState);
            }

            else if (playerMovement.body.velocity.y == 0)
            {
                playerAnimation.walkAnimation(playerDecay.currentState);
            }

            else if (playerMovement.body.velocity.y < 0)
            {
                playerAnimation.fallAnimation(playerDecay.currentState);
            }
        }


        /* Jumping */
        if (Input.GetButtonDown("Jump")) {

            playerMovement.Jump();

        }

        /* Launch */
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            playerMovement.Launch();
        }

        
        
        

    }

    void CheckDecayState()
    {

        if (playerEat.needsHeal)
        {
            playerEat.needsHeal = false;

            playerMovement.jumpSpeed = 18;
            playerMovement.walkSpeed = 6;

            playerDecay.Heal();
            health = maxHealth;
        }



        if (health < 75)
        {
            if (playerDecay.currentState == 0)
            {
                playerDecay.LoseLegOne(playerMovement);
            }
        }
        if (health < 50)
        {
            if (playerDecay.currentState == 1)
            {
                playerDecay.LoseEarOne(playerMovement);
            }
        }
        if (health < 25)
        {
            if (playerDecay.currentState == 2)
            {
                playerDecay.LoseLegTwo(playerMovement);
            }

            if (health < 15)
            {
                if (playerDecay.currentState == 3)
                {
                    playerDecay.LoseEarTwo(playerMovement);
                }
            }
            if (health < 10)
            {
                if (playerDecay.currentState == 4)
                {
                    playerDecay.LoseEyeOne(playerMovement);
                }
            }
            if (health <= 0)
            {
                onDeath();
            }

        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Death")
        {
            onDeath();
        }
    }

    public override void onDeath()
    {
        Debug.Log("you died");
    }

}
