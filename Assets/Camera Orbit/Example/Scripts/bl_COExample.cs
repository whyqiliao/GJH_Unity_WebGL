using UnityEngine;
using UnityEngine.UI;
using Lovatto.OrbitCamera;

public class bl_COExample : MonoBehaviour {

    public bl_CameraOrbit Orbit;
    public Transform[] Targets;
    public Text CurrenTragetText;

    private int CurrentTarget;

    void Start()
    {
        CurrenTragetText.text = Targets[0].name;
    }

    public void ChangeType(int _type)
    {
        switch (_type)
        {
            case 0:
                Orbit.MovementType = CameraMovementType.Normal;
                Orbit.LerpSpeed = 10;
                break;
            case 1:
                Orbit.MovementType = CameraMovementType.Dynamic;
                Orbit.LerpSpeed = 7;
                break;
            case 2:
                Orbit.MovementType = CameraMovementType.Towars;
                Orbit.LerpSpeed = 6;
                break;
        }
    }

    public void OnAxisSpeed(float value)
    {
        Orbit.SpeedAxis.x = value;
        Orbit.SpeedAxis.y = value;
    }

    public void OnAxisSmooth(float value)
    {
        Orbit.LerpSpeed = value;
    }

    public void LockCursor(bool value)
    {
        Orbit.LockCursorOnRotate = value;
    }

    public void ReuieredInput(bool value)
    {
        Orbit.RequiredInput = value;
    }

    public void OnOutInput(float value)
    {
        Orbit.OutInputSpeed = value;
    }

    public void OnPuw(float value)
    {
        Orbit.TouchZoomAmount = value;
    }

    public void Teleport(bool value)
    {
        Orbit.TeleporOnHit = value;
    }

    public void AutoRot(bool value)
    {
        Orbit.AutoRotate = value;
    }

    public void AutoRotSpeed(float value)
    {
        Orbit.AutoRotationSpeed = value;
    }

    public void ZoomSpeed(float value)
    {
        Orbit.ZoomSpeed = value;
    }

    public void Radius(float value)
    {
        Orbit.CollisionRadius = value;
    }

    public void DetectCollision(bool value)
    {
        Orbit.DetectCollision = value;
    }

    public void ChangeTarget(bool b)
    {
        if (b)
        {
            CurrentTarget = (CurrentTarget + 1) % Targets.Length;
        }
        else
        {
            if(CurrentTarget > 0)
            {
                CurrentTarget = (CurrentTarget - 1) % Targets.Length;
            }
            else
            {
                CurrentTarget = Targets.Length - 1;
            }
        }
        Orbit.SetTarget(Targets[CurrentTarget]);
        CurrenTragetText.text = Targets[CurrentTarget].name;
    }
}