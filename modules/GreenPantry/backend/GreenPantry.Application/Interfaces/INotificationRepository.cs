using System;
using System.Threading.Tasks;
using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
    }
}
