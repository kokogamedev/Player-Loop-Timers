using UnityEngine;
using UnityEngine.UIElements;

namespace PsigenVision.ImprovedTimers.Demo
{
    public class TimerExample : MonoBehaviour
    {
        [SerializeField] private UIDocument radialBarUI;
        [SerializeField] private float timer1Duration = 10f;
        [SerializeField] private float timer2Duration = 5f;
        
        private CountdownTimer timer1, timer2;
        [SerializeField] private float outerRadialBarProgress;
        [SerializeField] private float innerRadialBarProgress;
        private int outerRadialBarSegmentCount;
        private bool isInitialized = false;
        void Awake() => Initialize();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (!isInitialized) return;
            
            //Start countdown timers
            timer1.Start();
            timer2.Start();
        }

        private void Initialize()
        {
            isInitialized = true;
            //Bind progress of radial bars to this script as its data source
            VisualElement root = radialBarUI.rootVisualElement;
            var firstRadialBar = root.Q<RadialBar>();
            var secondRadialBar = root.Q<VisualElement>("RadialBar").Q<RadialBar>();

            if (firstRadialBar == null)
            {
                Debug.LogError("Outer RadialBar element not found");
                isInitialized = false;
            }

            if (secondRadialBar == null)
            {
                Debug.LogError("Inner RadialBar element not found");
                isInitialized = false;
            }

            if (isInitialized)
            {
                firstRadialBar.dataSource = this;
                secondRadialBar.dataSource = this;
            }
            
            //Create countdown timers
            if ((timer1 = new CountdownTimer(timer1Duration)) == null)
            {
                Debug.LogError("Failed to create timer1");
                isInitialized = false;
            }

            if ((timer2 = new CountdownTimer(timer2Duration)) == null)
            {
                Debug.LogError("Failed to create timer2");
                isInitialized = false;
            }

            if (!isInitialized) Debug.LogError("Failed to initialize timer example");
            else
            {
                //Calculate the amount of segments in outer radial bar
                outerRadialBarSegmentCount = Mathf.CeilToInt(timer1Duration); //Derive how many segments we want in the bar
                
                //Provide debug log message hooks into timer start and finish callback events
                timer1.OnTimerStart += () => Debug.Log("Timer1 started");
                timer2.OnTimerStart += () => Debug.Log("Timer2 started");
                timer1.OnTimerFinish += () => Debug.Log("Timer1 finished");
                timer2.OnTimerFinish += () => Debug.Log("Timer2 finished");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isInitialized) return;
            //Update radial bars' progress with the countdown timers' progress
            //First radial bar will be treated as a segmented radial bar
            //Derive how many segments are currently visible based on the timer's progress by dividing by the total segment count
            outerRadialBarProgress = 
                (float)Mathf.CeilToInt(timer1.Progress * outerRadialBarSegmentCount) //Calculate the amount of segments to show
                / outerRadialBarSegmentCount //Convert to normalized progress
                * 100f; //Convert to percent (the units used by the radial bar)
            
            //Second radial bar will be treated as if it is continuous
            innerRadialBarProgress = timer2.Progress * 100f;
        }

        void OnDestroy()
        {
            //Dispose of our timers (perform cleanup) when the object is destroyed
            timer1.Dispose();
            timer2.Dispose();
        }
    }
}

