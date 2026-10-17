using UnityEngine;

public class VisorController : CharacterProp
{
    protected override void determineAnimation()
    {
        int walkingLocation = -1;
        int aimingLocation = -1;
        int i = 0;
        //NOTE: The aiming animation (which is currently jsut a static image) should always override the walking animation
        //this piece of code does exactly that
        foreach (CharacterStateNode characterState in characterStates)
        {
            if(characterState.characterState == CHARACTERSTATE.WALKING)
            {
                walkingLocation = i;
            }
            else if(characterState.characterState == CHARACTERSTATE.AIMING)
            {
                aimingLocation = i;
            }
            i++;
        }
        if(walkingLocation != -1 && aimingLocation != -1)
        {
            characterStates.RemoveAt(walkingLocation);
        }

        base.determineAnimation();
    }
}
