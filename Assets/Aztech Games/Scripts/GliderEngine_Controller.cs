using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace AztechGames
{
    public class GliderEngine_Controller : MonoBehaviour
    {
        [Tooltip("Acceleration rate of the glider engine.")]
        public float acceleration = 0f;//�������� ��������� �������

        private float thrust = 0f;//������ ������� �������� ���� ������
        public InputActionProperty _right_trigger;//��� ���������� ����� ���� �������� �� 0 �� 1

        /// <summary>
        /// ���������� ��� ������������� �������� ���� � ��������� �� 0 �� 200.
        /// </summary>
        public float Thrust//�������� ��� ��������� � ������������ ����.
        {
            get => Mathf.Clamp(thrust, 0f, 200f);//��� ������ ���������� �������� � ��� ���������.
            set => thrust = value;//������������� �������� ����.
        }

        [SerializeField] PhysicOnGlider _physiconglider;

        /// <summary>
        ///��������� ������� ���������, ��������� ���� � ����������� �� ����� ������ ������������� � �������� ����������.
        /// </summary>
        void EngineInputs()//������������ ���� ������.
        {
            //Debug.Log("right_trigger" + _right_trigger.action.ReadValue<float>().ToString());


            if (_right_trigger.action.ReadValue<float>() == 1)
            {
                thrust += Time.deltaTime * acceleration;//���� ���� ���� ������� � ���� ���������.
                Debug.Log(thrust);
            }
            else
            {
                if (thrust > 0f)
                {
                    thrust -= Time.deltaTime * acceleration;
                }
            }
            //    thrust -= Time.deltaTime * acceleration;

            thrust -= GliderSurface_Controller.Instance.SlatAmount * Time.deltaTime;
        }

        private void Update()
        {
            if (GliderSurface_Controller.Instance != null)
            {
                GliderSurface_Controller.Instance.GetInputs();
                
                GliderSurface_Controller.Instance.HandleInputs();
                if (_physiconglider.AirSpeed() >= 30)
                {
                    GliderSurface_Controller.Instance.PlaneRotations();
                }
                EngineInputs();
            }
        }
    }
}