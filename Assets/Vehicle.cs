﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class Vehicle : MonoBehaviour
{
    public float TipAngle;
    protected Rigidbody MyRigidbody;

    //change this value to adjust ease of tipping
    protected readonly float _baseTipLiftForce = 0.025f;

    protected float _currentTipForce;

    private float s;

    private bool _flipped;
    private bool _rotated;
    public bool Dead;
    protected bool attached;
    protected Vehicle attaVehicle;
    float dirAngle;

    [Header("Sprites, do not use more than 4 atm")] public SpriteRenderer CurrentSprite;
    public List<Sprite> TippingSpritesList;
    private float[] AngleSpriteLevels = {2.5f, 5, 89};

    protected Axis[] AxlesList;


    public virtual void Start()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        AxlesList = GetComponentsInChildren<Axis>();
    }

    public virtual void Update()
    {
        var localSpeed = transform.InverseTransformDirection(MyRigidbody.velocity);
        s = localSpeed.z;
        s *= 2.237f;
        _currentTipForce = s * _baseTipLiftForce;

        if (TipAngle > 45)
        {
            _flipped = true;
            if (CurrentSprite.flipX)
            {
                CurrentSprite.flipX = false;
            }
        }
        else
        {
            _flipped = false;
        }
        if (TipAngle < AngleSpriteLevels[0])
        {
            CurrentSprite.sprite = TippingSpritesList[0];
        }

        else if (TipAngle >= AngleSpriteLevels[0] && TipAngle < AngleSpriteLevels[1])
        {
            CurrentSprite.sprite = TippingSpritesList[1];
        }
        else if (TipAngle >= AngleSpriteLevels[1] && TipAngle < AngleSpriteLevels[2])
        {
            CurrentSprite.sprite = TippingSpritesList[2];
        }
        else if (TipAngle >= AngleSpriteLevels[2])
        {
            CurrentSprite.sprite = TippingSpritesList[3];
        }
        if (_flipped && !_rotated)
        {
            CurrentSprite.transform.Rotate(0, 90, 0);
            _rotated = true;
        }
        else if (!_flipped && _rotated)
        {
            CurrentSprite.transform.Rotate(0, -90, 0);
            _rotated = false;
        }
    }

    public virtual void FixedUpdate()
    {
        TipAngle = Vector3.Angle(transform.up, Vector3.up);
        dirAngle = Vector3.Angle(transform.right, Vector3.up);
        if (dirAngle >= AngleSpriteLevels[2])
        {
            CurrentSprite.flipX = false;
        }
        else if (dirAngle <= AngleSpriteLevels[2])
        {
            CurrentSprite.flipX = true;
        }
        if (TipAngle > AngleSpriteLevels[1] && TipAngle < 30)
        {
            MyRigidbody.AddForceAtPosition(Vector3.up * TipAngle * _currentTipForce, Vector3.zero);
        }
        else if (TipAngle >= AngleSpriteLevels[2])
        {
            var joint = GetComponent<HingeJoint>();
            if (joint)
            {
                joint.useLimits = true;
            }

            transform.rotation.Set(0, transform.rotation.y, 0, 0);
            MyRigidbody.maxAngularVelocity = 0;
            MyRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Dead = true;
        }
    }

    public float TipAngle2 { get; set; }
}