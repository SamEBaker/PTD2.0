using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class DuckRotator : MonoBehaviour
{
    public UnityEvent ScoreIncrease;
    [SerializeField]
    Image curSprite;
    float speed;
    float speed1;
    float speed2;
    float speedIncrease = 0.5f;
    bool rotup = true;
    bool rotforward;
    bool rotright;

    // Adds rotation to sprite 
    // Listens to counter event to envoke at x number of clicks

    void Start()
    {
        speed = 0;
        StartCoroutine(RotChange());
    }

    // Update is called once per frame
    void Update()
    {
        if (rotup) curSprite.transform.Rotate(Vector3.up * speed * Time.deltaTime);
        if (rotright) curSprite.transform.Rotate(Vector3.right * speed1 * Time.deltaTime);
        if (rotforward) curSprite.transform.Rotate(-Vector3.forward * speed2 * Time.deltaTime);
    }

    public void ClickAction()
    {
        speed += speedIncrease;
        if (rotright) speed1 += speedIncrease;
        if (rotforward) speed2 += speedIncrease;
        speedIncrease += 0.000001f;
        ScoreIncrease.Invoke();
    }
    public void AdjRotation(bool rot)
    {
        rot = true;
        speedIncrease = 0;
    }
    IEnumerator RotChange()
    {
        yield return new WaitForSeconds(Random.Range(90f, 200f));
        AdjRotation(rotright);
        yield return new WaitForSeconds(Random.Range(90f, 200f));
        AdjRotation(rotforward);
    }
}
