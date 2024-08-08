using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private int[] phasePercents;
    private bool[] useSkill;
    private Object thisObject;

    private void Awake()
    {
        thisObject = GetComponent<Object>();

        useSkill = new bool[phasePercents.Length];
        for (int i = 0; i < useSkill.Length; i++) useSkill[i] = false;
    }
    public bool UseSkill()
    {
        float hpPercent = thisObject.HP / thisObject.MaxHp * 100;
        for(int i = phasePercents.Length; i > 0; --i)
        {
            if(hpPercent <= phasePercents[i] && !useSkill[i])
            {
                useSkill[i] = true;
                Skill(i);
                return true;
            }
        }
        return false;
    }

    private void Skill(int phase)
    {

    }
}
