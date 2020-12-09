using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laptop : MonoBehaviour
{

    private Animator _anim;


    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (CameraController.cameraState == CameraController.CameraState.TableView)
        {
            _anim.SetBool("isLaptopOpen", true);
        }
        else
        {
            _anim.SetBool("isLaptopOpen", false);
        }
    }
}
