using UnityEngine;

[CreateAssetMenu(fileName = "Ducks", menuName = "Scriptable Objects/Ducks")]
public class Ducks : ScriptableObject
{
   public int scoreNeeded;
   public float scoreMultiplier;
   public AnimationClip DuckSprites;
    public Sprite staticSprite;
}
