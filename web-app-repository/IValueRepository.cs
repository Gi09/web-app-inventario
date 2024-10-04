using web_app_domain;

namespace web_app_repository
{
    public interface IValueRepository
    {
        Task<IEnumerable<Values>> ListValues();
        Task SaveValues(Values produtos);
        Task UpdateValues(Values produtos);
        Task DeleteValues(int id);

    }
}