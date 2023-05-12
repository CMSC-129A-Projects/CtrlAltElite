using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutfitChanger : MonoBehaviour
{
    [Header("Sprite to Change")]
    public SpriteRenderer bodyPart;

    [Header("Sprite to Cycle Through")]
    public List<Sprite> options = new List<Sprite>();


    public int CurrentOption = 0;

    public void NextOption()
    {
        CurrentOption++;
        if (CurrentOption >= options.Count)
        {
            CurrentOption = 0;
        }
        bodyPart.sprite = options[CurrentOption];
    }

    public void PreviousOption()
    {
        CurrentOption--;
        if (CurrentOption < 0)
        {
            CurrentOption = options.Count - 1;
        }
        bodyPart.sprite = options[CurrentOption];
    }

    public void Randomize()
    {
        CurrentOption = Random.Range(0, options.Count);
        bodyPart.sprite = options[CurrentOption];
    }


}
