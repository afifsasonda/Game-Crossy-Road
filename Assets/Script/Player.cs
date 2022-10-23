using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] TMP_Text stepText;
    [SerializeField] ParticleSystem dieParticles;
    [SerializeField, Range(0.01f,1f)] float moveDuration=0.2f;
    [SerializeField, Range(0.01f,1f)] float jumpHeight=0.5f;
    [SerializeField] AudioSource crashAudio;
    [SerializeField] AudioSource jumpAudio;
    private float backBoundary;
    private float leftBoundary;
    private float rightBoundary;
    [SerializeField] private int maxTravel;
    public int MaxTravel { get => maxTravel; }
    [SerializeField] private int currentTravel;
    public int CurrentTravel { get => currentTravel;}
    public bool IsDie{ get => this.enabled == false;}

    public void SetUp(int minZPos, int extent)
    {
        backBoundary = minZPos-1;
        leftBoundary = -(extent + 1);
        rightBoundary = extent +1;
    }

    private void Update() {
        //if(Input.GetKey(KeyCode.UpArrow))
        //{
        //    Debug.Log("Forward");
        //}
        //if(Input.GetKey(KeyCode.DownArrow))
        //{
        //    Debug.Log("Back");
        //}

        var moveDir = Vector3.zero;
        if(Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += new Vector3(0,0,1);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += new Vector3(0,0,-1);
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += new Vector3(1,0,0);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir += new Vector3(-1,0,0);
        }
        //if (moveDir == Vector3.zero){
        //    return;
        //}

        //if(IsJumping() == false){
        //    Jump(moveDir);
        //}

        if(moveDir != Vector3.zero && IsJumping() == false){
            Jump(moveDir);
        }
        
    }
    private void Jump(Vector3 targetDirection)
    {
        // atur rotasi 
        Vector3 targetPosition = transform.position + targetDirection;

        transform.LookAt(targetPosition);

        //cara loncat versi 1
        //transform.DOMoveY(2f,0.1f).OnComplete(() => transform.DOMoveY(0,0.1f));

        //versi loncat 2
        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHeight, moveDuration/2));
        moveSeq.Append(transform.DOMoveY(0, moveDuration/2));

        jumpAudio.Play();

        //untuk supaya animal ga bisa lewati tree
        if(targetPosition.z <= backBoundary || targetPosition.x <= leftBoundary || targetPosition.x >= rightBoundary)
        {
            return; 
        }
        if(Tree.AllPositions.Contains(targetPosition))
        {
            return;
        }

        //gerak maju/mundur/samping
        transform.DOMoveX(targetPosition.x,moveDuration);
        transform.DOMoveZ(targetPosition.z,moveDuration).OnComplete(UpdateTravel);
    }

    private void UpdateTravel()
    {
        currentTravel = (int) this.transform.position.z;
        if(currentTravel > maxTravel)
        {
            maxTravel = currentTravel;
        }

        //lakukan update pada text step
        stepText.text = "STEP: "+ maxTravel.ToString();
    }

    public bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ketika player mati maka tidak trigger lagi
        if(this.enabled == false)
        {
            return;
        }
        // di execute sekali pada frame ketika nempel pertama kali
        var car = other.GetComponent<Car>();
        if(car != null)
        {
            AnimateCrash(car);
            
        }

        if (other.tag == "car")
        {
            //Debug.Log("Hit" + other.name);
            //AnimateCrash();
        }

        

    }
    private void AnimateCrash(Car car)
    {
        //var isRight = car.transform.rotation.y == 90;

        //transform.DOMoveX(isRight ? 8 : -8, 0.2f);
        //transform.DORotate(Vector3.forward*360,0.2f).SetLoops(-1,LoopType.Restart);

        //Gepeng
        transform.DOScaleY(0.1f,0.2f);
        transform.DOScaleX(2,0.2f);
        transform.DOScaleZ(2,0.2f);
        this.enabled = false;

        //partikel saat animal ditabrak
        dieParticles.Play();
        crashAudio.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        // di execute setiap frame selama masih nempel
        //Debug.Log("stay");
    }
    
    private void OnTriggerExit(Collider other)
    {
        // di execute sekali pada frame ketika tidak nempel 
        //Debug.Log("Exit");
    }
}
