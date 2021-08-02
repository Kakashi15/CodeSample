using System;
using UnityEngine;


public enum PlayerType
{
    Phoebee,
    Jay
}

public enum FootStepType
{
    Walk = 0,
    Run = 1
}

public enum AttackDirection
{
    Nuetral,
    Forward,
    Down,
    Up
}

[Serializable]
public struct PlayerAudioData
{
    [Header("FootSteps Clips")]
    public AudioData SandWalkSteps;
    public AudioData SandRunSteps;
    public AudioData TarpsWalkSteps;
    public AudioData TarpsRunSteps;
    public AudioData TracksWalkSteps;
    public AudioData TracksRunSteps;
    public AudioData WoodWalkSteps;
    public AudioData WoodRunSteps;
    public AudioData StoneWalkSteps;
    public AudioData StoneRunSteps;
    public AudioData MineWalkSteps;
    public AudioData MineRunSteps;

    [Header("Landing Clips")]
    public AudioData LandingSFX_Mine;
    public AudioData LandingSFX_Sand;
    public AudioData LandingSFX_Stone;
    public AudioData LandingSFX_Tarp;
    public AudioData LandingSFX_Tracks;

    public AudioData WallClimb;
    public AudioData WallRideLoop;
    //public AudioClip hardLanding;

    [Header("Light Ground Attacks Clips")]
    public AudioData LightAttackNuetral;
    public AudioData LightAttackForward;
    public AudioData LightAttackDown;
    public AudioData LightAttackUp;

    [Header("Light Ground  Attacks No Hit Clips")]
    public AudioData LightAttackNuetral_NoHit;
    public AudioData LightAttackForward_NoHit;
    public AudioData LightAttackDown_NoHit;
    public AudioData LightAttackUp_NoHit;

    [Header("Light Air Attacks Clips")]
    public AudioData LightAirAttackNuetral;
    public AudioData LightAirAttackForward;
    public AudioData LightAirAttackDown;
    public AudioData LightAirAttackUp;

    [Header("Light Air Attacks No hit Clips")]
    public AudioData LightAirAttackNuetral_NoHit;
    public AudioData LightAirAttackForward_NoHit;
    public AudioData LightAirAttackDown_NoHit;
    public AudioData LightAirAttackUp_NoHit;

    [Header("Light Running Attacks Clips")]
    public AudioData LightRunningAttackNuetral;
    public AudioData LightRunningAttackForward;
    public AudioData LightRunningAttackDown;
    public AudioData LightRunningAttackUp;

    [Header("Light Running Attacks No hit Clips")]
    public AudioData LightRunningAttackNuetral_NoHit;
    public AudioData LightRunningAttackForward_NoHit;
    public AudioData LightRunningAttackDown_NoHit;
    public AudioData LightRunningAttackUp_NoHit;

    [Header("Special Ground Attacks Clips")]
    public AudioData SpecialAttackNuetral;
    public AudioData SpecialAttackForward;
    public AudioData SpecialAttackDown;
    public AudioData SpecialAttackUp;

    [Header("Special Ground Attacks Not Hit Clips")]
    public AudioData SpecialAttackNuetral_NoHit;
    public AudioData SpecialAttackForward_NoHit;
    public AudioData SpecialAttackDown_NoHit;
    public AudioData SpecialAttackUp_NoHit;

    [Header("Special Air Attacks Clips")]
    public AudioData SpecialAirAttackNuetral;
    public AudioData SpecialAirAttackForward;
    public AudioData SpecialAirAttackDown;
    public AudioData SpecialAirAttackUp;

    [Header("Special Air Attacks No Hit Clips")]
    public AudioData SpecialAirAttackNuetral_NoHit;
    public AudioData SpecialAirAttackForward_NoHit;
    public AudioData SpecialAirAttackDown_NoHit;
    public AudioData SpecialAirAttackUp_NoHit;

    [Header("Special Running Attacks Clips")]
    public AudioData SpecialRunningAttackNuetral;
    public AudioData SpecialRunningAttackForward;
    public AudioData SpecialRunningAttackDown;
    public AudioData SpecialRunningAttackUp;

