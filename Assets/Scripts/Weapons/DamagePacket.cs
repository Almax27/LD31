using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamagePacket 
{
	public DamagePacket(Killable _sender, int _damageAmount, Vector2 _knockback)
	{
		sender = _sender;
		damageAmount = _damageAmount;
		knockback = _knockback;
	}
	public Killable sender;
	public int damageAmount = 0;
	public Vector2 knockback = Vector2.zero;
}
