using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


namespace AztechGames
{
    public class GliderSurface_Controller : Singleton< GliderSurface_Controller >
    {
        [Header("Control Surfaces")]
        [Tooltip("Left aileron transform")]
        public Transform aileronLeft;

        [Tooltip("Right aileron transform")]
        public Transform aileronRight;

        [Tooltip("Elevator transform")]
        public Transform elevator;

        [Tooltip("Left slats transform")]
        public Transform slatsLeft;

        [Tooltip("Right slats transform")]
        public Transform slatsRight;

        [Header("Surfaces Max Angle")]
        [Tooltip("Maximum angle for aileron movement")]
        public float aileronMaxAngle = 30f;

        [Tooltip("Maximum angle for elevator movement")]
        public float elevatorMaxAngle = 30f;

        [Tooltip("Maximum angle for slats movement")]
        public float slatMaxAngle = 0.22f;

        [Tooltip("—корость перемещени€ управл€ющей поверхности")]
        public float surfaceSpeed = 2f;

        [Header("Rotating speeds")]
        [Range(5f, 500f)]
        [SerializeField] private float yawSpeed = 50f;

        [Header("Rotating speeds multiplers when turbo is used")]
        [Range(0.1f, 5f)]
        [SerializeField] private float yawTurboMultiplier = 0.3f;

        // Control Surfaces Angles
        private float _slatAmount;
        private float _elevatorAmount;
        private float _aileronAmount;
        private float inputYawLeft;
        private bool inputYawRight;
        private float currentYawSpeed;

        public InputActionProperty _left_trigger;//дл€ подн€ти€ YAW значение от 0 до 1
       
        public InputActionProperty _left_joystic;//дл€ управлени€ полетом значени€ от -1 до 1
        public InputActionProperty _right_joystic;//дл€ управлени€ поворотом значени€ от -1 до 1


        public float SlatAmount
        {
            get => _slatAmount;
            set => _slatAmount = Mathf.Clamp(value, 0f, slatMaxAngle);
        }
        public float ElevatorAmount
        {
            get => Mathf.Clamp(_elevatorAmount, -elevatorMaxAngle, elevatorMaxAngle);//ƒл€ того чтобы держать значение в диапозоне
            set => _elevatorAmount = value;
        }
        public float AileronAmount
        {
            get => Mathf.Clamp(_aileronAmount, -aileronMaxAngle, aileronMaxAngle);
            set => _aileronAmount = value;
        }

        public void HandleInputs()
        {
            //Yaw axis inputs
            inputYawLeft = _left_joystic.action.ReadValue<Vector2>().x;
            
           

            currentYawSpeed = yawSpeed * yawTurboMultiplier;

            if (inputYawLeft > 0)
            {
                transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
            }
            else if (inputYawLeft<0)
            {
                transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
            }
        }
        public virtual void GetInputs()
        {
            Debug.Log("_right_joystic" + _right_joystic.action.ReadValue<Vector2>().x);
            // ѕримените входные значени€ к управл€ющим поверхност€м
            AileronController(_right_joystic.action.ReadValue<Vector2>().x);// UI scroll
            ElevatorController(_right_joystic.action.ReadValue<Vector2>().y);
            SlatController();

            // —брасывание позиции.
            if (Input.GetKeyDown(KeyCode.R))
            {
                AileronAmount = 0;
                ElevatorAmount = 0;
                SlatAmount = 0;
            }
        }

        void AileronController(float input)
        {
            
            AileronAmount += input * surfaceSpeed;

            aileronLeft.localRotation = Quaternion.Euler(Vector3.forward * (_aileronAmount - 90f) + Vector3.down * 90);//расчитываетс€ угол поворота (оранж крыла)
            aileronRight.localRotation = Quaternion.Euler(Vector3.back * (_aileronAmount + 90f) + Vector3.down * 90);
        }

        void ElevatorController(float input)
        {
            ElevatorAmount += input * surfaceSpeed;

            elevator.localRotation = Quaternion.Euler(Vector3.left * _elevatorAmount);//угол поворота хвоста вверх или вниз
        }

        void SlatController()
        {
            Debug.Log("_left_trigger" + _left_trigger.action.triggered);
            // Increase slat angle when pressing B, decrease when pressing V
            if (_left_trigger.action.triggered)
            {
                SlatAmount += Time.deltaTime * surfaceSpeed;
            }
            //else if (!_left_trigger.action.triggered)
            //{
            //    SlatAmount -= Time.deltaTime * surfaceSpeed;
            //}

            // Clamp the slat amount between 0 and slatMaxAngle
            SlatAmount = Mathf.Clamp(SlatAmount, 0f, slatMaxAngle);

            slatsLeft.localPosition = Vector3.forward * _slatAmount;
            slatsRight.localPosition = Vector3.forward * _slatAmount;
        }

        public void PlaneRotations()
        {

            transform.Rotate(new Vector3(ElevatorAmount / surfaceSpeed, 0f, -AileronAmount) * Time.deltaTime);
        }
    }
}
