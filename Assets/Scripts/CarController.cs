using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;

public class CarController : MonoBehaviour
{
    [Serializable]
    public struct Wheel
    {
        public Transform Model;
        public WheelCollider Collider;
    }

    [SerializeField] private Wheel leftFrontWheel;
    [SerializeField] private Wheel rightFrontWheel;
    [SerializeField] private Wheel leftRearWheel;
    [SerializeField] private Wheel rightRearWheel;
    [SerializeField] private float minimalAngleToDrift;
    [SerializeField] private float baseTorque;
    [SerializeField] private float brakePower;
    [SerializeField] private AnimationCurve steeringCurve;
    [SerializeField] private float maxWheelsSteerindAngle;
    [SerializeField] private float inputGasSensitivity;
    [SerializeField] private float inputSteeringSensitivity;
    [SerializeField] private float carLowSpeed; 
    [SerializeField] private float carHighSpeed;
    [SerializeField] private AudioSource carEngineSource;
    [SerializeField] private AudioSource carBreakeSource;
    [SerializeField] private AudioClip engineStartSound;
    [SerializeField] private AudioClip engineIdleSound;
    [SerializeField] private AudioClip engineLowSpeedSound;
    [SerializeField] private AudioClip engineHighSpeedSound;

    private Mybutton gasPedal;
    private Mybutton brakePedal;
    private Mybutton leftButton;
    private Mybutton rightButton;

    private float slipAngle;

    private float gasInput;
    private float brakeInput;
    private float steeringInput;
    public float speed;

    private NetworkPlayer player;
    private PhotonView photonView;
    private Rigidbody carRB;

    public void SetInputButtons(Mybutton[] inputButtons)
    {
        foreach (var button in inputButtons)
        {
            switch (button.InputType)
            {
                case InputType.Gas:
                    gasPedal = button;
                    break;
                case InputType.Brake:
                    brakePedal = button;
                    break;
                case InputType.Left:
                    leftButton = button;
                    break;
                case InputType.Right:
                    rightButton = button;
                    break;
            }
        }
    }
    private void Start()
    {
        if (PhotonNetwork.InRoom)
            photonView = GetComponent<PhotonView>();
        
        player = GetComponent<NetworkPlayer>();
        carRB = gameObject.GetComponent<Rigidbody>();
        carRB.centerOfMass = Vector3.zero;

        player.SetIsLockal(PhotonNetwork.InRoom && !photonView.IsMine);

        FindObjectOfType<GameController>().AddPlayer(player);
        PlayStarCarEngineSoundAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
            return;
        speed = carRB.velocity.magnitude;
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyWheelPositions();
        CheckIsDrift();
    }

    private async UniTask PlayStarCarEngineSoundAsync(CancellationToken token)
    {
        await UniTask.Delay(1000, cancellationToken: token);

        if (token.IsCancellationRequested)
            return;

        carEngineSource.clip = engineStartSound;
        carBreakeSource.Play();

        await UniTask.Delay(2500, cancellationToken: token);

        if (token.IsCancellationRequested)
            return;

        carEngineSource.clip = engineIdleSound;
        carEngineSource.Play();

        CheckCarSpeedAsync(token).Forget();
    }

    private async UniTask CheckCarSpeedAsync(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

        if(token.IsCancellationRequested)
            return;

        if(speed < carLowSpeed)
        {
            PlayEngineSound(engineIdleSound, token);
            return;
        }

        if(speed < carHighSpeed)
        {
            PlayEngineSound(engineLowSpeedSound, token);
            return;
        }

        PlayEngineSound(engineHighSpeedSound, token);
    }

    public void PlayEngineSound(AudioClip clip, CancellationToken token)
    {
        if (carEngineSource.clip != clip)
        {
            carEngineSource.clip = clip;
            carEngineSource.Play();
        }
        CheckCarSpeedAsync(token).Forget();
    }

    private void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");

        steeringInput = Input.GetAxis("Horizontal");
#if (UNITY_ANDROID && !UNITY_EDITOR)
        if (gasPedal.IsPressed)
        {
            gasInput += inputGasSensitivity * Time.deltaTime;
        }
        else if (brakePedal.IsPressed)
        {
            gasInput -= inputGasSensitivity * Time.deltaTime;
        }
        else
        {
            gasInput = 0;
        }

        if (rightButton.IsPressed)
        {
            steeringInput += inputSteeringSensitivity * Time.deltaTime;
        }
        else if (leftButton.IsPressed)
        {
            steeringInput -= inputSteeringSensitivity * Time.deltaTime;
        }
        else
        {
            steeringInput = 0;
        }
#endif
        slipAngle = Vector3.Angle(transform.forward, carRB.velocity-transform.forward);

        float movingDirection = Vector3.Dot(transform.forward, carRB.velocity);

        if (Mathf.Abs(gasInput) == 0 || Input.GetKey(KeyCode.Space) 
            || (movingDirection > 0.5f && gasInput < 0)
            || (movingDirection < -0.5f && gasInput > 0))
        {
            brakeInput = brakePower;
            return;
        }

        brakeInput = 0;
    }

    private void ApplyBrake()
    {
        leftFrontWheel.Collider.brakeTorque = brakeInput * 0.7f;
        rightFrontWheel.Collider.brakeTorque = brakeInput * 0.7f;

        leftRearWheel.Collider.brakeTorque = brakeInput * brakePower * 0.3f;
        rightRearWheel.Collider.brakeTorque = brakeInput * brakePower * 0.3f;
    }

    private void ApplyMotor() 
    {
        if (!player.InGame)
            return;

        leftFrontWheel.Collider.motorTorque = baseTorque * gasInput;
        rightFrontWheel.Collider.motorTorque = baseTorque * gasInput;
    }

    private void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);

        if (slipAngle < 120f)
        {
            steeringAngle += GetSignedAngle();
        }

        steeringAngle = Mathf.Clamp(steeringAngle,
            -maxWheelsSteerindAngle, maxWheelsSteerindAngle);
        leftFrontWheel.Collider.steerAngle = steeringAngle;
        rightFrontWheel.Collider.steerAngle = steeringAngle;
    }

    private void ApplyWheelPositions()
    {
        UpdateWheel(leftFrontWheel);
        UpdateWheel(rightFrontWheel);
        UpdateWheel(leftRearWheel);
        UpdateWheel(rightRearWheel);
    }
   
    private void UpdateWheel(Wheel wheel)
    {
        Quaternion quat;
        Vector3 position;
        wheel.Collider.GetWorldPose(out position, out quat);
        wheel.Model.position = position;
        wheel.Model.rotation = quat;
    }

    private void CheckIsDrift()
    {
        var driftAngle = Math.Abs(GetSignedAngle());

        driftAngle = driftAngle <= 90 
            ? driftAngle
            : 180 - driftAngle;

        if(driftAngle >= minimalAngleToDrift)
        {
            player.SetIsDrifting(true);
            PlayCarBreakeSound();
            return;
        }

        StopCarBreakeSound();
        player.SetIsDrifting(false);    
    }

    private float GetSignedAngle()
    {
        return Vector3.SignedAngle(transform.forward, carRB.velocity + transform.forward, Vector3.up); ; 
    }

    private void PlayCarBreakeSound()
    {
        if (carBreakeSource.isPlaying)
            return;

        carBreakeSource.Play();
    }

    private void StopCarBreakeSound()
    {
        carBreakeSource.Stop();
    }
}
