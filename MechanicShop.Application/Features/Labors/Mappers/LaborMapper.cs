using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Domain.Employees;

namespace MechanicShop.Application.Features.Labors.Mappers
{
    public static class LaborMapper
    {
        public static LaborDto ToDto(this Employee labors)
        {
            ArgumentNullException.ThrowIfNull(labors);
            return new LaborDto
            {
                LaborId = labors.Id,
                Name = labors.FullName
            };
        }

        public static List<LaborDto> ToDto(this IEnumerable<Employee> labors)
        {
            return [.. labors.Select(l => l.ToDto())];
        }


    }
}
