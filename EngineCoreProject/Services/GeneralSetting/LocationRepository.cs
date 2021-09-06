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
    public class LocationRepository : ILocationRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        ValidatorException _exception;
        public LocationRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _exception = new ValidatorException();
        }

        public async Task<int> AddLocation(LocationPostDto locationPostDto, string lang)
        {
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            if (locationPostDto.ParentId != null)
            {
                if (!await _EngineCoreDBContext.Location.AnyAsync(x => x.Id == locationPostDto.ParentId))
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "InvalidPrimeClass"));
                    throw _exception;
                }
            }

            Location location = new Location
            {
                NameShortcut = _iGeneralRepository.GenerateShortCut(Constants.LOCATION, Constants.LOCATION_NAME_SHORTCUT),
                ParentLocationId = locationPostDto.ParentId
            };

            await _EngineCoreDBContext.Location.AddAsync(location);
            if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
            {
                await _iGeneralRepository.InsertUpdateSingleTranslation(location.NameShortcut, locationPostDto.NameShortCutLangValue);
                await transaction.CommitAsync();
            }
            return location.Id;
        }


        public async Task<List<LocationGetDto>> GetLocations(string lang)
        {
            var Locations = await _EngineCoreDBContext.Location.ToListAsync();
            List<LocationGetDto> result = new List<LocationGetDto>();
            foreach (var row in Locations)
            {
                var locName = await _iGeneralRepository.GetTranslateByShortCut(lang, row.NameShortcut);
                if (row.ParentLocationId == null)
                {
                    locName = locName + " _ " + locName;
                }
                else
                {
                    var emarit = await _EngineCoreDBContext.Location.Where(x => x.Id == row.ParentLocationId).FirstOrDefaultAsync();
                    locName = await _iGeneralRepository.GetTranslateByShortCut(lang, emarit.NameShortcut) + " _ " + locName;
                }

                LocationGetDto locationGetDto = new LocationGetDto
                {
                    LocationId = row.Id,
                    EmaritLocationName = locName
                };


                result.Add(locationGetDto);
            }
            return result;
        }

    }
}
