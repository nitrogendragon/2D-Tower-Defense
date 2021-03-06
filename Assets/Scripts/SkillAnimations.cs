using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimations : MonoBehaviour
{
    public List<Animator> skillAnimators = new List<Animator>();

    public Animator GetSkillAnimation(int index)
    {
        return skillAnimators[index];
    }
}
