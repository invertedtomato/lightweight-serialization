using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace InvertedTomato.Serialization
{
    public static class DelegateExtensions
    {
        public static object DynamicInvokeTransparent(this Delegate target, params Object[] args)
        {
            try
            {
                return target.DynamicInvoke(args);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;
            }
        }
    }
}