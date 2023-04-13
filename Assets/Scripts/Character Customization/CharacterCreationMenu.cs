using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreationMenu : MonoBehaviour
{
    public List<OutfitChanger> outfitChangers = new List<OutfitChanger>();

    public void RandomizeCharacter(){
        foreach (OutfitChanger changer in outfitChangers)
        {
            changer.Randomize();
        }
    }
}