    [Header("Special Running Attacks No Hit Clips")]
    public AudioData SpecialRunningAttackNuetral_NoHit;
    public AudioData SpecialRunningAttackForward_NoHit;
    public AudioData SpecialRunningAttackDown_NoHit;
    public AudioData SpecialRunningAttackUp_NoHit;

    public AudioData jump;
}
public class CharacterAudio : MonoBehaviour
{
    [SerializeField] Entity phoebeeInstance;
    [SerializeField] Entity jayInstance;

    [SerializeField] PlayerAudioData PhoebeeAudioData;
    [SerializeField] PlayerAudioData JayAudioData;

    [Header("Phoebee Steps")]
    [SerializeField]
    [Tooltip("Interval At which Walk FootStep audio is played")]
    float PhoebeeWalkingStepsTimeGap = 15;
    [SerializeField]
    [Tooltip("Interval At which Running FootStep audio is played")]
    float PhoebeeRunningStepsTimeGap = 30;
    [SerializeField]
    [Tooltip("FirsStep Audio Play wait Time")]
    float PhoebeeFirstStepGap = 1f;

    [Header("Jay Steps")]
    [SerializeField]
    [Tooltip("Interval At which Walk FootStep audio is played")]
    float JayWalkingStepsTimeGap = 15;
    [SerializeField]
    [Tooltip("Interval At which Running FootStep audio is played")]
    float JayRunningStepsTimeGap = 30;
    [SerializeField]
    [Tooltip("FirsStep Audio Play wait Time")]
    float JayFirstStepGap = 1f;


    private float stepTimeCheck;

    private float stepsTimer;

    private float attackAudioTimer;

    private void Start()
    {

    }

    /// <summary>
    /// Play FootSteps Audio
    /// </summary>
    /// <param name="groundType">Ground Type like Sand, Stone, Tracks etc</param>
    /// <param name="footStepType">Type of Footstep (Run or Walk)</param>
    /// <param name="playerType">Player Type (Phoebee or Jay)</param>
    /// <param name="moveSpeed">Player Current Velocity</param>
    public void PlaySteps(GroundType groundType, FootStepType footStepType, PlayerType playerType, float moveSpeed)
    {
        if (groundType == GroundType.None)
            return;
        AudioData steps;
        Entity currentEntity;

        switch (groundType)
        {
            case GroundType.Sand:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.SandWalkSteps : PhoebeeAudioData.SandRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.SandWalkSteps : JayAudioData.SandRunSteps;
                break;
            case GroundType.Tarps:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.TarpsWalkSteps : PhoebeeAudioData.TarpsRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.TarpsWalkSteps : JayAudioData.TarpsRunSteps;
                break;
            case GroundType.Tracks:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.TracksWalkSteps : PhoebeeAudioData.TracksRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.TracksWalkSteps : JayAudioData.TracksRunSteps;
                break;
            case GroundType.Wood:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.WoodWalkSteps : PhoebeeAudioData.WoodRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.WoodWalkSteps : JayAudioData.WoodRunSteps;
                break;
            case GroundType.Stone:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.StoneWalkSteps : PhoebeeAudioData.StoneRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.StoneWalkSteps : JayAudioData.StoneRunSteps;
                break;
            case GroundType.Mine:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.MineWalkSteps : PhoebeeAudioData.MineRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.MineWalkSteps : JayAudioData.MineRunSteps;
                break;
            default:
                if (playerType == PlayerType.Phoebee)
                    steps = footStepType == FootStepType.Walk ? PhoebeeAudioData.SandWalkSteps : PhoebeeAudioData.SandRunSteps;
                else
                    steps = footStepType == FootStepType.Walk ? JayAudioData.SandWalkSteps : JayAudioData.SandRunSteps;
                break;
        }

        stepsTimer += Time.deltaTime * Mathf.Abs(moveSpeed);
        //Debug.Log("Steps TImer: " + moveSpeed);

        float currentWalkingPlayerStepsTimeGap = 0f;
        float currentPlayerFirstStepGap = 0f;
        float currentPlayerRunningStepsGap = 0f;

        switch (playerType)
        {
            case PlayerType.Phoebee:
                currentEntity = phoebeeInstance;
                currentWalkingPlayerStepsTimeGap = PhoebeeWalkingStepsTimeGap;
                currentPlayerFirstStepGap = PhoebeeFirstStepGap;
                currentPlayerRunningStepsGap = PhoebeeRunningStepsTimeGap;
                break;
            case PlayerType.Jay:
                currentEntity = jayInstance;
                currentWalkingPlayerStepsTimeGap = JayWalkingStepsTimeGap;
                currentPlayerFirstStepGap = JayFirstStepGap;
                currentPlayerRunningStepsGap = JayRunningStepsTimeGap;
                break;
            default:
                currentEntity = phoebeeInstance;
                currentWalkingPlayerStepsTimeGap = PhoebeeWalkingStepsTimeGap;
                currentPlayerFirstStepGap = PhoebeeFirstStepGap;
                currentPlayerRunningStepsGap = PhoebeeRunningStepsTimeGap;
                break;

        }

        if (footStepType.Equals(FootStepType.Walk))
            stepTimeCheck = Mathf.Abs(moveSpeed) > currentWalkingPlayerStepsTimeGap ? currentWalkingPlayerStepsTimeGap : currentPlayerFirstStepGap;
        else
            stepTimeCheck = Mathf.Abs(moveSpeed) > currentPlayerRunningStepsGap ? currentPlayerRunningStepsGap : currentPlayerFirstStepGap;

        if (stepsTimer > stepTimeCheck)
        {
            steps?.Play(currentEntity.gameObject.transform);
            stepsTimer = 0;
        }
    }

