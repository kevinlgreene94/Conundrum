using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public HitPoints hitPoints;
    public HealthBar healthBarPrefab;
    HealthBar healthBar;
    public Inventory inventoryPrefab;
    Inventory inventory;
    public bool isAttacking = false;
    public int damageStrength;
    public int playerLives;
    Coroutine damageCoroutine;

    private void OnEnable()
    {
        ResetCharacter();
    }
    // Start is called before the first frame update
    public override void ResetCharacter()
    {
        inventory = Instantiate(inventoryPrefab);
        healthBar = Instantiate(healthBarPrefab);
        healthBar.character = this;

        hitPoints.value = startingHitPoints;
    }
    public override void KillCharacter()
    {
        base.KillCharacter();
        Destroy(healthBar.gameObject);
        Destroy(inventory.gameObject);
        
    }
    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        while (true)
        {
            hitPoints.value = hitPoints.value - damage;

            if (hitPoints.value <= float.Epsilon)
            {
                KillCharacter();
                break;
            }

            if (interval > float.Epsilon)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isAttacking = true;

        }
        else
        {
           isAttacking = false;
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            KillCharacter();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CanBePickedUp"))
        {
            Item hitObject = collision.gameObject.GetComponent<Consumable>().item;
            if (hitObject != null)
            {
                bool shouldDisappear = false;
                switch (hitObject.itemType)
                {
                    case Item.ItemType.COIN:
                        shouldDisappear = inventory.AddItem(hitObject);
                        break;
                    case Item.ItemType.HEALTH:
                        shouldDisappear = AdjustHitPoints(hitObject.quantity);
                        break;
                    default:
                        break;
                }
                if (shouldDisappear)
                {
                    collision.gameObject.SetActive(false);
                }

            }

        }


    }
    public bool AdjustHitPoints(int amount)
    {
        if (hitPoints.value < maxHitPoints)
        {
            hitPoints.value = hitPoints.value + amount;
            return true;
        }
        return false;
    }
    public void PlayerAttack(Enemy enemy)
    {
            if (isAttacking == true)
            {
                damageCoroutine = StartCoroutine(enemy.DamageCharacter(damageStrength, 1.0f));
            }
    }
    public void PlayerStopAttack(Enemy enemy)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
}
