using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatInputData", menuName = "MMG_Player/Data/CombatInputData")]
public class CombatInputData : ScriptableObject
{
    private bool canCastFire;
    private bool castFire;

    private bool canCreateWall;
    private bool startWallPlace;
    private bool createWall;

    private bool isThirdRelic;

    private bool switchAbility;

    private bool cancelWall;

    private bool swingSword;

    public bool CastFire
    {
        get => castFire;
        set => castFire = value;
    }

    public bool CanCastFire
    {
        get => canCastFire;
        set => canCastFire = value;
    }

    public bool StartWallPlace
    {
        get => startWallPlace;
        set => startWallPlace = value;
    }

    public bool CreateWall
    {
        get => createWall;
        set => createWall = value;
    }

    public bool CanCreateWall
    {
        get => canCreateWall;
        set => canCreateWall = value;
    }

    public bool IsThirdRelic
    {
        get => isThirdRelic;
        set => isThirdRelic = value;
    }

    public bool CancelWall 
    {
        get => cancelWall; 
        set => cancelWall = value; 
    }    

    public bool SwitchAbility
    {
        get => switchAbility;
        set => switchAbility = value;
    }

    public bool SwingSword
    {
        get => swingSword;
        set => swingSword = value;
    }

}
