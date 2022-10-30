using UnityEngine;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        public bool _consoleShown;

        //#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }
        private void Awake()
        {
            _consoleShown = false;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.F1))
            {
                _consoleShown = !_consoleShown;
            }
        }
        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }
        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            if (_consoleShown)
            {
                myLog = GUI.TextArea(new Rect(10, 10, Screen.width / 4, Screen.height - 60), myLog);
                GUI.TextArea(new Rect(10, Screen.height - 50, Screen.width / 4, 40), "Press 'Left Control' and 'F1' to toggle the console on and off!");
            }
        }
    }
}