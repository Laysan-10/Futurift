using UnityEngine;

namespace AztechGames
{
    public class PhysicOnGlider : MonoBehaviour
    {
        [Header("Main Physics Variables")] 
        [Tooltip("Air density in kg/m³")]
        public float airDensity = 1.225f;

        [Tooltip("Wing area in square meters")]
        public float wingArea = 27.87f;

        [Tooltip("Coefficient of lift")]
        public float liftCoefficient = 1.8f;

        [Tooltip("Coefficient of drag")]
        public float dragCoefficient = 0.5f;

        [Tooltip("Temperature in degrees Celsius")]
        public float temperature = 59f;

        private float IAS = 0f;
        private float altitude = 0f;
        private Vector3 windVelocity;
     
        [HideInInspector]
        public Rigidbody _rb;

        private GliderEngine_Controller _gliderEngineController;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _gliderEngineController = GetComponent<GliderEngine_Controller>();
        }

        /// <summary>
        /// Вычисляет истинную воздушную скорость параплана.
        /// </summary>
        /// <returns>Истинная скорость полета в метрах в секунду.</returns>
        public float TrueAirSpeed()
        {
            IAS = _rb.velocity.magnitude;
            altitude = transform.position.y;
            float tas = IAS / (Mathf.Pow(1f + altitude / 44330f, 5.255f) * Mathf.Sqrt((temperature + 273.15f) / 288.15f));
            return tas;
        }

        /// <summary>
        /// Вычисляет воздушную скорость параплана с учетом направления ветра.
        /// </summary>
        /// <returns>Скорость полета в метрах в секунду.</returns>
        public float AirSpeed()
        {
            Vector3 relativeWind  = windVelocity - _rb.velocity;
            Quaternion rotation = Quaternion.Inverse(transform.rotation);
            Vector3 relativeWindLocal = rotation * relativeWind;
            float airSpeed = Mathf.Sqrt(Mathf.Pow(TrueAirSpeed(), 2) + Mathf.Pow(relativeWindLocal.magnitude, 2));
            return airSpeed;
        }

        /// <summary>
        ///Вычисляет подъемную силу параплана.
        /// </summary>
        /// <returns>Подъемная сила в ньютонах.</returns>
        public float CalculateLift()
        {
            float angleOfAttack = Vector3.Angle(Vector3.forward, transform.forward);
            float radianOfAngleDegree = Mathf.Deg2Rad * angleOfAttack;//Переводит угол атаки из градусов в радианы
            var lift = 0.5f * airDensity * TrueAirSpeed() * wingArea * liftCoefficient * Mathf.Cos(radianOfAngleDegree);//расчет подъемной силы
            return lift;
        }

        /// <summary>
        /// Вычисляет силу лобового сопротивления планера.
        /// </summary>
        /// <returns>Сила сопротивления в ньютонах.</returns>
        public float CalculateDrag()
        {
            float angleOfAttack = Vector3.Angle(Vector3.forward, transform.forward);
            float radianOfAngleDegree = Mathf.Deg2Rad * angleOfAttack;
            var drag = 0.5f * airDensity * AirSpeed() * wingArea * dragCoefficient * Mathf.Pow(Mathf.Cos(radianOfAngleDegree), 2);// сопротивления воздуха
            return drag;
        }

        /// <summary>
        ///Применяет подъемную силу и лобовое сопротивление для моделирования физики планера.
        /// </summary>
        void CalculateDradAndLift()
        {
            float liftForce = CalculateLift();
            float dragForce = CalculateDrag();
            
            Vector3 dragDirection = -_rb.velocity.normalized;// задает направление силы сопротивления
            Vector3 drag = dragDirection * dragForce;
            _rb.AddForce(drag);//обавляет силу сопротивления к объекту, замедляя его движение

            Vector3 liftVector = transform.up * liftForce;// вектор подъемной силы
            _rb.AddForce(liftVector);
        }
        
        private void Update()
        {
            _rb.AddForce(transform.forward * _gliderEngineController.Thrust, ForceMode.Acceleration);// - ForceMode.Acceleration — это режим приложения силы, который определяет,
                                                                                                     // что сила будет применяться как ускорение, то есть без учета массы объекта.
            CalculateDradAndLift();
        }
    }
}
