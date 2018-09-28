using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace LineCheckerSrv.Scripting
{
    /// <summary>
    /// Implements custom exception thrown if were errors during script scenario execution 
    /// </summary>
    [Serializable]
    public sealed class ScriptingException : Exception, ISerializable
    {

        public ScriptingException() : base() 
        {}

        public ScriptingException(String message) : base(message) 
        {}

        public ScriptingException(String message, Exception innerException) : base(message, innerException)
        {}

        private ScriptingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {}

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}