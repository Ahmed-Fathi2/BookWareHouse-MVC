using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Common
{
    public class Notes
    {
        //asp-validation-summary="ModelOnly"
        //<div asp-validation-summary="ModelOnly" class="text-danger"></div>  --->> show error doesnt related to any prop only
        //like : ModelState.AddModelError("", "Category name cannot be 'test'.");


        //asp-validation-summary="All"
        //<div asp-validation-summary="All" class="text-danger"></div>  --->> show error  all  related to any prop and not related
        //like : ModelState.AddModelError("", "Category name cannot be 'test'.");
        //like : ModelState.AddModelError("name", "Category name cannot be 'test'.");



    }
}
