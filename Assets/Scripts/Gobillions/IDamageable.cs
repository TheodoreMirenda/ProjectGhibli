using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gobillions
{
public interface IDamageable 
{
    void TakeHit(float damage, RaycastHit hit);
    void TakeDamage(float damage);
}
}
