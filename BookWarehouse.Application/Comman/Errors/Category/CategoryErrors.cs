using BookWarehouse.Application.Comman.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman.Errors.Category
{
    public class CategoryErrors
    {
        public static readonly Error CategoryNotFound = new(
              "CATEGORY_NOT_FOUND",
              "The requested category does not exist."
          );

        public static readonly Error CategoryAlreadyExists = new(
            "CATEGORY_ALREADY_EXISTS",
            "A category with this name already exists."
        );
    }

}
