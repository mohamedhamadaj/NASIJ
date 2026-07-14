using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;
using TMAProject.ViewModels.Admin.CategoryVM;

namespace TMAProject.Repository.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<Category?> GetCategoryWithProductsAsync(Guid categoryId);

        public Task<IEnumerable<CategoryListVm>> GetCategoriesWithProductCountAsync(CancellationToken cancellationToken = default);

        public  Task<bool> IsNameExistAsync(string name, Guid? id);

        public  Task<IEnumerable<Category>> GetActiveCategoriesAsync();

        Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default);

        public Task<IEnumerable<SelectListItem>> GetCategoriesForDropdownAsync(CancellationToken cancellationToken);
    }
}
