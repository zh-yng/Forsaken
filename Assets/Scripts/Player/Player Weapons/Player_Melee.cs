using UnityEngine;

public class Player_Melee : Weapon
{
    protected override void Init()
    {
        weilder = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
       string layer = LayerMask.LayerToName(other.gameObject.layer);
       
        if (layer.Equals("Enemies"))
        {
            Attack(other.gameObject.GetComponent<IDamageable>());
        }
        // //to-do: why does this trigger later?
        // else if (other.gameObject.CompareTag("HUE Weapon") && weilder.GetComponent<PlayerStateMachine>().IsBlocking && weilder.GetComponent<PlayerStateMachine>().CanParry)
        // {
        //     Debug.Log(other.gameObject.name);
        //     weilder.GetComponent<PlayerStateMachine>().StartParry();
        // }
    }

}