    /// <summary>
    /// Play Wall Climb Audio
    /// </summary>
    /// <param name="groundType">Ground Type like Sand, Stone, Tracks etc</param>
    /// <param name="playerType">Player Type (Phoebee or Jay)</param>
    public void PlayerWallClimb(PlayerType playerType)
    {
        if (playerType == PlayerType.Phoebee)
        {
            PhoebeeAudioData.WallClimb?.Play(phoebeeInstance.gameObject.transform);
        }
        else
        {
            JayAudioData.WallClimb?.Play(jayInstance.gameObject.transform);
        }
    }

    /// <summary>
    /// Play Wall Ride Loop Audio
    /// </summary>
    /// <param name="groundType">Ground Type like Sand, Stone, Tracks etc</param>
    /// <param name="playerType">Player Type (Phoebee or Jay)</param>
    public void PlayerWallRideLoop(PlayerType playerType)
    {
        if (playerType == PlayerType.Phoebee)
        {
            StopPlayerWallRideLoop(PlayerType.Jay);
            PhoebeeAudioData.WallRideLoop?.Play(phoebeeInstance.gameObject.transform);
        }
        else
        {
            StopPlayerWallRideLoop(PlayerType.Phoebee);
            JayAudioData.WallRideLoop?.Play(jayInstance.gameObject.transform);
        }
    }

    /// <summary>
    /// Stop Wall Ride Loop
    /// </summary>
    /// <param name="playerType">Player Type (Phoebee or Jay)</param>
    public void StopPlayerWallRideLoop(PlayerType playerType)
    {
        if (playerType == PlayerType.Phoebee)
        {
            PhoebeeAudioData.WallRideLoop?.StopControllerSound();
        }
        else
        {
            JayAudioData.WallRideLoop?.StopControllerSound();
        }
    }

    /// <summary>
    /// Play Jump Audio
    /// </summary>
    /// <param name="playerType">Player Type (Phoebee or Jay)</param>
    public void PlayJump(PlayerType playerType)
    {
        if (playerType == PlayerType.Phoebee)
            PhoebeeAudioData.jump?.Play(phoebeeInstance.gameObject.transform);
        else
            JayAudioData.jump?.Play(jayInstance.gameObject.transform);
    }

