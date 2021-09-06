using EngineCoreProject.DTOs.QueueDto;
using EngineCoreProject.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using EngineCoreProject.DTOs.TabDto;
using EngineCoreProject.DTOs.RoleDto;
using Microsoft.AspNetCore.Http;

namespace EngineCoreProject.IRepository.ITabRepository
{
    public interface ITabRepository
    {
        Task<int> AddTab(TabPostDto tabPostDto);
        Task<int> UpdateTab(TabPostDto tabPostDto, int rowId);
        Task<int> DeleteTab(int id, string lang);
        Task<List<TabGetDto>> GetTabs(string lang);
        Task<List<UserTabGetDTO>> GetTabsByIds(List<int> tabIds, string lang);
        Task<List<UserTabGetDTO>> GetMyTabs(int userId, string lang);
        Task<int> UpdateIconTab(IFormFile IconImage, int rowId);
        Task<int> UpdateIconStringTab(string iconString, int rowId);
    }
}
