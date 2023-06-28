using AutoMapper;
using BussinessLogicInterfaces;
using Database;
using DataEntities;
using DomainEntities;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogicServices
{
    public class NewsService : INewsInterface
    {
        #region Fields Declaration
        /// <summary>
        /// read only field for intialise data base context
        /// </summary>
        private readonly NewsDbContext _newsDbContext;
        /// <summary>
        ///  read only field for automapper configuration
        /// </summary>
        private readonly IMapper _newsMapper;
        #endregion

        #region  Constructor
        /// <summary>
        /// constructor of employee questionnaire mapping service
        /// </summary>
        /// <param name="roboSoftPlociyContext"></param>
        /// <param name="robosoftMapper"></param>
        public NewsService(NewsDbContext newsDbContext, IMapper newsMapper)
        {
            this._newsDbContext = newsDbContext;
            this._newsMapper = newsMapper;
        }

        #endregion

        #region AddNews
        /// <summary>
        /// Insert new entry in news table.
        /// </summary>
        /// <param name="newsDto"></param>
        /// <returns></returns>
        public async Task<string> AddNews(NewsDto newsDto)
        {
            var news = this._newsMapper.Map<News>(newsDto);
            await this._newsDbContext.AddAsync(news);
            this._newsDbContext.SaveChanges();
            return news.Id ?? string.Empty;
        }
        #endregion End AddNews

        #region FetchAllNews
        /// <summary>
        /// Fetch all news and top news. 
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewsDto>> FetchAllNews()
        {
            var allNews = await this._newsDbContext.News.OrderByDescending(news => news.Date).ToListAsync();
            var allNewsDtos = this._newsMapper.Map<List<News>, List<NewsDto>>(allNews);
            return allNewsDtos;
        }
        #endregion End FetchAllNews

        #region SearchInAllNews
        /// <summary>
        /// Fetch all news. 
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewsDto>> SearchInAllNews(string searchText)
        {
            var allNews = await (this._newsDbContext.News.Where(news => news.Title.ToLower().Contains
            (searchText.ToLower()) || news.Detail.ToLower().Contains(searchText.ToLower()))).OrderBy
            (news => news.Title).ThenBy(news => news.Detail).AsQueryable().ToListAsync();
            var allNewsDtos = this._newsMapper.Map<List<News>, List<NewsDto>>(allNews);
            return allNewsDtos;
        }
        #endregion End SearchInAllNews



    }
}
