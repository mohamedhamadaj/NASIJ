
using TMAProject.Common;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.ColorVM;

namespace TMAProject.Services.Implementations
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        public ColorService( IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public async Task<ServiceResult> CreateAsync(ColorCreateVM model, CancellationToken cancellationToken = default)
        {
            var isExisting = await _colorRepository.IsNameExistAsync(model.Name, Guid.Empty);
            if (isExisting)
            {
                return ServiceResult.Fail("Color Alredy Existing");
            }

            var color = new Color
            {
                Name = model.Name,
            };

            await _colorRepository.AddAsync(color,cancellationToken);
            await _colorRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Color Created Successfully");

        }

        public async Task<ServiceResult> DeleteAsync(Guid colorId, CancellationToken cancellationToken = default)
        {
            var color = await _colorRepository.GetOneAsync(c => c.Id == colorId,
                tracked: false,
                cancellationToken: cancellationToken);

            if (color is null)
            {
                return ServiceResult.Fail("Color Not Found");
            }

             _colorRepository.Delete(color);
            await _colorRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Color Deleted Successfully");

        }

        public async Task<IEnumerable<ColorListVM>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _colorRepository.GetAllColorsAsync(cancellationToken);
            
        }

        public async Task<ColorEditVM?> GetOneAsync(Guid colorId, CancellationToken cancellationToken = default)
        {
            var color = await _colorRepository.GetOneAsync(c => c.Id == colorId,
                tracked: false,
                cancellationToken: cancellationToken);
            if (color == null)
            {
                return null;
            }

            return new ColorEditVM
            {
                ColorId = color.Id,
                Name = color.Name,
            };

                
        }

        public async Task<ServiceResult> UpdateAsync(ColorEditVM model, CancellationToken cancellationToken = default)
        {
            var color = await _colorRepository.GetOneAsync(c => c.Id == model.ColorId,
                tracked: false,
                cancellationToken: cancellationToken);
            if (color is null)
            {
                return ServiceResult.Fail("Color Not Found");
            }
            var isExisting = await _colorRepository.IsNameExistAsync(model.Name, model.ColorId, cancellationToken);
            if (isExisting)
            {
                return ServiceResult.Fail("Color Already Existing");
            }

            color.Name = model.Name;

            _colorRepository.Update(color);
            await _colorRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Color Updated Successfuly");
        }
    }
}
