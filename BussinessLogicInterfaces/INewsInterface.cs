using DomainEntities;

namespace BussinessLogicInterfaces
{
    /// <summary>
    /// Interface for News related methods
    /// </summary>
    public interface INewsInterface
    {
        /// <summary>
        /// Insert new entry in news table.
        /// </summary>
        /// <param name="newsDto"></param>
        /// <returns></returns>
        Task<string> AddNews(NewsDto newsDto);

        /// <summary>
        ///Fetch all news from news table
        /// </summary>
        /// <returns></returns>
        Task<List<NewsDto>> FetchAllNews();

        /// <summary>
        /// Fetch all news. 
        /// </summary>
        /// <returns></returns>
        Task<List<NewsDto>> SearchInAllNews(string searchText);
    }
}
