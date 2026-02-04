using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Sharing
{
    public class GetUsersParams
    {
        public string? Sort { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int MaxPageSize { get; set; } = 6;
        private int _pageSize { get; set; } = 3;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
    public class ProductParams : GetUsersParams
    {
        [AllowNull]
       public int? CategoryId { get; set; }
    }
  
}
