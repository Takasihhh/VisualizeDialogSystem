
using System;
using System.Reflection;
using UnityEngine;

namespace DialogSystem.Utilies
{

    public static class ReflectionMethodExtension
    {
        public static object CreateInstance(string className)
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Type type = assembly.GetType(className);
            if (type != null)
            {
                object res = Activator.CreateInstance(type);
                return res;
            }
            else
            {
                Debug.LogError("type为空");
            }
            return null;
        }

    }
}