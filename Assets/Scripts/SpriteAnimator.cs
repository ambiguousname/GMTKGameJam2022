using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] runRight;
    public Sprite[] runUp;
    public Sprite[] runDown;
    public Sprite[] idle;
    private Sprite[] currentAnim;
    private float currentAnimSpeed;
    public SpriteRenderer spriteRenderer;

    int tick;

    private void Awake()
    {
        currentAnim = idle;
        currentAnimSpeed = 60;
    }

    private void FixedUpdate()
    {
        tick++;
        playFrame(currentAnim, ((int)(tick * (currentAnimSpeed))) % currentAnim.Length);
        Debug.Log(((int)(tick * (currentAnimSpeed))) % currentAnim.Length);
    }

    public void playAnimation(Sprite[] animation, float speed)
    {
        //tick = 0;
        currentAnim = animation;
        currentAnimSpeed = speed;
    }

    public void playFrame(Sprite[] animation, int frame)
    {
        spriteRenderer.sprite = animation[frame];
    }

    public float map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        if(OldRange == 0)
        {
            return (NewRange / 2) + NewMin;
        } else
        {
            float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
            return(NewValue);
        }
        
    }
}
