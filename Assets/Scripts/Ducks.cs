using UnityEngine;

[CreateAssetMenu(fileName = "Ducks", menuName = "Scriptable Objects/Ducks")]
public class Ducks : ScriptableObject
{
   public int scoreNeeded;
   public int scoreIncrease;
   public string Duckanim;
    public Sprite staticSprite;
}
