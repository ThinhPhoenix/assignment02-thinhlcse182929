using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using newsmng_bussinessobject;

namespace newsmng_dao
{
    public class NewsArticleDAO
    {

        private FunewsManagementContext _dbContext;
        private static NewsArticleDAO instance;

        public NewsArticleDAO()
        {
            _dbContext = new FunewsManagementContext();
        }

        public static NewsArticleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NewsArticleDAO();
                }
                return instance;
            }
        }


//PLACE AT HTML
<div class="search">
 <form method = "get" class="row mb-3 gap-2">
  <input type = "text" class="form-control" name="searchMedicineName" placeholder="Search by medicine name..." value="@Model.SearchMedicineName">
  <input type = "text" class="form-control" name="searchWarningAndPrecautions" placeholder="Search by warnings..." value="@Model.SearchWarningAndPrecautions">
  <button type = "submit" class="btn btn-primary me-md-2">Search</button>
 </form>
</div>

//IF HAVE PAGEABLE AT TO PAGEABLE SEARCH FIELD AT CSHTML
?page=@(Model.CurrentPage - 1)&searchMedicineName=@(Model.SearchMedicineName)&searchWarningAndPrecautions=@(Model.SearchWarningAndPrecautions)

//CSHARP INDEX INIT (SEARCHES FIELD)
[BindProperty(SupportsGet = true)]
public string SearchMedicineName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchWarningAndPrecautions { get; set; }

        //CSHARP INDEX INSIDE ONGET()
        _medicineInformationRepo.Search(SearchMedicineName, SearchWarningAndPrecautions)

//DAO
public List<MedicineInformation> Search(string searchMedicineName, string searchWarningAndPrecautions)
        {
            if (string.IsNullOrWhiteSpace(searchMedicineName) && string.IsNullOrWhiteSpace(searchWarningAndPrecautions))
                return GetAll();

            if (searchMedicineName != null)
            {
                searchMedicineName = searchMedicineName.Trim().ToLower();
            }
            if (searchWarningAndPrecautions != null)
            {
                searchWarningAndPrecautions = searchWarningAndPrecautions.Trim().ToLower();
            }

            return _dbContext.MedicineInformations
                .Where(m =>
                    m.MedicineName.ToLower().Contains(searchMedicineName) ||
                    m.WarningsAndPrecautions.ToLower().Contains(searchWarningAndPrecautions))
                .OrderBy(e => e.ManufacturerId)
                .ToList();
        }


//CSHTML                    
<div class="pagination gap-3 mb-2">
	@{
		if (Model.CurrentPage > 1)
		{
			<a href = "?page=@(Model.CurrentPage - 1)&searchMedicineName=@(Model.SearchMedicineName)&searchWarningAndPrecautions=@(Model.SearchWarningAndPrecautions)" > Previous </ a >

        }
		<span>Page @Model.CurrentPage of @Model.TotalPages</span>
		if (Model.CurrentPage<Model.TotalPages)

        {

            < a href = "?page=@(Model.CurrentPage + 1)&searchMedicineName=@(Model.SearchMedicineName)&searchWarningAndPrecautions=@(Model.SearchWarningAndPrecautions)" > Next </ a >

        }
}
</ div >

//CSHARP INIT
public int CurrentPage { get; set; }
public int TotalPages { get; set; }

//CSHARP IN ONGET()
var pageQuery = HttpContext.Request.Query["page"];
CurrentPage = 1;

if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
{
    CurrentPage = parsedPage;
}

dynamic pageable = _medicineInformationRepo.Pageable(_medicineInformationRepo.Search(SearchMedicineName, SearchWarningAndPrecautions), CurrentPage, 3);

TotalPages = pageable.TotalPages;
Object = pageable.Data;

//DAO
public class PageableResult<T>
{
    public List<T> Data { get; set; }
    public int TotalPages { get; set; }
}

public PageableResult<MedicineInformation> Pageable(List<MedicineInformation> data, int curPage, int pageSize)
{
    if (curPage <= 0)
        throw new ArgumentException("Current page must be greater than 0.");
    if (pageSize <= 0)
        throw new ArgumentException("Number of items per page must be greater than 0.");

    // Get total count of items
    int totalItems = data.Count();

    // Calculate total pages
    int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

    // Get data for current page
    var Res = data
        .OrderBy(e => e.ManufacturerId) // Make sure results are in a deterministic order
        .Skip((curPage - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return new PageableResult<MedicineInformation>
    {
        Data = Res,
        TotalPages = totalPages
    };
}
                
    }
}
