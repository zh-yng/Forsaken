using UnityEngine;

public class Boss_Laser : Weapon
{
    protected override void Init()
    {
        //weilder = GameObject.FindGameObjectWithTag("Boss").transform;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.gameObject.tag.Equals("Player"))
        {
            Attack(other.gameObject.GetComponent<IDamageable>());
        }

    }
}
