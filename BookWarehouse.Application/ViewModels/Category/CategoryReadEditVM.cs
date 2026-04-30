using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Category
{
    public class CategoryReadEditVM
    {

        public int Id { get; set; }

        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "Category name is required.")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters long.")]
        [MaxLength(20, ErrorMessage = "Category name cannot exceed 20 characters.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        [Required(ErrorMessage = "Display order is required.")]
        [Range(1, 100, ErrorMessage = "Display order must be between 1 and 100.")]
        public int DisplayOrder { get; set; }

        [Required(ErrorMessage = "Category description is required.")]
        [MinLength(3, ErrorMessage = "Category description must be at least 3 characters long.")]
        [MaxLength(256, ErrorMessage = "Category description cannot exceed 256 characters.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        public int NumOfProducts { get; set; }

        public bool IsDeleted { get; set; }

 
    }
}
