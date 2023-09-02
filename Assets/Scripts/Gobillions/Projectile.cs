using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gobillions
{
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10, projectileLifeTime = 2, damage, critChance = 0, critAmount = 1;
    public bool targetEnemy;
    public int projectilePiercing = 0;
    public string tagToHit;
    Enemy enemy;
    public void SetStats(Enemy _enemy, float newSpeed, float _damage, int _projectilePiercing, float _critChance, float _critAmount)
    {
        enemy = _enemy;
        speed = newSpeed;
        damage = _damage;
        projectilePiercing = _projectilePiercing;
        critChance = _critChance;
        critAmount = _critAmount;
    }
    void Update()
    {
        float bulletSpeed = speed;
        transform.Translate(bulletSpeed * Time.deltaTime * Vector3.forward);

        projectileLifeTime -= Time.deltaTime;
        
        if(projectileLifeTime<=0)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(tagToHit))
        {
            // Debug.Log($"hit");
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            float totalDamage = damage;

            if(critChance>Random.Range(0,100))
                totalDamage *= critAmount;

            damageable.TakeDamage(totalDamage);
            
            projectilePiercing-=1;

            if(projectilePiercing<0)
                Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}

}