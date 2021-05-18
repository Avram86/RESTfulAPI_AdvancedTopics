using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using RESTfulAPI_Aync.Helpers;
using RESTfulAPI_Aync.Services;
using RESTfulAPI_Pluralsight.Models;
using RESTfulAPI_Pluralsight.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RESTfulAPI_Pluralsight.Controller
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController:ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
        }

        [HttpGet(Name ="GetAuthors")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(
            //for filtering
            [FromQuery]AuthorsResourceParameters authorsResourceParameters)
        {
            if(!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var authorsFromRepo =await _courseLibraryRepository.GetAuthorsAsync(authorsResourceParameters);

            var previousPagedLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPagedLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount=authorsFromRepo.TotalCount,
                pageSize=authorsFromRepo.PageSize,
                currentPage=authorsFromRepo.CurrentPage,
                totalPages=authorsFromRepo.TotalPages,
                previousPageLink=previousPagedLink,
                nextPageLink=nextPagedLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId:guid}", Name ="GetAuthor")]
        public async Task<ActionResult<AuthorDto>> GetAuthorAsync(Guid authorId)
        {
            var authorFromRepo =await _courseLibraryRepository.GetAuthorAsync(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto authorForCreationDto)
        {
            var authorEntity = _mapper.Map<Author>(authorForCreationDto);

            _courseLibraryRepository.AddAuthor(authorEntity);
            await _courseLibraryRepository.SaveAsync();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { authorId=authorToReturn.Id}, authorToReturn);
        }

        [HttpPut("{authorId}")]
        public async Task<IActionResult> UpdateAuthor(Guid authorId, AuthorForUpdateDto authorForUpdateDto)
        {
            var authorFromRepo =await _courseLibraryRepository.GetAuthorAsync(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            //copiem valorile editate peste valorile entitatii
            _mapper.Map(authorForUpdateDto, authorFromRepo);

            _courseLibraryRepository.UpdateAuthor(authorFromRepo);
            await _courseLibraryRepository.SaveAsync();

            return NoContent();
        }
        
        [HttpDelete("{authorId}")]
        public async Task<ActionResult> DeleteAuthor(Guid authorId)
        {
            var authorFromRepo =await _courseLibraryRepository.GetAuthorAsync(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteAuthor(authorFromRepo);
            await _courseLibraryRepository.SaveAsync();

            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            orderBy=authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuerry = authorsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuerry = authorsResourceParameters.SearchQuery
                        });
                default:
                    return Url.Link("GetAuthors",
                        new
                        {
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuerry = authorsResourceParameters.SearchQuery
                        });
            }
        }
    }
}
