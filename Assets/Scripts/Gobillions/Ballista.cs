using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gobillions
{
public class Ballista : MonoBehaviour
{
    [SerializeField] float speed = 10, projectileLifeTime = 2, damage, critChance = 0, critAmount = 1;
    [SerializeField] Collider sphereCollider;
    public int projectilePiercing = 0;
    public float fireRate, currentCooldown;
    public string tagToHit;
    Enemy target;
    [SerializeField] private Transform ballistaBase, ballistaHead;

    [Header("Projectile")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform fireSpot;

    private void Update() {
        HandleCoolDown();

        if(FailsTargetChecks())
            return;

        LookAtTarget();
        FireAtTarget();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(tagToHit) && !target)
        {
            // Debug.Log($"sees goblin");
            //reset cooldown
            currentCooldown = fireRate;
            target = other.gameObject.GetComponent<Enemy>();
        }
    }
    float ballistaBaseRotation, ballistaHeadRotation;
    private void LookAtTarget(){
        
        //rotate only in the y axis
        ballistaBaseRotation = Quaternion.Slerp(ballistaBase.transform.rotation, Quaternion.LookRotation(target.transform.position - ballistaBase.transform.position), 1).eulerAngles.y;
        ballistaBase.transform.rotation = Quaternion.Euler(0, ballistaBaseRotation, 0);

        //rotate only in the x axis
        ballistaHeadRotation = Quaternion.Slerp(ballistaHead.transform.rotation, Quaternion.LookRotation(target.transform.position - ballistaHead.transform.position), 1).eulerAngles.x;
        //clamp at 20 degrees
        ballistaHead.transform.localRotation = Quaternion.Euler(ballistaHeadRotation, 0, 0);
        // ballistaHead.transform.localRotation = Quaternion.Euler( Mathf.Clamp(ballistaHeadRotation, -20, 20), 0, 0);
    }
    private void FireAtTarget(){
        if(currentCooldown > 0)
            return;

        currentCooldown = fireRate;
        Projectile newProjectile = Instantiate(projectile, fireSpot.position, fireSpot.rotation);
        newProjectile.SetStats(target, speed, damage, projectilePiercing, critChance, critAmount);
    }
    private void HandleCoolDown(){
        if(currentCooldown > 0)
            currentCooldown -= Time.deltaTime;
    }
    private bool FailsTargetChecks(){
        if(!target)
            return true;

        if(target.Health <= 0){
            FindNewTarget();
            target = null;
            return true;
        }

        return false;
    }

    //turn trigger off when enemy dies and turn it back on to get a new target
    private void FindNewTarget(){
        sphereCollider.enabled = false;
        sphereCollider.enabled = true;
    }

}
}