﻿using CcbnbApi.Models;

namespace CcbnbApi.Repositories
{
    public interface IRoomRepository
    {

        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room> GetRoomByIdAsync(int roomId);
        Task AddRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int roomId);
        Task<bool> SaveAsync();

        //PatchRoom
        Task PatchRoomAsync(Room room);

        //GetAvailableRoomsByStartDateAndEndDate
        Task<IEnumerable<Room>> GetAvailableRoomsByStartDateAndEndDate(DateTime startDate, DateTime endDate);



    }



}
