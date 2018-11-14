namespace FoundationDbNet
{
    using System;
    using System.Threading.Tasks;

    public interface IFdbConnection : IDisposable
    {
        Task<IFdbDatabase> OpenDefaultDatabaseAsync();
    }
}
