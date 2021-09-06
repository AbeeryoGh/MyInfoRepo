using EngineCoreProject.DTOs.ActionButton;
using EngineCoreProject.IRepository.IActionButtonRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.ActionButton
{
    public class ActionButtonRepository : IActionButtonRepository
    {

        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public ActionButtonRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;

        }
        public async Task<List<ActionButtonDto>> getallActions(string lang)
        {
            Task<List<ActionButtonDto>> query = null;
            query = (from act in _EngineCoreDBContext.AdmAction
                     join tr in _EngineCoreDBContext.SysTranslation
                     on act.Shortcut equals tr.Shortcut
                     join lv in _EngineCoreDBContext.SysLookupValue
                     on act.ActionTypeId equals lv.Id
                     join tr1 in _EngineCoreDBContext.SysTranslation
                     on lv.Shortcut equals tr1.Shortcut


                     where tr.Lang == lang
                     where tr1.Lang == lang
                     select new ActionButtonDto
                     {
                         actionid = act.Id,
                         actionname = tr.Value,
                         typeid = act.ActionTypeId,
                         typename = tr1.Value
                     }).ToListAsync();

            return await query;
        }

        public async Task<AdmAction> Getone(int id)
        {
            var query = await _EngineCoreDBContext.AdmAction.Where(x => x.Id == id).FirstOrDefaultAsync();
            return query;
        }

        public async Task<AdmAction> add()
        {
            AdmAction admAction = new AdmAction()

            {
                //UgId = serviceDto.UgId,
                Shortcut = _iGeneralRepository.GenerateShortCut("Action", "name_shortcut"),
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,
                //Fee = serviceDto.Fee
            };

            _iGeneralRepository.Add(admAction);


            if (await _iGeneralRepository.Save())
            {

                return admAction;
            }
            return admAction;
        }

        public async Task<int> update(int id, int actiontypeid)
        {
            AdmAction admAction = await Getone(id);
            //if (admStage.OrderNo==stageDto.Order)
            try
            {
                admAction.LastUpdatedDate = DateTime.Now;
                admAction.ActionTypeId = actiontypeid;

                _iGeneralRepository.Update(admAction);

                if (await _iGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }
    }
}

