using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Utils.CompileScripts
{
    /// <summary>
    /// Implements custom exception thrown if scripts were compiled with errors
    /// </summary>
    [Serializable]
    public sealed class CompileErrorException : Exception, ISerializable
    {

        private readonly string fileName;

        public string FileName
        {
            get { return fileName; }
        }

        public CompileErrorException(string message, string fileName)
            : base(message) 
        {
            this.fileName = fileName;
        }

        public CompileErrorException(string message, Exception innerException) : base(message, innerException)
        {}

        private CompileErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {}

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
