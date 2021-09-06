using EngineCoreProject.DTOs.ActionButton;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IActionButtonRepository
{
    public interface IActionButtonRepository
    {
        Task<List<ActionButtonDto>> getallActions(string lang);
        Task<AdmAction> Getone(int id);
        Task<AdmAction> add();
        Task<int> update(int id, int actiontypeid);
    }
}
