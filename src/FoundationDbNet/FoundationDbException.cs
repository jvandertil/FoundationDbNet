namespace FoundationDbNet
{
    using System;
    using System.Runtime.Serialization;
    using FoundationDbNet.Native;

    [Serializable]
    public class FoundationDbException : Exception
    {
        public FdbError ErrorCode { get; }

        public FoundationDbException()
        {
        }

        public FoundationDbException(string message)
            : base(message)
        {
        }

        public FoundationDbException(string message, FdbError errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public FoundationDbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FoundationDbException(SerializationInfo info, StreamingContext context)
        {
            if (info != null)
            {
                this.ErrorCode = (FdbError)info.GetInt32(nameof(ErrorCode));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(ErrorCode), (int)ErrorCode);
            }
        }
    }
}
