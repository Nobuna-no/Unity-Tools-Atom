using UnityEngine;

namespace UnityTools.Atom
{
    public static class Logger
    {
        public enum Type : int
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Assert = 3,
            Error = 4,
            Critical = 5
        }

        public static void Assert(bool condition, object message, Object context = null)
        {
            if (!condition)
            {
                Log(Type.Assert, message, context);
                Debug.Break();
            }
        }

        public static void Log(Type type, object message, Object context = null)
        {
            string typeHeader = "[" + type.ToString() + "]";
            string msg = "\n => " + message + "\n";

            if (context != null)
                msg = "\t{ Type: " + context.GetType().FullName + ", Name: " + context.name + " }" + msg;
            else
                msg = "\t{ Anonymous }" + msg;

            switch (type)
            {
                case Type.Critical:
                    Debug.LogError("<color=red>" + typeHeader + "</color>" + msg);
                    break;
                case Type.Error:
                    Debug.LogError("<color=magenta>" + typeHeader + "</color>  " + msg);
                    break;
                case Type.Assert:
                    Debug.LogAssertion("<color=yellow>" + typeHeader + "</color>" + msg);
                    break;
                case Type.Warning:
                    Debug.LogWarning("<color=orange>" + typeHeader + "</color>" + msg);
                    break;
                case Type.Info:
                    Debug.Log("<color=green>" + typeHeader + "</color>  " + msg);
                    break;
                default:
                    Debug.Log("<color=cyan>" + typeHeader + "</color>" + msg);
                    break;
            }
        }
        public static void LogFormat(Type type, Object context, string format, params object[] args)
        {
            Log(type, string.Format(format, args), context);
        }
    }
}