    /// <summary>
    /// Play Landing Audio
    /// </summary>
    /// <param name="groundType">Ground Type like Sand, Stone, Tracks etc</param>
    /// <param name="playerType">Player Type (Phoebee or Jay)</param>
    public void PlayLanding(GroundType groundType, PlayerType playerType)
    {
        if (groundType == GroundType.None)
            return;

        AudioData currentLandingData;
        switch (groundType)
        {
            case GroundType.Sand:
                if (playerType == PlayerType.Phoebee)
                    currentLandingData = PhoebeeAudioData.LandingSFX_Sand;
                else
                    currentLandingData = JayAudioData.LandingSFX_Sand;
                break;
            case GroundType.Tarps:
                if (playerType == PlayerType.Phoebee)
                    currentLandingData = PhoebeeAudioData.LandingSFX_Tarp;
                else
                    currentLandingData = JayAudioData.LandingSFX_Tarp;
                break;
            case GroundType.Tracks:
                if (playerType == PlayerType.Phoebee)
                    currentLandingData = PhoebeeAudioData.LandingSFX_Tracks;
                else
                    currentLandingData = JayAudioData.LandingSFX_Tracks;
                break;
            case GroundType.Stone:
                if (playerType == PlayerType.Phoebee)
                    currentLandingData = PhoebeeAudioData.LandingSFX_Stone;
                else
                    currentLandingData = JayAudioData.LandingSFX_Stone;
                break;
            case GroundType.Mine:
                if (playerType == PlayerType.Phoebee)
                    currentLandingData = PhoebeeAudioData.LandingSFX_Mine;
                else
                    currentLandingData = JayAudioData.LandingSFX_Mine;
                break;
            default:
                if (playerType == PlayerType.Phoebee)
                    currentLandingData = PhoebeeAudioData.LandingSFX_Sand;
                else
                    currentLandingData = JayAudioData.LandingSFX_Sand;
                break;
        }

        if (playerType == PlayerType.Phoebee)
        {
            currentLandingData?.Play(phoebeeInstance.gameObject.transform);
        }
        else
        {
            currentLandingData?.Play(jayInstance.gameObject.transform);
        }
    }

