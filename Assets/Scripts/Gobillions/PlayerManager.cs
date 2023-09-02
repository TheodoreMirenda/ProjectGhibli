using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gobillions
{
public class PlayerManager : LivingEntity, IDamageable
{
    
	public static PlayerManager instance;
    [SerializeField] public float attackRange, attackCooldown, rateOfFire, muzzleVelocity, damage, projectilePiercing, critChance, critDamage;
    [SerializeField] public Transform firepoint;

    [Header("Prefabs")]
    [SerializeField] public Transform projectilePrefab;
    Collider[] hitColliders = new Collider[0];
    private void Awake()
    {
        if(instance==null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // DontDestroyOnLoad(gameObject);

        // inputHandler = GetComponent<InputHandler>();
        // playerMovement = GetComponent<PlayerMovement>();
        // playerStats = GetComponent<PlayerStats>();
        // weaponController = GetComponent<WeaponController>();
        // playerRigidbody = GetComponent<Rigidbody>();
        // cyberwareManager = GetComponent<CyberwareManager>();

        // expShardAsLayerMask = (1 << expShardLayer);

        LivingEntity player = GetComponent<LivingEntity>();
        // player.OnDamage += PlayerTakeDamage;
        this.OnDeath += OnPlayerDeath;

        // viewCamera = Camera.main;
    }

    private void SeachForEnemies() {
        if(attackCooldown > 0) {
            attackCooldown -= Time.deltaTime;
            return;
        }
        attackCooldown = rateOfFire;
        
        Physics.OverlapSphereNonAlloc(transform.position, attackRange, hitColliders);
        if(hitColliders.Length==0)
            return;
            
        Debug.Log($"Found {hitColliders.Length} colliders");
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.gameObject.CompareTag("Enemy")) {
                Debug.Log($"Enemy found: {hitCollider.gameObject.name}");
                Fire(hitCollider.gameObject.GetComponent<Enemy>());
            }
        }
    }
    private void Fire(Enemy enemy) {
        // Quaternion newrotation = PlayerManager.instance.transform.rotation;
        // newrotation *= Quaternion.Euler(0, offset, 0);
        Projectile newProjectile = Instantiate(projectilePrefab, firepoint.transform.position, Quaternion.identity).GetComponent<Projectile>();

        newProjectile.SetStats(enemy, muzzleVelocity, damage, (int)projectilePiercing, critChance, critDamage);
    }
    protected override void Start()
    {
        // startingHealth = playerStats.BaseHealth;
        base.Start();
        // WeaponManager.instance.LoadWeaponsUnlocked();
    }
    void Update()
    {
        if(dead)
            return;

        SeachForEnemies();
        float delta = Time.deltaTime;
        // inputHandler.TickInput(delta);
        // playerRigidbody.angularVelocity = Vector3.zero;

        // if(Time.timeScale==0)
        //     return;

        // invulnerable = playerMovement.animator.GetBool("invulnerable");
        // isInteracting = playerMovement.animator.GetBool("isInteracting");
        // playerMovement.RechargeRollCharges(delta);
        // uIManager.timerText.text = FormatTime(Time.time);

        // if(isInteracting)
        //     return;

        // playerMovement.HandleRolling();
        // LookAtCursor();
        // PickUpExp();
        // PullInExp();
    }
    public void StartGame()
    {
        // spawner.NextWave();
    }

    // private void LookAtCursor()
    // {
    //     //Ray ray = viewCamera.ScreenPointToRay(inputHandler.cameraInput);
    //     // RaycastHit hit;
    //     // if(Physics.Raycast(ray, out hit, 100))
    //     // {
    //     //     playerMovement.LookAt(hit.transform.position);
    //     // }
    //     Ray ray = viewCamera.ScreenPointToRay(inputHandler.cameraInput);
    //     Vector3 m_DistanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 12, Camera.main.transform.position.z + 4);

    //     //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
    //     Plane groundPlane = new Plane(Vector3.up, m_DistanceFromCamera);
    //     float rayDistance;

    //     if(groundPlane.Raycast(ray, out rayDistance))
    //     {
    //         Vector3 point = ray.GetPoint(rayDistance);
    //         Debug.DrawLine(ray.origin, point, Color.green);
    //         playerMovement.LookAt(point);
    //     }
    // }
    
    // private void PlayerTakeDamage(float damage)
    // {
    //     if(invulnerable)
    //     {
    //         Debug.Log($"Player invulnerable, no damage");
    //         return;
    //     }
    //     float mitigatedDamage = damage - (damage*(playerStats.Armor/100f));
    //     health -= mitigatedDamage;
    //     // Debug.Log($"Player took {mitigatedDamage} damage");

    //     if(health<=0&&!dead){
    //         Die();
    //     }
    // }
    
    // private void PickUpExp()
    // {
    //     float range = 0f;
    //     Vector3 p1 = transform.position;
    //     float sphereCastRadius = playerStats.PickUpRadius;
    //     RaycastHit[] hitObjects = Physics.SphereCastAll(p1, sphereCastRadius, transform.forward, range, expShardAsLayerMask);

    //     foreach(RaycastHit hit in hitObjects)
    //     {
    //         // Debug.Log($"Hit {hit.collider.gameObject}");
    //         hit.collider.gameObject.layer = 9;
    //         shards.Add(hit.collider.gameObject.transform);
    //     }
    // }
    // private void PullInExp()
    // {
    //     for (int i = shards.Count; i > 0; i--)
    //     {
    //         shards[i-1].position = Vector3.Lerp(shards[i-1].position, this.transform.position, Time.deltaTime * 5f);
    //         if(Vector3.Distance(shards[i-1].position, this.transform.position)<1)
    //         {
    //             CollectExp(shards[i-1].GetComponent<ExpShardItem>());
    //             shards.RemoveAt(i-1);
    //         }
    //     }
    // }
    // private void CollectExp(ExpShardItem expShardItem)
    // {
    //     playerStats.GainExp(expShardItem.expShard.expAmount * playerStats.ExpMultiplier);
    //     Destroy(expShardItem.gameObject);
    //     IAudioRequester.instance.PlaySFX("pickUpExp");
    // }
    
    public string FormatTime( float time )
    {
        int minutes = (int) time / 60 ;
        int seconds = (int) time - 60 * minutes;
        int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds );
    }
    private void OnPlayerDeath()
    {
        // playerMovement.animator.Play("Death");
        // playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        // float rewardAmount = RewardsManager.instance.CalculateRewardAmount(Time.time);
        // GameManager.instance.saveSystem.RecordRecentGame((int)rewardAmount, Time.time, enemiesSlain, (int)playerStats.expLevel.levelNumber);
        // StartCoroutine(uIManager.EndGameUI());
    }
    public void Heal(float amount)
    {
        health += amount;
        // if(health > startingHealth * playerStats.BonusHealth/100)
        //     health = startingHealth * playerStats.BonusHealth/100;
    }
}
}