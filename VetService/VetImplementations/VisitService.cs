using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetModel;
using VetService.VetBindingModels;
using VetService.VetInterfaces;
using VetService.VetViewModels;

namespace VetService.VetImplementations
{
    public class VisitService : IVisit
    {
        private AbstractDbContext context;

        public VisitService(AbstractDbContext context)
        {
            this.context = context;
        }

        public List<VisitViewModel> GetList()
        {
            List<VisitViewModel> result = context.Visit
                .Select(rec => new VisitViewModel
                {
                    Id = rec.Id,
                    VisitName = rec.VisitName,
                    Price = rec.Price,
                    VisitService = context.VisitService
                            .Where(recPC => recPC.VisitId == rec.Id)
                            .Select(recPC => new VisitServiceViewModel
                            {
                                Id = recPC.Id,
                                VisitId = recPC.VisitId,
                                ServiceId = recPC.ServiceId,
                                ServiceName = recPC.Service.ServiceName,
                                ServicePrice = recPC.ServicePrice
                            })
                            .ToList()
                })
                .ToList();
            return result;
        }

        public VisitViewModel GetElement(int id)
        {
            Visit element = context.Visit.FirstOrDefault(rec => rec.Id == id);
            if (element != null)
            {
                return new VisitViewModel
                {
                    Id = element.Id,
                    VisitName = element.VisitName,
                    Price = element.Price,
                    VisitService = context.VisitService
                            .Where(recPC => recPC.VisitId == element.Id)
                            .Select(recPC => new VisitServiceViewModel
                            {
                                Id = recPC.Id,
                                VisitId = recPC.VisitId,
                                ServiceId = recPC.ServiceId,
                                ServiceName = recPC.Service.ServiceName,
                                ServicePrice = recPC.ServicePrice
                            })
                            .ToList()
                };
            }
            throw new Exception("Элемент не найден");
        }

        public void AddElement(VisitBindingModel model)
        {

            Visit element = context.Visit.FirstOrDefault(rec => rec.VisitName == model.VisitName);
            if (element != null)
            {
                throw new Exception("Уже есть изделие с таким названием");
            }
            context.Visit.Add(new Visit
            {
                VisitName = model.VisitName,
                Price = model.Price
            });

            var groupComponents = model.VisitService
                                        .GroupBy(rec => rec.ServiceId)
                                        .Select(rec => new
                                        {
                                            ComponentId = rec.Key,
                                            Count = rec.Sum(r => r.ServicePrice)
                                        });
            foreach (var groupComponent in groupComponents)
            {
                context.VisitService.Add(new VetModel.VisitService
                {
                    VisitId = element.Id,
                    ServiceId = groupComponent.ComponentId,
                    ServicePrice = groupComponent.Count
                });
                context.SaveChanges();
            }
            context.SaveChanges();



        }

        public void UpdElement(VisitBindingModel model)
        {

            Visit element = context.Visit.FirstOrDefault(rec =>
                                rec.VisitName == model.VisitName && rec.Id != model.Id);
            if (element != null)
            {
                throw new Exception("Уже есть изделие с таким названием");
            }
            element = context.Visit.FirstOrDefault(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Элемент не найден");
            }
            element.VisitName = model.VisitName;
            element.Price = model.Price;
            context.SaveChanges();

            var compIds = model.VisitService.Select(rec => rec.ServiceId).Distinct();
            var updateComponents = context.VisitService
                                            .Where(rec => rec.VisitId == model.Id &&
                                                compIds.Contains(rec.ServiceId));
            foreach (var updateComponent in updateComponents)
            {
                updateComponent.ServicePrice = model.VisitService
                                                .FirstOrDefault(rec => rec.Id == updateComponent.Id).ServicePrice;
            }
            context.SaveChanges();
            context.VisitService.RemoveRange(
                                context.VisitService.Where(rec => rec.VisitId == model.Id &&
                                                                    !compIds.Contains(rec.ServiceId)));
            context.SaveChanges();
            var groupComponents = model.VisitService
                                        .Where(rec => rec.Id == 0)
                                        .GroupBy(rec => rec.ServiceId)
                                        .Select(rec => new
                                        {
                                            ComponentId = rec.Key,
                                            Count = rec.Sum(r => r.ServicePrice)
                                        });
            foreach (var groupComponent in groupComponents)
            {
                VetModel.VisitService elementPC = context.VisitService
                                        .FirstOrDefault(rec => rec.VisitId == model.Id &&
                                                        rec.ServiceId == groupComponent.ComponentId);
                if (elementPC != null)
                {
                    elementPC.ServicePrice += groupComponent.Count;
                    context.SaveChanges();
                }
                else
                {
                    context.VisitService.Add(new VetModel.VisitService
                    {
                        VisitId = model.Id,
                        ServiceId = groupComponent.ComponentId,
                        ServicePrice = groupComponent.Count
                    });
                    context.SaveChanges();
                }
            }
        }




        public void DelElement(int id)
        {

            Visit element = context.Visit.FirstOrDefault(rec => rec.Id == id);
            if (element != null)
            {
                context.VisitService.RemoveRange(
                                    context.VisitService.Where(rec => rec.VisitId == id));
                context.Visit.Remove(element);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("Элемент не найден");
            }

        }


    }
}
