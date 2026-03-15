using Popsalon.Domain.Entities;

namespace Popsalon.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment, Guid>;
