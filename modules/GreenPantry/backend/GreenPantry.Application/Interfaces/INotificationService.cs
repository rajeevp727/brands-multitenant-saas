using System;
using System.Threading.Tasks;
using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string title, string message, string type, string severity, Guid? userId = null, string targetRole = "All", string brandName = "GreenPantry");
    }
}
