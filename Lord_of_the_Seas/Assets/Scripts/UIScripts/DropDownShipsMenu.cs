using UnityEngine;

public class DropDownShipsMenu : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip[] animationClips;
    bool drop = true;

    public void Drop_OR_Lift_Menu()
    {
        if(drop == true)
        {
            animator.Play(animationClips[0].name);
            drop = false;
        }
        else
        {
            animator.Play(animationClips[1].name);
            drop = true;
        }
    }
}
