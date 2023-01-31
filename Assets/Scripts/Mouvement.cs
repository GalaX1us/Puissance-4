using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouvement : MonoBehaviour
{
    [SerializeField] // permet d'accéder aux variables dans l'Inspector de Unity
    float speed = 10f;
    public Vector3 targetPostion;

    // Fonction appelée à chaque frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPostion, step);
    }
}
