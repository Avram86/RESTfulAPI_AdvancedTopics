using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI_Pluralsight.ResourceParameters
{
    public class AuthorsResourceParameters
    {
        const int maxPageSize = 20;

        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pagesize=10;
        public int PageSize 
        {
            get => _pagesize;
            set => _pagesize = (value > maxPageSize) ? maxPageSize : value;
        } 
    }
}
