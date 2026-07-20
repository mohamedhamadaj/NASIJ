using TMAProject.Common;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.SizeVM;

namespace TMAProject.Services.Implementations
{
    public class SizeService : ISizeService
    {

        private readonly ISizeRepository _sizeRepository;
        public SizeService(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
        }

        public async Task<ServiceResult> CreateAsync(SizeCreateVM model, CancellationToken cancellationToken)
        {
            var isExisting =await _sizeRepository.IsNameExistAsync(model.SizeName, Guid.Empty);
            if (isExisting)
            {
                return ServiceResult.Fail("Size Already Existing");
            }

            var size = new Size
            {
                Name = model.SizeName,
            };

            await _sizeRepository.AddAsync(size, cancellationToken);
            await _sizeRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Size Created Successfully");

        }

        public async Task<ServiceResult> DeletAsync(Guid id, CancellationToken cancellationToken)
        {
            var size = await _sizeRepository.GetOneAsync(s => s.Id == id,
                tracked: false,
                cancellationToken: cancellationToken);
            if (size is null)
            {
                return ServiceResult.Fail("Size Not Exist");
            }

            _sizeRepository.Delete(size);
            await _sizeRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Size Deleted Successfully");
        }

        public async Task<IEnumerable<SizeListVM>> GetAllSizeAsync(CancellationToken cancellationToken)
        {
            return await _sizeRepository.GetAllSizesAsync(cancellationToken);
        }

        public async Task<SizeEditVM> GetOneAsync(Guid id, CancellationToken cancellationToken)
        {
            var size = await _sizeRepository.GetOneAsync(s=> s.Id == id,
                tracked:false,
                cancellationToken: cancellationToken);
            if (size is null)
            {
                return null;
            }

            return new SizeEditVM
            {
                SizeId = size.Id,
                SizeName = size.Name,
            };
        }

        public async Task<ServiceResult> UpdateAsync(SizeEditVM model, CancellationToken cancellationToken)
        {
            var size = await _sizeRepository.GetOneAsync(s => s.Id == model.SizeId,
                tracked: false,
                cancellationToken: cancellationToken);

            if(size is null)
            {
                return ServiceResult.Fail("Size not exist");
            }
            var isExisting = await _sizeRepository.IsNameExistAsync(model.SizeName, model.SizeId, cancellationToken);
            if (isExisting)
            {
                return ServiceResult.Fail("Size Already Existing");
            }

            size.Name = model.SizeName;

            _sizeRepository.Update(size);
            await _sizeRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Size Updated Successfully");
        }
    }
}
