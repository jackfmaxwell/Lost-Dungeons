using System.Collections;
using UnityEngine;
using Mirror;

//This class handles the animations for an enemy
public class EnemyAnimationManager : NetworkBehaviour
{
    //References
    private Animator anim;
    public Enemy_Generic owner; //must be set manually

    //Visuals/Particles
    public Material whiteFlash, normalSprite;
    public GameObject damageParticles;
    public GameObject deathParticles;

    void Start()
    {
        anim = GetComponent<Animator>(); //grab animator
    }

    void FixedUpdate()
    {
        //set anim running based on desired movement of enemy
        Vector2 force = owner.desiredMovement;
        if (Mathf.Abs(force.x) > 0f)
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }
    }

    //called by enemy when attacking
    public void swordAttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
    public void bowAttackAnimation()
    {
        anim.SetTrigger("BowAttack");
    }

    //death animation to be called on die()
    public void deathAnim()
    {
        GameObject particles = GameObject.Instantiate(deathParticles);
        particles.transform.position = this.transform.position;
    }

    //damage animation to be called by enemy on take damage()
    public void damageTakeAnim(float dir)
    {
        StartCoroutine(flashWhite());
        //set particle throw direction based on direction of hit
        GameObject particles = GameObject.Instantiate(damageParticles);
        particles.transform.position = this.transform.position;

        if (dir > 0f)
        {
            particles.transform.GetChild(0).transform.localScale = new Vector3(-0.4f, 0.4f, 1f);
        }
    }
    //flashes the enemy material white when taking damage to provide visual feedback
    private IEnumerator flashWhite()
    {
        //change material
        this.GetComponent<SpriteRenderer>().material = whiteFlash;
        //wait
        yield return new WaitForSeconds(0.1f);
        //change back
        this.GetComponent<SpriteRenderer>().material = normalSprite;
    }
}
