namespace FoundationDbNet.Native
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

            var errorMessage = ErrorMessages.GetOrAdd(
                error,
                err => Marshal.PtrToStringAnsi(NativeMethods.fdb_get_error(err))
            );

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
                return;

            ThrowFoundationDbException(error);
        }

        private static void ThrowFoundationDbException(FdbError error)
        {
            throw error.ToException();
        }
    }
}
