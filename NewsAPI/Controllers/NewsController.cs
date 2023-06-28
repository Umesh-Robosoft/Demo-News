using BussinessLogicInterfaces;
using CommonObjects;
using DomainEntities;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        #region Fields Declarations
        private readonly INewsInterface _newsInterface;
        private readonly IValidator<NewsDto> newsValidator;
        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment hostingEnvironment;
        #endregion

        #region Constructor
        public NewsController(INewsInterface newsService, IValidator<NewsDto> newsValidator)
        {
            this._newsInterface = newsService;
            this.newsValidator = newsValidator;
        }
        #endregion

        #region AddNews
        /// <summary>
        /// Add data into the news entity 
        /// </summary>
        /// <param name="newsDto"></param>
        /// <returns></returns>
        [Route("SaveNews")]
        [HttpPost]
        [ProducesResponseType(typeof(NewsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult AddNews([FromForm] NewsDto newsDto)
        {
            try
            {
                var validationResult = this.newsValidator.Validate(newsDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(error => new { ErrorCode = error.ErrorCode, ErrorMessage = error.ErrorMessage });
                    return Results.BadRequest(ResponseHelper.Error(ApplicationHelper.ExceptionMsg, errors));
                }
                if (Request.Form.Files.Count >= 1)
                {
                    var fileExtensions = new string[] { ".jpg", ".jpeg", ".png" };
                    var newsImage = Request.Form.Files.Where(file => file.Name == nameof(newsDto.NewsImage)).FirstOrDefault();
                    if (newsImage != null)
                    {
                        //Validate image extension
                        var newsImageExtension = Path.GetExtension(newsImage.FileName);
                        if (!(fileExtensions.Contains(newsImageExtension.ToLower())))
                        {
                            return Results.BadRequest(ResponseHelper.Error(ApplicationHelper.ImageValidationMsg, new object()));
                        }
                        //Validate image name lenth accept only lessthan 150 char in file name
                        if (newsImage.FileName.Length > ApplicationHelper.ImageNameLenth)
                        {
                            return Results.BadRequest(ResponseHelper.Error(ApplicationHelper.ImageNameLenthMsg, new object()));
                        }

                        string newsFilePath = ApplicationHelper.FilePath;
                        if (!Directory.Exists(newsFilePath))
                        {
                            Directory.CreateDirectory(newsFilePath);
                        }

                        newsFilePath = Path.Combine(newsFilePath, newsImage.FileName);
                        using (var stream = new FileStream(newsFilePath, FileMode.Create))
                        {
                            newsDto.NewsImage.CopyTo(stream);
                        }
                        newsDto.ImagePath = string.Concat("~NewsImages/", newsImage.FileName);
                        newsDto.Id = System.Guid.NewGuid().ToString();
                        newsDto.Date = DateTime.UtcNow.Date;
                        this._newsInterface.AddNews(newsDto);
                    }
                }
                return Results.Ok(ResponseHelper.Success(ApplicationHelper.NewsSaveSuccess + "Id= " + newsDto.Id));
            }
            catch (Exception)
            {
                var exceptionHandlerFeature =
                 HttpContext.Features.Get<IExceptionHandlerFeature>()!;
                return Results.Problem(detail: exceptionHandlerFeature.Error.StackTrace, title: exceptionHandlerFeature.Error.Message);
            }

        }
        #endregion End AddNews

        #region FetchAllNews
        [HttpGet]
        [ProducesResponseType(typeof(List<NewsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> FetchAllNews()
        {
            var newsList = await this._newsInterface.FetchAllNews();
            return newsList.Count == 0 ? Results.NoContent() : Results.Ok(newsList);
        }
        #endregion FetchAllNews

        #region SearchNews
        [Route("SearchAllNews")]
        [HttpGet]
        [ProducesResponseType(typeof(List<NewsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> SearchAllNews(string searchText)
        {
            var newsList = await this._newsInterface.SearchInAllNews(searchText);
            return newsList.Count == 0 ? Results.NoContent() : Results.Ok(newsList);
        }
        #endregion End SearchNews

    }
}