    /// <summary>
    /// Play Attack Audio
    /// </summary>
    /// <param name="playerType">Player Type(Phoebee or Jay)</param>
    /// <param name="attackOrientationType">Attack Orientation like Ground, Air, Special Ground, Special Air etc</param>
    /// <param name="attackDirection">Direction Attack</param>
    /// <param name="attackComboIndex">Combo Index in case of Light attacks</param>
    /// <param name="delay">Delay after which audio should be played</param>
    /// <param name="attackHit">Check for if the attack is hitting an Entity</param>
    public void PlayAttack(PlayerType playerType, AttackOrientationType attackOrientationType, AttackDirection attackDirection, int attackComboIndex, float delay, bool attackHit)
    {
        PlayerAudioData currentPlayerAudioData;
        AudioData currentAttackDirectionClips;
        Entity currentEntity;

        if (playerType == PlayerType.Phoebee)
        {
            currentEntity = phoebeeInstance;
            currentPlayerAudioData = PhoebeeAudioData;
        }
        else
        {
            currentEntity = jayInstance;
            currentPlayerAudioData = JayAudioData;
        }

        switch (attackOrientationType)
        {
            case AttackOrientationType.Light_Ground:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackNuetral
                            : currentPlayerAudioData.LightAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackForward
                            : currentPlayerAudioData.LightAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackDown
                            : currentPlayerAudioData.LightAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackUp
                            : currentPlayerAudioData.LightAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackNuetral
                            : currentPlayerAudioData.LightAttackNuetral_NoHit;
                        break;
                }
                break;
            case AttackOrientationType.Light_Air:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAirAttackNuetral
                            : currentPlayerAudioData.LightAirAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAirAttackForward
                            : currentPlayerAudioData.LightAirAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAirAttackDown
                            : currentPlayerAudioData.LightAirAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAirAttackUp
                            : currentPlayerAudioData.LightAirAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAirAttackNuetral
                            : currentPlayerAudioData.LightAirAttackNuetral_NoHit;
                        break;
                }
                break;
            case AttackOrientationType.Light_Running:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightRunningAttackNuetral
                            : currentPlayerAudioData.LightRunningAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightRunningAttackForward
                            : currentPlayerAudioData.LightRunningAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightRunningAttackDown
                            : currentPlayerAudioData.LightRunningAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightRunningAttackUp
                            : currentPlayerAudioData.LightRunningAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightRunningAttackNuetral
                            : currentPlayerAudioData.LightRunningAttackNuetral_NoHit;
                        break;
                }
                break;
            case AttackOrientationType.Special_Ground:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAttackNuetral
                            : currentPlayerAudioData.SpecialAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAttackForward
                            : currentPlayerAudioData.SpecialAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAttackDown
                            : currentPlayerAudioData.SpecialAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAttackUp
                            : currentPlayerAudioData.SpecialAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAttackNuetral
                            : currentPlayerAudioData.SpecialAttackNuetral_NoHit;
                        break;
                }
                break;
            case AttackOrientationType.Special_Air:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAirAttackNuetral
                            : currentPlayerAudioData.SpecialAirAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAirAttackForward
                            : currentPlayerAudioData.SpecialAirAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAirAttackDown
                            : currentPlayerAudioData.SpecialAirAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAirAttackUp
                            : currentPlayerAudioData.SpecialAirAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialAirAttackNuetral
                            : currentPlayerAudioData.SpecialAirAttackNuetral_NoHit;
                        break;
                }
                break;
            case AttackOrientationType.Special_Running:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialRunningAttackNuetral
                            : currentPlayerAudioData.SpecialRunningAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialRunningAttackForward
                            : currentPlayerAudioData.SpecialRunningAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialRunningAttackDown
                            : currentPlayerAudioData.SpecialRunningAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialRunningAttackUp
                            : currentPlayerAudioData.SpecialRunningAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.SpecialRunningAttackNuetral
                            : currentPlayerAudioData.SpecialRunningAttackNuetral_NoHit;
                        break;
                }
                break;
            default:
                switch (attackDirection)
                {
                    case AttackDirection.Nuetral:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackNuetral
                            : currentPlayerAudioData.LightAttackNuetral_NoHit;
                        break;
                    case AttackDirection.Forward:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackForward
                            : currentPlayerAudioData.LightAttackForward_NoHit;
                        break;
                    case AttackDirection.Down:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackDown
                            : currentPlayerAudioData.LightAttackDown_NoHit;
                        break;
                    case AttackDirection.Up:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackUp
                            : currentPlayerAudioData.LightAttackUp_NoHit;
                        break;
                    default:
                        currentAttackDirectionClips = attackHit ? currentPlayerAudioData.LightAttackNuetral
                            : currentPlayerAudioData.LightAttackNuetral_NoHit;
                        break;
                }
                break;
        }

        if (delay > 0)
        {
            attackAudioTimer += Time.deltaTime;
            if (attackAudioTimer > delay)
            {
                Debug.Log("Attack COmbo Index: " + (attackComboIndex > 0 ? attackComboIndex - 1 : 0));
                currentAttackDirectionClips?.Play(PlayerTagManager.Instance.ActiveCharacter().transform, attackComboIndex - 1);
                attackAudioTimer = 0;
            }
        }

        int soundClipIndex = (attackComboIndex > 0 ? attackComboIndex - 1 : 0);

        Debug.Log("Attack Combo Index Raw: " + attackComboIndex);
        Debug.Log("Attack Combo Index: " + soundClipIndex);
        Debug.Log("Attack orientation " + attackOrientationType);
        if (currentAttackDirectionClips)
        {
            if (soundClipIndex >= currentAttackDirectionClips.Sounds.Count)
            {
                Debug.Log("<color=red> The Sound clip you are trying to access in not assigned in the Audio Data</color>");
            }
            else
                currentAttackDirectionClips?.Play(currentEntity.gameObject.transform, soundClipIndex);
        }
        else
            Debug.Log("<color=red> Audio Data for sound missing </color>");
    }
}
