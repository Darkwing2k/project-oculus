using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FallOnFloorBehaviour : IBehaviour
{
    GeneralBehaviour generalBehaviour;
    bool falling = false;
    bool isOnFloor = false;

    public FallOnFloorBehaviour(GeneralBehaviour generalBehaviour)
    {
        this.generalBehaviour = generalBehaviour;
        generalBehaviour.behaviourChangeLocked = true;
        generalBehaviour.lastYPos = generalBehaviour.enemy.transform.position.y;
    }

    public void execute(float timePassed)
    {
        if (!falling && !isOnFloor)
        {
            generalBehaviour.enemy.rigidbody.useGravity = true;
            generalBehaviour.updateDelegate += generalBehaviour.ProcessTurnWhileFalling;
            falling = true;
            generalBehaviour.collisionWithObject = false;
        }
        else if (!isOnFloor && generalBehaviour.isFallDownFinished())
        {
            generalBehaviour.enemy.rigidbody.useGravity = false;
            generalBehaviour.enemy.rigidbody.velocity = Vector3.zero;
            generalBehaviour.agent.baseOffset = GeneralBehaviour.defaultNavMeshBaseOffset;
            generalBehaviour.agent.enabled = true;
            generalBehaviour.isClimbingOnCeiling = false;
            generalBehaviour.behaviourChangeLocked = false;
            isOnFloor = true;
        }
    }

}

