using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using PlayerRecordsApi.Models;
using PlayerRecordsApi.Controllers;
using Xunit;

namespace PlayerRecordsApi.Tests;

public class PrivateTests
{
    static internal void CallPrivateStaticVoidMethod(Type type, string methodName, object[] parameters) // new object[] { ... }
    {
        BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Static; //https://learn.microsoft.com/zh-cn/dotnet/api/system.reflection.bindingflags?view=net-6.0
        MethodInfo method = type.GetMethod(methodName, bindingAttr);
        
        method.Invoke(null, parameters);
    }

    static internal object CallPrivateStaticMethod(Type type, string methodName, object[] parameters)
    {
        BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Static; 
        MethodInfo method = type.GetMethod(methodName, bindingAttr);
        
        return (object)method.Invoke(null, parameters);
    }

    static internal void CallPrivateInstanceVoidMethod<TInstance> (TInstance instance, string methodName, object[] parameters)
    {
        Type type = instance.GetType();
        BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        MethodInfo method = type.GetMethod(methodName, bindingAttr);
        
        method.Invoke(instance, parameters);
    }

    static internal object CallPrivateInstanceMethod<TInstance> (TInstance instance, string methodName, object[] parameters)
    {
        Type type = instance.GetType();
        BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        MethodInfo method = type.GetMethod(methodName, bindingAttr);
        
        return (object)method.Invoke(instance, parameters);
    }
}
