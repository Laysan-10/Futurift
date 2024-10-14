using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace AztechGames
{
    public class GliderEngine_Controller : MonoBehaviour
    {
        [Tooltip("Acceleration rate of the glider engine.")]
        public float acceleration = 0f;//скорость ускорени€ планера

        private float thrust = 0f;//хранит текущее значение т€ги движка
        public InputActionProperty _right_trigger;//дл€ управлени€ силой т€ги значение от 0 до 1

        /// <summary>
        /// ¬озвращает или устанавливает значение т€ги в диапазоне от 0 до 200.
        /// </summary>
        public float Thrust//—войство дл€ получени€ и установлени€ т€ги.
        {
            get => Mathf.Clamp(thrust, 0f, 200f);//ѕри чтении возвращает значение в опр диапозоне.
            set => thrust = value;//”станавливает значение т€ги.
        }

        [SerializeField] PhysicOnGlider _physiconglider;

        /// <summary>
        ///”правл€ет работой двигател€, регулиру€ т€гу в зависимости от ввода данных пользователем и величины предкрылка.
        /// </summary>
        void EngineInputs()//ќбрабатывает ввод игрока.
        {
            Debug.Log(_right_trigger.action.triggered);
            if (_right_trigger.action.triggered)
                thrust += Time.deltaTime * acceleration;//т€га увел проп времени и коэф ускорени€.
            else if (!_right_trigger.action.triggered)
                thrust -= Time.deltaTime * acceleration;

            thrust -= GliderSurface_Controller.Instance.SlatAmount * Time.deltaTime;
        }

        private void Update()
        {
            if (GliderSurface_Controller.Instance != null)
            {
                GliderSurface_Controller.Instance.GetInputs();
                
                GliderSurface_Controller.Instance.HandleInputs();
                //if(_physiconglider.AirSpeed() >= 30)
                //{
                    GliderSurface_Controller.Instance.PlaneRotations();
                //}
                EngineInputs();
            }
        }
    }
}