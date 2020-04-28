using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorMovies.Shared.DTOS
{
    public class FilterMovieDTO
    {
        public int Page { get; set; } = 1;
        public int RecordPerPages { get; set; } = 10;
        public PaginationDTO Pagination
        {
            get { return new PaginationDTO() { Page = Page, RecordsPerPage = RecordPerPages }; }
        }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool UpComingReleases { get; set; }
    }
}
