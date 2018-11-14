namespace FoundationDbNet
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides a set of methods to help with <see cref="FdbError"/> values.
    /// </summary>
    internal static class FdbErrorExtensions
    {
        private static readonly ConcurrentDictionary<FdbError, string> ErrorMessages = new ConcurrentDictionary<FdbError, string>();

        /// <summary>
        /// Converts the given <paramref name="error"/> to an exception.
        /// </summary>
        /// <param name="error">The error code to convert to an <see cref="Exception"/>.</param>
        /// <returns>The exception encapsulating the error code.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="error"/> is Success.</exception>
        public static FoundationDbException ToException(this FdbError error)
        {
            if (error == FdbError.Success)
            {
                throw new InvalidOperationException("FdbError.Success is not an error.");
            }

            if (!ErrorMessages.TryGetValue(error, out string errorMessage))
            {
                errorMessage = GetOrAddErrorMessage(error);
            }

            return new FoundationDbException(errorMessage, error);
        }

        /// <summary>
        /// Ensures success by throwing an exception if <paramref name="error"/> does not indicate Success.
        /// </summary>
        /// <param name="error">The error code to check.</param>
        /// <exception cref="FoundationDbException">Thrown when <paramref name="error"/> does not indicate Success.</exception>
        public static void EnsureSuccess(this FdbError error)
        {
            if (error == FdbError.Success)
            {
                return;
            }

            ThrowFoundationDbException(error);
        }

        private static string GetOrAddErrorMessage(FdbError error)
        {
            return ErrorMessages.GetOrAdd(error, err =>
            {
                var msgPtr = fdb_get_error(err);

                string message = Marshal.PtrToStringAnsi(msgPtr);

                return message;
            });
        }

        private static void ThrowFoundationDbException(FdbError error)
        {
            throw error.ToException();
        }

        [DllImport(FdbConstants.FdbDll)]
        private static extern IntPtr fdb_get_error(FdbError error);
    }
}
