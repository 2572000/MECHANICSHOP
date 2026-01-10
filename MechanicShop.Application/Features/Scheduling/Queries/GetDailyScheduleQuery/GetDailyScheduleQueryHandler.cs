using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.Scheduling.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Workorders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MechanicShop.Application.Features.Scheduling.Queries.GetDailyScheduleQuery
{
    public class GetDailyScheduleQueryHandler(IAppDbContext context,
        TimeProvider timeProvider)
        :IRequestHandler<GetDailyScheduleQuery,Result<ScheduleDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result<ScheduleDto>> Handle(GetDailyScheduleQuery request, CancellationToken ct)
        {
            var localStart = request.ScheduleDate.ToDateTime(TimeOnly.MinValue);
            var localEnd = localStart.AddDays(1);

            var utcStart=TimeZoneInfo.ConvertTimeToUtc(localStart,request.TimeZone);
            var utcEnd=TimeZoneInfo.ConvertTimeToUtc(localEnd,request.TimeZone);

            var workOrders=await _context.Workorders
                .Where(w=>
                w.StartAtUtc < utcEnd &&
                w.EndAtUtc > utcStart &&
                (request.LaborId == null || w.LaborId == request.LaborId))
                .Include(w=>w.Vehicle)
                .Include(w=>w.Labor)
                .Include(w=>w.RepairTasks)
                .ToListAsync(ct);
            var now=TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(),request.TimeZone);

            var result = new ScheduleDto
            {
                OnDate = request.ScheduleDate,
                EndOfDay = localEnd < now,
                Spots = []
            };
            foreach (var spot in Enum.GetValues<Spot>())
            {
                var current = localStart;
                var slots = new List<AvailabilitySlotDto>();

                var woBySpot = workOrders
                    .Where(w => w.Spot == spot)

                    .OrderBy(w => w.StartAtUtc)
                    .ToList();

                while (current < localEnd)
                {
                    var next = current.AddMinutes(15);
                    var startUtc = TimeZoneInfo.ConvertTimeToUtc(current, request.TimeZone);
                    var endUtc = TimeZoneInfo.ConvertTimeToUtc(next, request.TimeZone);

                    var wo = woBySpot.FirstOrDefault(w =>
                        w.StartAtUtc < endUtc && w.EndAtUtc > startUtc);

                    if (wo != null)
                    {
                        if (!slots.Any(s => s.WorkOrderId == wo.Id))
                        {
                            slots.Add(new AvailabilitySlotDto
                            {
                                WorkOrderId = wo.Id,
                                Spot = spot,
                                StartAt = wo.StartAtUtc,
                                EndAt = wo.EndAtUtc,
                                Vehicle = FormatVehicleInfo(wo.Vehicle!),
                                Labor = wo.Labor!.ToDto(),
                                IsOccupied = true,
                                RepairTasks = [.. wo.RepairTasks.ToList().ConvertAll(rt => rt.ToDto())],
                                WorkOrderLocked = !wo.IsEditable,
                                State = wo.State,
                                IsAvailable = false
                            });
                        }
                    }
                    else
                    {
                        slots.Add(new AvailabilitySlotDto
                        {
                            Spot = spot,
                            StartAt = startUtc,
                            EndAt = endUtc,
                            WorkOrderLocked = false,
                            IsAvailable = current >= now
                        });
                    }

                    current = next;
                }

                result.Spots.Add(new SpotDto
                {
                    Spot = spot,
                    Slots = slots
                });
            }

            return result;
        }
        private static string? FormatVehicleInfo(Vehicle vehicle) =>
        vehicle != null ? $"{vehicle.Make} | {vehicle.LicensePlate}" : null;
    }
}
