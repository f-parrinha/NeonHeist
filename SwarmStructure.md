# Swarm Structure

SWARM_TYPES: VOLT (Electric Type), BANG (Explosion Type), Ghost (Camouflage), Builder (Carries stuff) 

<br>

JOB_TYPES: HACK, SMOKE, COLLECT, EXPLOSION, SHOCK

<br>

SwarmManager (Singleton)
 - Has swarm minions
 - Chooses correct swarm minion for the job

<br>

Swarm:
 - SwarmGoal
 - Uses NavMesh
 - CharacterSounds
 - AnimatorController

<br>

SwarmGoal:
 - Enum: ATTACK, DISABLE_CAMERA, SMOKE_SCREEN
 - Position:
 - Entity:
 - Player: