using UnityEngine;
using System.Collections;

public class AnimationTestScript : MonoBehaviour 
{
    public enum TriggerType
    {
        Move,
        Jump,
        Hit,
        Crouch
    }

    public float Speed;

    private Animator anim;

    public void SetTrigger(TriggerType type)
    {
        anim.SetTrigger(type.ToString());
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

	private void Update () 
    {
        anim.SetFloat("Speed", Speed);
	}
}
