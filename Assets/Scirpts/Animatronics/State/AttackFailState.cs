﻿using UnityEngine;
public class AttackFailState : IState
{
    private AnimatronicsController controller;
    private Animatronics animatronics;


    public AttackFailState(AnimatronicsController controller)
    {
        this.controller = controller;
        this.animatronics = controller.animatronics;
    }
    public void Enter()
    {
        Debug.Log("AttackFailState");
        animatronics.PlayAnimation("Shocked");
        animatronics.HpDecrease();
        animatronics.HitAnimatronicsBodyParticle();
        animatronics.PlaySound(animatronics.shockAudioClip);
        if (animatronics.HpCheck())
        {
            animatronics.ChangeGlitchBoolValue(true);
            controller.StateMachine.TransitionTo(controller.StateMachine.idleState);
        }
        else
        {
            controller.StateMachine.TransitionTo(controller.StateMachine.stopWorkState);
        }
        animatronics.SetGameOverGlitch(false);
        animatronics.GameClearOverlay();
    }

    public void Update()
    {

    }

    public void Exit()
    {
    }

}