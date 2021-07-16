using UnityEngine;
using System.Collections;
 
 public class CamRotation: MonoBehaviour {
 
     public float rotationSpeed = 10;
 
     void Update() {
         Vector3 rotation = transform.eulerAngles;
 
         rotation.y += Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime; // Standart Left-/Right Arrows and A & D Keys
 
         transform.eulerAngles = rotation;
     }
 }
