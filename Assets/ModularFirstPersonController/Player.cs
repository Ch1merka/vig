using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
[SerializeField] Animator anim;
 bool isReload;
 private void Update()
 {
     if (Input.GetKeyDown(KeyCode.R))
     {
         anim.SetTrigger("reload");
         isReload = true;
         Invoke("StopReload", 3.75f);
     }
     if (Input.GetMouseButtonDown(0) && !isReload)
     {
         //стрелять
     }
 }
 public void StopReload()
 {
     isReload = false;
 }
}
