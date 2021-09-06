using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EngineCoreProject.Services.GeneralSetting
{
    public class ServiceKindRepository : IServiceKindRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        public ServiceKindRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
        }

        public async Task<int> AddServiceKind(ServiceKindPostDto serviceKindPostDto, string lang)
        {
            int res = 0;
            ValidateServiceKind(serviceKindPostDto, lang);
            using (var transaction = _EngineCoreDBContext.Database.BeginTransaction())
            {
                ServiceKind serviceKind = serviceKindPostDto.GetEntity();
                serviceKind.ServiceKindNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.SERVICE_KIND, Constants.SERVICE_KIND_NAME_SHORTCUT);
                _EngineCoreDBContext.ServiceKind.Add(serviceKind);
                _EngineCoreDBContext.SaveChanges();

                await _iGeneralRepository.InsertUpdateSingleTranslation(serviceKind.ServiceKindNameShortcut, serviceKindPostDto.NameShortCutLangValue);
                await transaction.CommitAsync();
                res = serviceKind.Id;
            }
            return res;
        }

        public async Task<int> UpdateServiceKind(ServiceKindPostDto serviceKindPostDto, int rowId, string lang = null)
        {
            int res = 0;
            ValidateServiceKind(serviceKindPostDto, lang, rowId);
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            ServiceKind serviceKind = await _EngineCoreDBContext.ServiceKind.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (serviceKind == null)
            {
                return res;
            }

            serviceKind.EmployeeCount = serviceKindPostDto.EmployeeCount;
            serviceKind.EstimatedTimePerProcess = serviceKindPostDto.EstimatedTimePerProcess;
            serviceKind.Symbol = serviceKindPostDto.Symbol;

            _EngineCoreDBContext.ServiceKind.Update(serviceKind);
            _EngineCoreDBContext.SaveChanges();

            await _iGeneralRepository.InsertUpdateSingleTranslation(serviceKind.ServiceKindNameShortcut, serviceKindPostDto.NameShortCutLangValue);
            await transaction.CommitAsync();
            res = serviceKind.Id;

            return res;
        }


        public async Task<int> DeleteServiceKind(int id)
        {
            int res = 0;
            ServiceKind oriServiceKind = await _EngineCoreDBContext.ServiceKind.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (oriServiceKind == null)
            {
                return res;
            }

            using (var transaction = _EngineCoreDBContext.Database.BeginTransaction())
            {
                await _iGeneralRepository.DeleteTranslation(oriServiceKind.ServiceKindNameShortcut);
                _EngineCoreDBContext.ServiceKind.Remove(oriServiceKind);
                _EngineCoreDBContext.SaveChanges();
                transaction.Commit();
                res = oriServiceKind.Id;
            }
            return res;
        }

        public async Task<List<ServiceKindGetDto>> GetServiceKinds(string lang)
        {
            var serviceKind = await _EngineCoreDBContext.ServiceKind.ToListAsync();
            List<ServiceKindGetDto> result = new List<ServiceKindGetDto>();
            foreach (var row in serviceKind)
            {
                var dept = ServiceKindGetDto.GetDTO(row);
                var LangValue = await _iGeneralRepository.getTranslationsForShortCut(row.ServiceKindNameShortcut);
                if (LangValue.ContainsKey(lang))
                {
                    dept.serviceName = LangValue;
                    dept.NameShortCut = LangValue[lang];
                }

                result.Add(dept);
            }
            return result;
        }

        public async Task<ServiceKindGetDto> GetServiceKindById(int id, string lang)
        {
            ServiceKind serviceKind = new ServiceKind();
            serviceKind = await _EngineCoreDBContext.ServiceKind.Where(d => d.Id == id).FirstOrDefaultAsync();
            ServiceKindGetDto res = new ServiceKindGetDto();
            if (serviceKind == null)
            {
                return res;
            }
            res = ServiceKindGetDto.GetDTO(serviceKind);

            var LangValue = await _iGeneralRepository.getTranslationsForShortCut(serviceKind.ServiceKindNameShortcut);
            if (LangValue.ContainsKey(lang))
            {
                res.NameShortCut = LangValue[lang];
            }

            return res;
        }

        private void ValidateServiceKind(ServiceKindPostDto ServiceKindDto, string lang, int exclude = 0)
        {
            var repeatedSymbol = _EngineCoreDBContext.ServiceKind.Where(a => a.Symbol == ServiceKindDto.Symbol && a.Id != exclude);
            if (repeatedSymbol.Count() != 0)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " repeated symbol");
            }

            if (ServiceKindDto.EmployeeCount < 1)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " for EmployeeCount");
            }

            if (ServiceKindDto.EstimatedTimePerProcess < 1)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " for EstimatedTimePerProcess");
            }
        }
    }
}
