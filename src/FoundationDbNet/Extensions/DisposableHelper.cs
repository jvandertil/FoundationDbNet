namespace FoundationDbNet
{
    using System;

    internal static class DisposableExtensions
    {
        /// <summary>
        /// Executes the <paramref name="map"/> function over the given <see cref="IDisposable"/> <paramref name="value"/>.
        /// Any exception that occurs while mapping will trigger the <paramref name="value"/> to be disposed.
        /// </summary>
        /// <typeparam name="T">The type to map.</typeparam>
        /// <typeparam name="TMapped">The resulting type after mapping.</typeparam>
        /// <param name="value">The value to map.</param>
        /// <param name="map">The function that performs the mapping.</param>
        /// <returns>The result of the <paramref name="map"/> function.</returns>
        internal static TMapped SafeMap<T, TMapped>(this T value, Func<T, TMapped> map)
            where T : class, IDisposable
        {
            var localValue = value;
            try
            {
                var result = map(localValue);
                localValue = null;

                return result;
            }
            finally
            {
                localValue?.Dispose();
            }
        }
    }
}
