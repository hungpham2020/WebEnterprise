namespace WebEnterprise.Models.Common
{
    public class CommonPaging
    {
        public int? PageIndex { get; set; }

        public int? PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public int? NextPage { get; set; }
        
        public int? PrevPage { get; set; }

        public CommonPaging(int totalItems, int? pageIndex, int? pageSize)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var nextPage = pageIndex + 1;
            var prevPage = pageIndex - 1;

            if(pageIndex < 1 || totalPages == 0)
            {
                pageIndex = 1;
            }
            else if(pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }
            else if(pageIndex == 1)
            {
                prevPage = pageIndex;
            }

            TotalItems = totalItems;
            TotalPages = totalPages;
            PageIndex = pageIndex;
            PageSize = pageSize;
            NextPage = nextPage;
            PrevPage = prevPage;
        }
    }
}
