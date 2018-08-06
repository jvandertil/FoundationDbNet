namespace FoundationDbNet
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using FoundationDbNet.Native;

    [Serializable]
    public class FoundationDbException : Exception
    {
        [SuppressMessage("Microsoft.Usage", "CA2235", Justification = "False positive due to bug in analyzer not recognizing Enum in .NET Standard.")]
        internal FdbError ErrorCode { get; }

        public int ErrorNumber => (int)ErrorCode;

        public FoundationDbException()
        {
        }

        public FoundationDbException(string message)
            : base(message)
        {
        }

        internal FoundationDbException(string message, FdbError errorCode)
            : base(message)
        {
            if (errorCode == FdbError.Success)
            {
                throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, "Error code can not be Success.");
            }

            ErrorCode = errorCode;
        }

        public FoundationDbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FoundationDbException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                ErrorCode = (FdbError)info.GetInt32(nameof(ErrorCode));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ErrorCode), (int)ErrorCode);
        }
    }
}
